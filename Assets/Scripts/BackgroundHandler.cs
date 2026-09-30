using UnityEngine;

public class BackgroundHandler : MonoBehaviour
{
    private float startPos_x, start_pos_y, length;
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallaxEffect_x, parallaxEffect_y;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos_x = transform.position.x;
        start_pos_y = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance_x = cam.transform.position.x * parallaxEffect_x;
        float distance_y = cam.transform.position.y * parallaxEffect_y;
        float movement_x = cam.transform.position.x * (1 - parallaxEffect_x);
        float movement_y = cam.transform.position.y * (1 - parallaxEffect_y);

        transform.position = new Vector3(startPos_x + distance_x, start_pos_y + distance_y, transform.position.z);

        if (movement_x > startPos_x + length)
        {
            startPos_x += length;
        }
        else if (movement_x < startPos_x - length)
        {
            startPos_x -= length;
        }
        if (movement_y > start_pos_y + length)
        {
            start_pos_y += length;
        }
        else if (movement_y < start_pos_y - length)
        {
            start_pos_y -= length;
        }
    }
}
