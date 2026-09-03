using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerControllerNew : MonoBehaviour
{
    // These variables hold the key-bind reference. They are then to be initialized in void Start.
    InputAction moveAction;
    InputAction jumpAction;
    // Serialized referenses.
    [Header("References")]
    [SerializeField] InputActionAsset inputActionAsset;
    [SerializeField] private Transform avatarRoot;
    [SerializeField] private Animator animator;

    [Header("Ray points")]
    [SerializeField] private GameObject foot;
    [SerializeField] private GameObject hand;
    // Customizable settings.
    [Header("Settings")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravityMultiplier = 6f;
    [SerializeField] private float terminal_y = 50f;
    [SerializeField] private float gravityMultiplierWall = 1f;

    [SerializeField] private float jump_hold_deacceleration = 7f;
    [SerializeField] private float jump_deacceleration = 30f;
    [SerializeField] private float wall_jump_repel = 1.0f;
    [SerializeField] private float wall_jump_force = 1.0f;

    private Vector2 velocity;
    private Vector2 moveInput;
    private PlayerState currentPlayerState = PlayerState.FALLING;
    private PlayerState previousPlayerState = PlayerState.GROUNDED;
    private Rigidbody2D rb;
    private float ground_ray_dist = 1f;
    private float wall_ray_dist = 1f;

    private float last_wall_jump_time = 0.0f;

    public GameObject debugText;

    

    enum PlayerState
    {
        GROUNDED,
        FALLING,   
        ON_WALL,
    }   
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inputActionAsset != null)
        {
            inputActionAsset.Enable();
            moveAction = inputActionAsset.FindAction("Move");
            jumpAction = inputActionAsset.FindAction("Jump");
            if (moveAction != null) moveAction.Enable();
            if (jumpAction != null) jumpAction.Enable();
        }
        else
        {
            Debug.LogWarning("InputActionAsset not assigned on " + gameObject.name);
        }

        rb = GetComponent<Rigidbody2D>();
        currentPlayerState = PlayerState.FALLING;
    }

    void ChangeState(PlayerState newState)
    {
        previousPlayerState = currentPlayerState;
        currentPlayerState = newState;
    }

    void UpdateGroundedState()
    {
        velocity.x = moveInput.x * maxSpeed;

       if (jumpAction.IsPressed())
        {
            velocity.y = jumpForce;
            ChangeState(PlayerState.FALLING);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, ground_ray_dist, LayerMask.GetMask("Ground"));
        if (hit.collider == null)
        {
            Debug.DrawRay(transform.position, Vector2.down * ground_ray_dist, Color.red);
            ChangeState(PlayerState.FALLING);
        }
    }

    private void UpdateOnWallState()
    {
        //velocity.x = moveInput.x * maxSpeed;
        if (jumpAction.triggered)
        {
            velocity.y = jumpForce;
            velocity.x = avatarRoot.transform.localScale.x * maxSpeed * wall_jump_repel; // Jump away from the wall
            last_wall_jump_time = Time.time;
            ChangeState(PlayerState.FALLING);
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, ground_ray_dist, LayerMask.GetMask("Ground"));
        if (velocity.y < 0 && hit.collider != null)
        {
            Debug.DrawRay(transform.position, Vector2.down * ground_ray_dist, Color.red);
            ChangeState(PlayerState.GROUNDED);
            velocity.y = 0.0f;
        }
        hit = Physics2D.Raycast(hand.transform.position, Vector2.right * -avatarRoot.transform.localScale.x, wall_ray_dist, LayerMask.GetMask("Ground"));
        if (hit.collider == null)
        {
            Debug.DrawRay(hand.transform.position, Vector2.right * -avatarRoot.transform.localScale.x * wall_ray_dist, Color.blue);
            ChangeState(PlayerState.FALLING);
        }
    }

    private void UpdateFallingState()
    {   
        
        if (previousPlayerState != PlayerState.ON_WALL) {
             velocity.x = moveInput.x * maxSpeed;
             velocity.x = Mathf.MoveTowards(velocity.x, 0, Time.deltaTime * maxSpeed * 2); // Apply friction when in the air
        }
        // Check for ground
        RaycastHit2D hit;
        int numrays = 5;
        Vector2 start_pos = new Vector2(foot.transform.position.x - 0.5f, foot.transform.position.y);
        hit = Physics2D.Raycast(start_pos, Vector2.down, ground_ray_dist, LayerMask.GetMask("Ground"));
        Debug.Log(hit.distance);
        Debug.DrawRay(start_pos, Vector2.down * ground_ray_dist, Color.red);
        for(int i = 1; i < numrays; i++)
        {
            if (hit.collider != null) break;
            Vector2 ray_origin = new Vector2(start_pos.x + 0.5f / (numrays - 1) * i, start_pos.y);
            hit = Physics2D.Raycast(ray_origin, Vector2.down, ground_ray_dist, LayerMask.GetMask("Ground"));
        }
        if (velocity.y <= 0 && hit.collider != null && hit.distance < 0.1f)
        {
            ChangeState(PlayerState.GROUNDED);
            velocity.y = 0.0f;
            //transform.position = new Vector2(transform.position.x, hit.point.y); // Adjust the player's position to be on top of the ground
        }
        // Check for walls
        hit = Physics2D.Raycast(hand.transform.position, Vector2.right * Mathf.Sign(moveInput.x), wall_ray_dist, LayerMask.GetMask("Ground"));
        if (hit.collider != null && hit.distance < 0.1f && Time.time - last_wall_jump_time > 0.01f)
        {
            Debug.DrawRay(transform.position, Vector2.right * Mathf.Sign(moveInput.x) * wall_ray_dist, Color.blue);
            Debug.Log("On Wall");
            ChangeState(PlayerState.ON_WALL);
            avatarRoot.transform.localScale = new Vector3(-avatarRoot.transform.localScale.x, avatarRoot.transform.localScale.y, avatarRoot.transform.localScale.z); 
            velocity.y = 0.0f;
            velocity.x = 0.0f;
        }

        if (velocity.y <= -terminal_y && rb.linearVelocityY >= -0.1f){
            velocity.y = 0f;
            ChangeState(PlayerState.GROUNDED);
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction != null ? moveAction.ReadValue<Vector2>().normalized : Vector2.zero;
        if (currentPlayerState == PlayerState.GROUNDED)
        {
            UpdateGroundedState();
        }
        else if (currentPlayerState == PlayerState.FALLING)
        {
            UpdateFallingState();
        }
        else if (currentPlayerState == PlayerState.ON_WALL)
        {
            UpdateOnWallState();
        }
        
        if (avatarRoot)
        {
            if (currentPlayerState != PlayerState.ON_WALL)
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
        }

        debugText.GetComponent<Text>().text = "Position: " + transform.position.ToString() + "\nVelocity: " + velocity.ToString() + "\nState: " + currentPlayerState.ToString() + "\nPrevious State: " + previousPlayerState.ToString();
    }

    void FixedUpdate() 
    {
        if (currentPlayerState == PlayerState.GROUNDED && velocity.y < 0.0f)
        {
            velocity.y = 1.0f;
        }
        if (currentPlayerState == PlayerState.FALLING)
        {
            if (velocity.y > 0)
            {
                float deceleration = !jumpAction.IsPressed() ? jump_deacceleration : jump_hold_deacceleration;
                velocity.y += Physics2D.gravity.y * deceleration * Time.deltaTime;
            }
            else
            {
                velocity.y += Physics2D.gravity.y * gravityMultiplier * Time.deltaTime;
            }
        } 
        else if (currentPlayerState == PlayerState.ON_WALL)
        {
            velocity.y += Physics2D.gravity.y * gravityMultiplierWall * Time.deltaTime;
        } 
        Mathf.Clamp(velocity.y, -terminal_y, terminal_y);
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
    }
}
