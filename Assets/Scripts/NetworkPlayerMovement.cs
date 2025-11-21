using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Components;

public class NetworkPlayerMovement : NetworkBehaviour
{
    private float horizontal;
    private bool isFacingRight = true;
    private bool isZPressed;
    private bool isPowerGoingUp;
    private Camera cam;
    public bool canMove = true;

    [SerializeField] public float speed = 10f;
    [SerializeField] public float acceleration = 20f;
    [SerializeField] public float deceleration = 30f;
    [SerializeField] private float jumpingPower = 6f;
    [SerializeField] private float kickPower = 3f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject powerBarGameObject;
    [SerializeField] private GameObject kickPowerIndicator;
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private GameObject firePoint;

    void Start()
    {
        if (IsOwner)
        {
            cam = Camera.main;
            Camera.main.GetComponent<CameraFollow>().target = transform;
            Camera.main.GetComponent<CameraFollow>().offset = new Vector3(0, 1, -10);
        }
    }

    void Update()
    {
        if (!IsOwner) return;
        if (canMove)
        {
            HandleNormalMovement();
            HandleKickPower(KeyCode.Z);
            HandlePowerBar();

            if (Input.GetKeyDown(KeyCode.E))
            {
                Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0f;
                Vector2 direction = (mouseWorld - firePoint.transform.position).normalized;
                RequestFireHookServerRpc(direction);
            }
        }
    }

    private void HandleNormalMovement()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");

        float targetSpeed = horizontal * speed;
        float currentSpeed = rb.linearVelocity.x;

        float accel = (Mathf.Abs(targetSpeed) > 0.1f) ? acceleration : deceleration;

        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.deltaTime);

        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
        HandleFlip();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void HandleFlip()
    {
        if (isFacingRight && rb.linearVelocity.x < 0f || !isFacingRight && rb.linearVelocity.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void HandleKickPower(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            SetKickPower(true);
        }
        else if (Input.GetKeyUp(key))
        {
            SetKickPower(false);
        }
    }

    private void SetKickPower(bool isPressed)
    {
        isZPressed = isPressed;
        powerBarGameObject.SetActive(isPressed);

        if (isPressed)
        {
            kickPowerIndicator.transform.localScale = new Vector3(0.3f, 0f, 0f);
        }
        else
        {
            HandleKick();
        }
    }

    private void HandlePowerBar()
    {
        if (isZPressed)
        {
            float diff = Time.deltaTime * 0.5f * (isPowerGoingUp ? 1f : -1f);
            float newY = kickPowerIndicator.transform.localScale.y + diff;
            newY = Mathf.Clamp(newY, 0f, 1f);
            kickPowerIndicator.transform.localScale = new Vector3(0.3f, newY, 0);

            if (newY >= 1f)
            {
                isPowerGoingUp = false;
            }
            else if (newY <= 0f)
            {
                isPowerGoingUp = true;
            }
        }
    }

    private void HandleKick()
    {
        float checkDistance = 1f;
        Vector3 position = transform.position;
        position.x = position.x + (isFacingRight ? 1 : -1) * 0.6f;
        RaycastHit2D hit2D = Physics2D.Raycast(position, isFacingRight ? Vector2.right : Vector2.left, checkDistance);

        if (hit2D.collider != null && hit2D.collider.CompareTag("Player"))
        {
            ulong targetId = hit2D.collider.GetComponent<NetworkObject>().OwnerClientId;
            RequestKickServerRpc(targetId, kickPowerIndicator.transform.localScale.y, isFacingRight);
        }
    }

    [ServerRpc]
    void RequestKickServerRpc(ulong targetClientId, float strength, bool isFacingRight)
    {
        ApplyKickClientRpc(targetClientId, strength, isFacingRight, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { targetClientId }
            }
        });
    }

    [ClientRpc]
    void ApplyKickClientRpc(ulong targetClientId, float strength, bool isFacingRight, ClientRpcParams rpcParams = default)
    {
        NetworkObject targetObj = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        Rigidbody2D rb2D = targetObj.GetComponent<Rigidbody2D>();
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;
        rb2D.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * jumpingPower * kickPower * strength, jumpingPower * kickPower * strength);
    }

    [ServerRpc]
    void RequestFireHookServerRpc(Vector2 direction)
    {
        GameObject hookObj = Instantiate(hookPrefab, firePoint.transform.position, Quaternion.identity);

        hookObj.GetComponent<HookController2D>().SetDirection(direction);

        hookObj.GetComponent<NetworkObject>().Spawn(true);
    }

    [ClientRpc]
    public void MoveHookClientRpc(ulong targetClientId, Vector2 newPos, ClientRpcParams rpcParams = default)
    {
        NetworkObject targetObj = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;
        Rigidbody2D rb2D = targetObj.GetComponent<Rigidbody2D>();
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;
        rb2D.MovePosition(newPos);
    }

    void ApplyJumpPowerUp(float jumpMultiplier)
    {
        jumpingPower *= jumpMultiplier;
        Debug.Log($"jump increased by {(jumpMultiplier - 1) * 100}%");
    }

    private void OnEnable()
    {
        GameEvents.OnJumpBoostPickedUp += ApplyJumpPowerUp;
    }

    private void OnDisable()
    {
        GameEvents.OnJumpBoostPickedUp -= ApplyJumpPowerUp;
    }
}