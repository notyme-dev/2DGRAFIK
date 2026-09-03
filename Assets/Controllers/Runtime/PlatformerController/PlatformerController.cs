using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(CapsuleCollider2D))]
public class PlatformerController : MonoBehaviour
{
    // These variables hold the key-bind reference. They are then to be initialized in void Start.
    InputAction moveAction;
    InputAction jumpAction;

    // Serialized referenses.
    [Header("References")]
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] private Transform avatarRoot;
    [SerializeField] private Animator animator;

    // Customizable settings.
    [Header("Settings")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravityMultiplier = 6f;

    [SerializeField] private GameObject front;

    public bool controlEnabled { get; set; } = true; // Editable from Unity Events

    private Vector2 moveInput;
    private Rigidbody2D rb;

    // Platformer specific variables
    private CircleCollider2D groundCheckCollider;
    private LayerMask groundLayer = ~0;
    private Vector2 velocity;
    //private bool jumpInput;
    private bool jumpReleased;
    private bool wasGrounded;
    private bool isGrounded;

    private bool isOnWall = false;
    private CapsuleCollider2D wallCheckCollider;

    [SerializeField] private float wallfallSpeed = 1f;


    [SerializeField] public GameObject debugText;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        groundCheckCollider = GetComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;

        wallCheckCollider = GetComponent<CapsuleCollider2D>();
        wallCheckCollider.isTrigger = true;


        // Set gravity scale to 0 so player won't "fall" 
        rb.gravityScale = 0;
    }

    private void Start()
    {
        // Assign the correct action to each variable
        moveAction = inputActionAsset.FindAction("Move");
        jumpAction = inputActionAsset.FindAction("Jump");
    }

    void Update()
    {
        velocity = TranslateInputToVelocity(moveInput); 

        if (animator != null)
        {
            animator.SetFloat("velocityX", rb.linearVelocity.x);
            animator.SetFloat("velocityY", rb.linearVelocity.y);
        }

        // Move
        moveInput = moveAction.ReadValue<Vector2>().normalized;

        // Ground contact checks
        if (wasGrounded && !isGrounded)
        {
            if (velocity.y < 0)
            {
                // Has fallen
            }
            else
            {
                // Has jumped
            }
        }
        else if (!wasGrounded && isGrounded)
        {
            jumpReleased = false;
        }
        wasGrounded = isGrounded;


        // Apply jump-input:
        if (jumpAction.IsPressed() && wasGrounded)
        {
            velocity.y = jumpForce;
        //    jumpInput = false;
        }

        if (!jumpAction.IsPressed() && !isGrounded)
        {
            jumpReleased = true;
        //    jumpInput = false;
        }

        // Flip character according to direction (if one has been assigned)
        if (avatarRoot)
        {
            if (moveInput.x > 0.01f)
            {
                avatarRoot.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveInput.x < -0.01f)
            {
                avatarRoot.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

       /* if (!isGrounded && !isOnWall)
        {
            if(wallCheckCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                isOnWall = true;
            }
        }*/

        debugText.GetComponent<Text>().text = "isGrounded: " + isGrounded + "\nisOnWall: " + isOnWall;
    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        ApplyGravity();
        rb.linearVelocity = velocity;

        if (animator != null)
        {
            animator.SetBool("grounded", isGrounded);
            animator.SetFloat("speedY", velocity.y);
            animator.SetFloat("speedX", velocity.x);
        }
    }

    private bool IsGrounded()
    {
        return groundCheckCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = 1.0f;
        }
        else
        {
            if (velocity.y > 0)
            {
                float deceleration = jumpReleased ? 30 : 7;
                velocity.y += Physics2D.gravity.y * deceleration * Time.deltaTime;
            }
            else
            {
                velocity.y += Physics2D.gravity.y * gravityMultiplier * Time.deltaTime;
            }
        }
    }

    Vector2 TranslateInputToVelocity(Vector2 input)
    {
        return new Vector2(input.x * maxSpeed, velocity.y);
    }
}
