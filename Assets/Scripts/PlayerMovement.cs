using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return; // If you are not the owner of this object, don't do shit
    }
}
