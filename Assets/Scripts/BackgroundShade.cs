using UnityEngine;

public class BackgroundShade : MonoBehaviour
{
    [SerializeField] private Color shadeColor = new Color(0.0f, 0.0f, 0.0f); // Dark gray color
    [SerializeField] private float shadeAlpha = 0.6f; // Alpha value for the shade
    [SerializeField] private float fadeSpeed = 0.6f; // Alpha value for the shade

    private bool isPlayerInside = false; // Flag to track if the player is inside the background shade area
    private float cur_alpha = 0.0f; // Current alpha value of the background shade
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInside) {
            // Gradually increase the alpha value to create a smooth transition effect
            cur_alpha = Mathf.Lerp(cur_alpha, shadeAlpha, Time.deltaTime * fadeSpeed);
        } else {
            // Gradually decrease the alpha value to create a smooth transition effect
            cur_alpha = Mathf.Lerp(cur_alpha, 0.0f, Time.deltaTime * fadeSpeed);
        }
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SpriteRenderer>() == null) continue;
            child.GetComponent<SpriteRenderer>().color = new Color(shadeColor.r, shadeColor.g, shadeColor.b, cur_alpha);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}
