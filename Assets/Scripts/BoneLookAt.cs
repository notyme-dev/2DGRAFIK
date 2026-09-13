using UnityEngine;

public class BoneLookAt : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float maxAngle, minAngle = 90f;
    private Vector3 startAngle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startAngle = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        float vx = target.position.x - transform.position.x;
        float vy = target.position.y - transform.position.y;
        float angle = Mathf.Atan2(vy, vx) * Mathf.Rad2Deg;
        transform.localEulerAngles = new Vector3(startAngle.x, startAngle.y, Mathf.Clamp(angle, minAngle, maxAngle));
    }
}
