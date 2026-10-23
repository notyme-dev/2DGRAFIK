using TMPro;
using UnityEngine;

public class InteractTextFade : MonoBehaviour
{
    [SerializeField] private float show_distance = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TextMeshPro tmp = GetComponent<TextMeshPro>();
        if(tmp == null) return;
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        if(player == null) return;
        float op = 1 - Mathf.Min(show_distance,Vector2.Distance(player.transform.position, transform.position)) / show_distance;
        Color c = tmp.color;
        c.a = op;
        tmp.color = c;
    }
}
