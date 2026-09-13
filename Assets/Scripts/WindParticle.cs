using UnityEngine;

public class WindParticle : MonoBehaviour
{
    public Vector2 startPos;
    public bool wasTouched = false;
    public bool isActive = true;

    private float last_contact_time = 0f;
    private float contact_cooldown = 0.3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null) return;

        if (collider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            last_contact_time = Time.time;
            //isActive = false;
             if (Vector2.Distance(transform.position, startPos) > 0.005f)
            {
                collider.isTrigger = true;
            }
            else {
                if(last_contact_time + contact_cooldown < Time.time)
                {
                    collider.isTrigger = false;
                }
            }
            wasTouched = true;
        }
        else
        {
            //isActive = true;
            if (wasTouched)
            {
                collider.isTrigger = false;
                //transform.position = startPos;
                wasTouched = false;
            }
        }
    }
}
