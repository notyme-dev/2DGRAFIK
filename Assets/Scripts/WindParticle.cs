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

        if (Vector2.Distance(transform.position, startPos) > 0.25f)
            {
                last_contact_time = Time.time;
                collider.isTrigger = true;
            }
            else {
                if(last_contact_time + contact_cooldown < Time.time && !collider.IsTouchingLayers(LayerMask.GetMask("Player")))
                {
                    collider.isTrigger = false;
                }
            }

        if (collider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            //isActive = false;
             
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

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color col = collider.isTrigger ? new Color(1f,0.5f,0.5f) : new Color(1f,1f,1f);
        sr.color = col;
    }
}
