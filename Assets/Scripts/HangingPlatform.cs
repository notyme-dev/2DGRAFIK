using Unity.VisualScripting;
using UnityEngine;

public class HangingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject joint_1;
    [SerializeField] private GameObject joint_2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector2 joint_1_pos = joint_1.transform.position;
        Vector2 joint_2_pos = joint_2.transform.position;

        Vector2 direction = (joint_2_pos - joint_1_pos).normalized;
        float distance = Vector2.Distance(joint_1_pos, joint_2_pos);

        Vector2 target_pos = joint_1_pos + direction * distance * 0.5f;

        transform.rotation = Quaternion.FromToRotation(Vector2.right, direction);
        transform.position = target_pos;
        //rb.MovePosition(target_pos);
    }
}
