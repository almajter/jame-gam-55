using System;
using UnityEngine;

public class JumpBoost : MonoBehaviour
{
    [SerializeField] private float jumpForceMultiplier = 1.5f; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.OnJumpBoostPickedUp?.Invoke(jumpForceMultiplier);
            Debug.Log("Jump power up picked up");
            Destroy(gameObject);
        }
    }
}
