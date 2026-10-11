using UnityEngine;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;
using System.Threading;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float flickerSpeedRad = 0.1f; // Speed of the flickering effect
    [SerializeField] private float flickerSpeedInt = 0.1f; // Speed of the flickering effect
    [SerializeField] private float flickerRange = 0.5f; // Int
    private float initial_inner = 0.0f;
    private float initial_intensity = 0.0f;
    private float r = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GetComponent<Light2D>() != null)
        {
            initial_inner = GetComponent<Light2D>().pointLightInnerRadius;
            initial_intensity = GetComponent<Light2D>().intensity;
        }
        r = UnityEngine.Random.Range(transform.position.x * 32, transform.position.y * 34);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Light2D>() != null)
        {
            float flicker = Mathf.Sin(Time.time * flickerSpeedRad) * flickerRange;
            GetComponent<Light2D>().pointLightInnerRadius = initial_inner + flicker;
            float intensity = math.remap(-1f,1f,.3f,1f,Mathf.Sin((Time.time + r) * flickerSpeedInt));
            GetComponent<Light2D>().intensity = initial_intensity * intensity;
        }
    }
}
