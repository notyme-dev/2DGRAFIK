using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BackgroundShade : MonoBehaviour
{
    [SerializeField] private Color shadeColor = new Color(0.0f, 0.0f, 0.0f); // Dark gray color
    [SerializeField] private float shadeAlpha = 0.6f; // Alpha value for the shade
    [SerializeField] private float fadeSpeed = 0.6f; // Alpha value for the shade
    [SerializeField] private Light2D globalLight; // Reference to the global light source
    [SerializeField] private GameObject[] backgroundBlockers; // Array of background blocker objects

    private GameObject player; // Reference to the player object

    private bool isPlayerInside = false; // Flag to track if the player is inside the background shade area
    private float tar_alpha = 0.0f; // Current alpha value of the background shade
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (isPlayerInside) {
            // Gradually increase the alpha value to create a smooth transition effect
            cur_alpha = Mathf.Lerp(cur_alpha, shadeAlpha, Time.deltaTime * fadeSpeed);
            if (globalLight != null)
            {
                globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.35f, Time.deltaTime * fadeSpeed);
            }
            if (player != null)
            {
                if (player.GetComponent<Light2D>() != null)
                {
                    player.GetComponent<Light2D>().intensity = Mathf.Lerp(player.GetComponent<Light2D>().intensity, 0.46f, Time.deltaTime * fadeSpeed);
                }
            }
        } else {
            // Gradually decrease the alpha value to create a smooth transition effect
            cur_alpha = Mathf.Lerp(cur_alpha, 0.0f, Time.deltaTime * fadeSpeed);
            if (globalLight != null)
            {
                globalLight.intensity = Mathf.Lerp(globalLight.intensity, 1.0f, Time.deltaTime * fadeSpeed);
            }
            if (player != null)
            {
                if (player.GetComponent<Light2D>() != null)
                {
                    player.GetComponent<Light2D>().intensity = Mathf.Lerp(player.GetComponent<Light2D>().intensity, 0f, Time.deltaTime * fadeSpeed);
                }
            }
        }*/
        foreach(GameObject bgBlocker in backgroundBlockers)
        {
            if (bgBlocker.GetComponent<SpriteRenderer>() == null) continue;
            float newAlpha = Mathf.Lerp(bgBlocker.GetComponent<SpriteRenderer>().color.a, tar_alpha, fadeSpeed * Time.deltaTime);
            bgBlocker.GetComponent<SpriteRenderer>().color = new Color(shadeColor.r, shadeColor.g, shadeColor.b, newAlpha);
        }
        //0.46
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        tar_alpha = shadeAlpha;
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            player = other.gameObject; // Store the reference to the player object
            if (player.GetComponent<PlayerLightHandler>() != null)
            {
                player.GetComponent<PlayerLightHandler>().SetTargetIntensity(0.46f, fadeSpeed); // Set the target intensity for the player's light
            }
            if (globalLight != null && globalLight.GetComponent<GlobalLightHandler>() != null)
            {
                globalLight.GetComponent<GlobalLightHandler>().SetTargetIntensity(0.35f, fadeSpeed); // Set the target intensity for the global light 
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        tar_alpha = 0f;
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            player = other.gameObject; // Store the reference to the player object
            if (player.GetComponent<PlayerLightHandler>() != null)
            {
                player.GetComponent<PlayerLightHandler>().SetTargetIntensity(0f, fadeSpeed, true); // Set the target intensity for the player's light
            }
            if (globalLight != null && globalLight.GetComponent<GlobalLightHandler>() != null)
            {
                globalLight.GetComponent<GlobalLightHandler>().SetTargetIntensity(1f, fadeSpeed); // Set the target intensity for the global light 
            }
        }
    }
}
