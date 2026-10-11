using UnityEngine;

public class PlayerLightHandler : MonoBehaviour
{
    private float targetIntensity = 0.0f; // Target intensity for the global light
    private float fadeSpeed = 0.6f; // Fade speed for the global light
    private float initIntensity = 0.0f;
    
    public void SetTargetIntensity(float intensity, float fadeSpeed, bool returnToInit = false)
    {
        targetIntensity = returnToInit ? intensity : initIntensity;
        this.fadeSpeed = fadeSpeed;
    }
    void Start()
    {
        if (GetComponent<UnityEngine.Rendering.Universal.Light2D>() != null)
        {
            initIntensity = GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity;
            GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 0f;
        }
    }

    void Update()
    {
        if (GetComponent<UnityEngine.Rendering.Universal.Light2D>() != null)
        {
            GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity, targetIntensity, Time.deltaTime * fadeSpeed);
        }
    }
}
