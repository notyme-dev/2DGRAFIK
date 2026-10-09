using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightHandler : MonoBehaviour
{
    private float target_intensity = 1.0f; // Target intensity for the global light
    private float fadeSpeed = 0.6f; // Fade speed for the global light
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetTargetIntensity(float intensity, float fadeSpeed)
    {
        target_intensity = intensity;
        this.fadeSpeed = fadeSpeed;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Light2D>() != null)
        {
            GetComponent<Light2D>().intensity = Mathf.Lerp(GetComponent<Light2D>().intensity, target_intensity, Time.deltaTime * fadeSpeed);
        }
    }
}
