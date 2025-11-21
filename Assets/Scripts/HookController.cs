using Unity.Netcode;
using UnityEngine;

public class HookController2D : NetworkBehaviour
{
    public float speed = 10f;
    public float pullSpeed = 5f;

    private Vector2 direction;
    private Vector2 startPoint;
    private bool pulling = false;
    private NetworkObject playerToPull;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            startPoint = transform.position;
        }
    }

    void Update()
    {
        if (!IsServer) return;

        if (!pulling)
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
        else if (playerToPull != null)
        {
            Rigidbody2D rb = playerToPull.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 newPos = Vector2.MoveTowards(rb.position, startPoint, pullSpeed * Time.fixedDeltaTime);
                playerToPull.GetComponent<NetworkPlayerMovement>().MoveHookClientRpc(playerToPull.OwnerClientId, newPos);
                // rb.MovePosition(newPos);
                transform.position = newPos;
            }

            if (Vector2.Distance(playerToPull.transform.position, startPoint) < 0.05f)
            {
                NetworkObject.Destroy(gameObject);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            var netObj = other.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                pulling = true;
                playerToPull = netObj;

                var playerMovement = other.GetComponent<NetworkPlayerMovement>();
                if (playerMovement != null)
                    playerMovement.canMove = false;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (playerToPull != null)
        {
            var playerMovement = playerToPull.GetComponent<NetworkPlayerMovement>();
            if (playerMovement != null)
                playerMovement.canMove = true;
        }
    }
}
