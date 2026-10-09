using UnityEngine;

public class PlayerLightHandler : MonoBehaviour
{
    private float target_intensity = 1.0f; // Target intensity for the global light
    private float fadeSpeed = 0.6f; // Fade speed for the global light

    private float initIntensity = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetTargetIntensity(float intensity, float fadeSpeed, bool returnToInit = false)
    {
        target_intensity = returnToInit ? intensity : 0f;
        this.fadeSpeed = fadeSpeed;
    }
    void Start()
    {
        if (GetComponent<UnityEngine.Rendering.Universal.Light2D>() != null)
        {
            initIntensity = GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<UnityEngine.Rendering.Universal.Light2D>() != null)
        {
            GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity, target_intensity, Time.deltaTime * fadeSpeed);
        }
    }
}
