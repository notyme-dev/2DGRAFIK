using UnityEngine;

public class CameraFollow : MonoBehaviour
{

     [SerializeField] private GameObject player; 
    [SerializeField] private float smoothSpeed = 0.005f;
    [SerializeField] private float smoothDistance = 1f;

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
        float new_x = Mathf.Min(Mathf.Max(player.transform.position.x, cur_max), cur_min);
        if((Mathf.Abs(transform.position.x - cur_max) < smoothDistance && transform.position.x < cur_max) 
        || (Mathf.Abs(transform.position.x - cur_min) < smoothDistance && transform.position.x > cur_min))
        {
            Debug.Log("Smooth camera movement");
            new_x = Mathf.MoveTowards(transform.position.x, new_x, smoothSpeed * Time.deltaTime);
        }
        transform.position = new Vector3(new_x, ylevel, transform.position.z);
    }
}
