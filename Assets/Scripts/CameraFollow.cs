using UnityEngine;

public class CameraFollow : MonoBehaviour
{

     [SerializeField] private GameObject player; 

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
        transform.position = new Vector3(Mathf.Min(Mathf.Max(player.transform.position.x, cur_max), cur_min), ylevel, transform.position.z);
    }
}
