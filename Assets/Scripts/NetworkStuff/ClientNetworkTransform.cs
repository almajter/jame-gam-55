using System;
using UnityEngine;
using Unity.Netcode.Components;
public class ClientNetworkTransform : NetworkTransform
{
    /*
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] private Transform groundCheck;
    //[SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();   
        }
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))//&& IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    /*private bool IsGrounded()
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
    }*/
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
    /*
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
    }*/
}
