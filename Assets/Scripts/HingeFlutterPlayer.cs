using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HingeFlutterPlayer : MonoBehaviour
{
    private List<Rigidbody2D> player_hinge_rbs = new List<Rigidbody2D>();

    [SerializeField] private float wind_strength = 10f;
    [SerializeField] private float wind_frequency = 5f;
    [SerializeField] private float wind_variation = 0.3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HingeJoint2D[] joints = GetComponentsInChildren<HingeJoint2D>();
        foreach (HingeJoint2D j in joints)
        {
            if (j.tag != "HingeBlow")
            {
                continue;
            }
            Rigidbody2D rb = j.gameObject.GetComponent<Rigidbody2D>();
            player_hinge_rbs.Add(rb);
        }
    }

    public void ApplyForces(Vector2 wind_direction)
    {
        foreach (Rigidbody2D rb in player_hinge_rbs)
        {
            float force = wind_strength * Mathf.Max(Mathf.Sin(Time.time * wind_frequency + Random.Range(-wind_variation, wind_variation)),-.3f);
            rb.AddForce(wind_direction * wind_strength * force);
        }
    }
}
