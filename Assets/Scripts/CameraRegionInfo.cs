using UnityEngine;

public class CameraRegionInfo : MonoBehaviour
{
    public GameObject start;
    public GameObject end;
    public float y_level = 0f;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            (other.GetComponent<CameraRegionChecker>()).current_camera_region = this;
        }
    }
}
