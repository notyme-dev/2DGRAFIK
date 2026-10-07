using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    [SerializeField] private float wind_strength = 10f;
    [SerializeField] private Vector2 wind_direction = new Vector2(1f, 0f);

    [SerializeField] private float wind_frequency = 5f;

    [SerializeField] private float wind_variation = 0.3f;
    
    private List<Collider2D> colliders = new List<Collider2D>();
    public List<Collider2D> GetColliders () { return colliders; }

    private void OnTriggerEnter2D (Collider2D other) {
        if (!colliders.Contains(other)) { colliders.Add(other); }
    }

    private void OnTriggerExit2D (Collider2D other) {
        colliders.Remove(other);
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Collider2D a in colliders)
        {
            //a.gameObject.GetComponent<Rigidbody2D>().AddForce(wind_direction * wind_strength);
            HingeJoint2D[] joints = a.gameObject.GetComponentsInChildren<HingeJoint2D>();
            foreach (HingeJoint2D j in joints)
            {
                if (j.tag != "HingeBlow")
                {
                    continue;
                }
                Rigidbody2D rb = j.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float force = wind_strength * (Mathf.Sin(Time.time * wind_frequency + Random.Range(-wind_variation, wind_variation)) + 1f) / 2f;
                    rb.AddForce(wind_direction * wind_strength * force);
                }
            }
        }
    }
}
