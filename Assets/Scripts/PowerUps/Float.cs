using UnityEngine;

public class Float : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.25f; // how high it moves
    [SerializeField] private float frequency = 2f;    // how fast it moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
    }
}