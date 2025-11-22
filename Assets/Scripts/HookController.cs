using Unity.Netcode;
using UnityEngine;

public class HookController2D : NetworkBehaviour
{
    float speed = 10f;
    float pullSpeed = 10f;

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
            transform.position = Vector2.MoveTowards(transform.position, startPoint, pullSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, startPoint) < 0.05f)
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
                GetComponent<Collider2D>().enabled = false;
                transform.position = other.gameObject.transform.position;
                var player = other.GetComponent<NetworkPlayerMovement>();
                StartPullClientRpc(player.OwnerClientId, startPoint);
            }
        }
    }

    [ClientRpc]
    public void StartPullClientRpc(ulong targetClientId, Vector2 pullPoint)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        var player = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject.GetComponent<NetworkPlayerMovement>();

        player.BeginBeingPulled(pullPoint);
    }

    [ClientRpc]
    public void StopPullClientRpc(ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        var player = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject.GetComponent<NetworkPlayerMovement>();

        player.StopPulling();
    }

    public override void OnNetworkDespawn()
    {
        if (playerToPull != null)
        {
            var player = playerToPull.GetComponent<NetworkPlayerMovement>();
            StopPullClientRpc(player.OwnerClientId);
        }
    }
}
