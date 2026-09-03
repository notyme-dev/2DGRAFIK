using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownController : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    Vector2 movementInput;
    [SerializeField] bool runToggle;
    [SerializeField] float movementSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] Animator myAnimator;
    float actualSpeed;


    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if  bool runToggle is active
        if (runToggle == true)
        {
            actualSpeed = movementSpeed + runSpeed;
        }
        else
        {
            actualSpeed = movementSpeed;
        }

        if (myAnimator != null)
        {
            myAnimator.SetFloat("speedX", System.MathF.Round(myRigidbody.linearVelocity.x, 1));
            myAnimator.SetFloat("speedY", System.MathF.Round(myRigidbody.linearVelocity.y, 1));
            myAnimator.SetFloat("velocity", System.MathF.Round(myRigidbody.linearVelocity.magnitude, 2));
        }

    }

    // FixedUpdate is called every frame when the physics are calculated
    void FixedUpdate()
    {
        // Movement with acceleration, or...
        myRigidbody.AddForce(movementInput * actualSpeed);

        // ...you could use the following instead for instant movement
        // myRigidbody.velocity = movementInput * movementSpeed;
    }

    private void OnMove(InputValue inputValue)
    {
        //Debug.Log("OnMove is moving");
        movementInput = inputValue.Get<Vector2>();
    }

    private void OnRun(InputValue inputValue)
    {
        Debug.Log("OnRun is running " + inputValue.isPressed);
        runToggle = inputValue.isPressed;
    }

}
