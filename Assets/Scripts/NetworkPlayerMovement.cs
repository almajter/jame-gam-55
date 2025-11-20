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

    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpingPower = 6f;
    [SerializeField] private float kickPower = 2f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject powerBarGameObject;
    [SerializeField] private GameObject kickPowerIndicator;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        HandleKickPower(KeyCode.Z);
        HandlePowerBar();
        Flip();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
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
            RequestKickServerRpc(targetId);
        }
        // TODO add kick depending on kickPowerIndicator.transform.localScale.y (0-1)
    }

    [ServerRpc]
    void RequestKickServerRpc(ulong targetClientId)
    {
        Debug.Log($"TargetClientId: {targetClientId}");
        ApplyKickClientRpc(targetClientId, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { targetClientId }
            }
        });
    }

    [ClientRpc]
    void ApplyKickClientRpc(ulong targetClientId, ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;
        Debug.Log("AAAAAAAAA");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
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