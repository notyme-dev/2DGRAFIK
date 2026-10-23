using UnityEngine;

public class PickupAnim : MonoBehaviour
{
    [SerializeField] private float animSpeed = 1f;
    [SerializeField] private float animDist = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.y += Mathf.Sin(Time.time * animSpeed) * animDist;
        transform.position = pos;
    }
}
