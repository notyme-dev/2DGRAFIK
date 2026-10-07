using UnityEngine;

public class CameraFollow : MonoBehaviour
{

     [SerializeField] private GameObject player; 
    [SerializeField] private float smoothSpeed_x = 2f;
    [SerializeField] private float smoothSpeed_y = 3f;
    [SerializeField] private float velocity_scalar = 1f;
     [SerializeField] private float velocity_accum = 1f;

      [SerializeField] private float y_offset = .5f;

    private float velocity = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        float cur_min = player.GetComponent<CameraRegionChecker>().current_camera_region.end.transform.position.x;
        float cur_max = player.GetComponent<CameraRegionChecker>().current_camera_region.start.transform.position.x;
        float ylevel = player.GetComponent<CameraRegionChecker>().current_camera_region.y_level;
        float pvel = 0;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            pvel = rb.linearVelocityX;
        }
        velocity = Mathf.Lerp(velocity, pvel, velocity_accum * Time.deltaTime);
        float new_x = Mathf.Min(Mathf.Max(player.transform.position.x + velocity * velocity_scalar, cur_max), cur_min);
        float new_y = Mathf.Max(player.transform.position.y + y_offset, ylevel);
        float smooth_x = Mathf.Lerp(transform.position.x, new_x, smoothSpeed_x * Time.deltaTime);
        float smooth_y = Mathf.Lerp(transform.position.y, new_y, smoothSpeed_y * Time.deltaTime);
        transform.position = new Vector3(smooth_x, smooth_y, transform.position.z);
    }
}
