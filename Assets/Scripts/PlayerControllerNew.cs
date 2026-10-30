using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal;

public class PlayerControllerNew : MonoBehaviour
{
    // These variables hold the key-bind reference. They are then to be initialized in void Start.
    InputAction moveAction;
    InputAction jumpAction;
    InputAction interactAction;
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
    [SerializeField] private float falling_anim_delay = 0.1f;
    [SerializeField] private GameObject pickup;
    [SerializeField] private GameObject handik;

    private Vector2 velocity;
    private Vector2 moveInput;
    private PlayerState currentPlayerState = PlayerState.FALLING;
    private PlayerState previousPlayerState = PlayerState.GROUNDED;
    private Rigidbody2D rb;
    private float ground_ray_dist = 1f;
    private float wall_ray_dist = 1f;
    private float last_wall_jump_time = 0.0f;
    public GameObject debugText;
    private Vector2 current_grounded_forward = Vector2.right;
    private float ground_hit_distance = 0.1f;
    private float started_falling = 0.0f;
    private bool jumped = false;
    private float coyote = 0.1f;
    private bool has_coyoted = false;
    private bool isPickingUp = false;
    private bool hasPickedUp = false;

    
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
            interactAction = inputActionAsset.FindAction("Interact");
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

    private RaycastHit2D CheckGrounded()
    {
        RaycastHit2D hit;
        int numrays = 5;
        Vector2 start_pos = new Vector2(foot.transform.position.x - 0.3f, foot.transform.position.y);
        start_pos = new Vector2(foot.transform.position.x, foot.transform.position.y) - current_grounded_forward * 0.3f;
        hit = Physics2D.Raycast(start_pos, -Vector2.Perpendicular(current_grounded_forward), ground_ray_dist, LayerMask.GetMask("Ground"));
        
        for(int i = 1; i < numrays; i++)
        {
            // weird early exit because we begin check before loop starts T_T STUPID BRAIN IDIOT THING
            if (hit.collider != null && hit.distance < ground_hit_distance) break;
            Vector2 ray_origin = new Vector2(foot.transform.position.x, foot.transform.position.y) - current_grounded_forward * 0.3f + current_grounded_forward * 0.5f / (numrays - 1) * i;
            Debug.DrawRay(ray_origin, -Vector2.Perpendicular(current_grounded_forward) * ground_ray_dist, Color.red);
            hit = Physics2D.Raycast(ray_origin, -Vector2.Perpendicular(current_grounded_forward), ground_ray_dist, LayerMask.GetMask("Ground"));
        }
        Vector2 normal = hit.normal;
        if (hit && -Vector2.Perpendicular(normal).y < 0.4f){ 
            current_grounded_forward = -Vector2.Perpendicular(normal);
        } else
        {
            current_grounded_forward = Vector2.right;
        }
        Debug.DrawRay(foot.transform.position, current_grounded_forward * 0.5f, Color.green);
        //Debug.Log("Grounded forward: " + current_grounded_forward.ToString());
        return hit;
    }


    void ChangeState(PlayerState newState)
    {
        previousPlayerState = currentPlayerState;
        currentPlayerState = newState;
        if (animator != null)
        {
            animator.ResetTrigger("land");
            animator.ResetTrigger("grab_wall");
        }

        if (newState == PlayerState.FALLING)
        {
            started_falling = Time.time;
        }
        if (newState == PlayerState.GROUNDED)
        {
            SetAnimationTrigger("land");
            animator.ResetTrigger("jump");
            has_coyoted = false;
        }
        if (newState == PlayerState.ON_WALL)
        {
            animator.ResetTrigger("jump");
            SetAnimationTrigger("grab_wall");
        }
    }

    void UpdateGroundedState()
    {
        /// !!!!!!!! VERY JANK DO NOT DO !!!!!!!!!!!!!
        if (isPickingUp)
        {
            rb.linearVelocity = Vector2.zero;
            AnimatorClipInfo[] aci = animator.GetCurrentAnimatorClipInfo(0);
            AnimatorStateInfo asi = animator.GetCurrentAnimatorStateInfo(0);
            
            if(aci[0].clip.name == "pickup" && asi.normalizedTime > 0.01f)
            {
                // parent interactable to hand
                pickup.GetComponent<Light2D>().enabled = false;
                pickup.transform.position = handik.transform.position;
                Vector3 ea = -handik.transform.localEulerAngles;
                ea.z -= 90f;
                pickup.transform.localEulerAngles = ea;
            }
            if(aci[0].clip.name == "pickup" && asi.normalizedTime > .9f)
            {
                pickup.GetComponent<SpriteRenderer>().enabled = false;
                isPickingUp = false;
                hasPickedUp = true;
                Destroy(pickup);
            }
            return;
        }
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        //velocity.x = moveInput.x * maxSpeed;

        velocity = current_grounded_forward * Mathf.Sign(moveInput.x) * moveInput.magnitude * maxSpeed;

       if (jumpAction.triggered)
        {
            velocity.y = jumpForce;
            jumped = true;
            ChangeState(PlayerState.FALLING);
            SetAnimationTrigger("jump");
            has_coyoted = true;
        }

        if (interactAction.triggered && Vector2.Distance(pickup.transform.position, transform.position) < 1.5f && !hasPickedUp)
        {
            SetAnimationTrigger("interact");
            isPickingUp = true;
            foreach(Transform child in pickup.transform)
            {
                if(child.GetComponent<TextMeshPro>() != null)
                {
                    child.GetComponent<TextMeshPro>().enabled = false;
                }
            }
        }

        RaycastHit2D hit = CheckGrounded();
        if ((hit.collider == null) || hit.distance > ground_hit_distance)
        {
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
            jumped = true;
            ChangeState(PlayerState.FALLING);
            SetAnimationTrigger("jump");
            has_coyoted = true;
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
            SetAnimationTrigger("drop");
        }
    }

    private void UpdateFallingState()
    {   
        if (previousPlayerState != PlayerState.ON_WALL) {
             velocity.x = Mathf.Sign(moveInput.x) * moveInput.magnitude * maxSpeed;
             velocity.x = Mathf.MoveTowards(velocity.x, 0, Time.deltaTime * maxSpeed * 2); // Apply friction when in the air
        }
        if (jumpAction.triggered && started_falling + coyote > Time.time && previousPlayerState == PlayerState.GROUNDED && !has_coyoted)
        {
            velocity.y = jumpForce;
            jumped = true;
            SetAnimationTrigger("jump");
            has_coyoted = true;
        }
        // Check for ground
        RaycastHit2D hit = CheckGrounded();
        //Debug.Log(hit.distance);
        Debug.DrawRay(hit.point, Vector2.down * ground_ray_dist, Color.red);
        if (velocity.y <= 0 && hit.collider != null && hit.distance < ground_hit_distance)
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
            //Debug.Log("On Wall");
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

    private void SetAnimationTrigger(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    private void UpdateAnimationState() {
        if (animator == null) return;
        animator.SetBool("running", Mathf.Abs(moveInput.x) > 0.01f);
        animator.SetBool("falling", currentPlayerState == PlayerState.FALLING && ((started_falling + falling_anim_delay < Time.time) || jumped));
        animator.SetBool("on_wall", currentPlayerState == PlayerState.ON_WALL || jumped);
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
                if (moveInput.x > 0.01f && !isPickingUp)
                {
                    avatarRoot.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (moveInput.x < -0.01f && !isPickingUp)
                {
                    avatarRoot.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
        if(debugText)
        {
            debugText.GetComponent<Text>().text = "Position: " + transform.position.ToString() + "\nVelocity: " + velocity.ToString() + "\nState: " + currentPlayerState.ToString() + "\nPrevious State: " + previousPlayerState.ToString() + "\nFPS: " + (int)(1.0f / Time.deltaTime)
            + "\nRes:" + Screen.width + "x" + Screen.height;
        }
        UpdateAnimationState();
        jumped = false;
        if (GetComponent<HingeFlutterPlayer>() != null && Mathf.Abs(velocity.x) > 0.01f)
        {
            GetComponent<HingeFlutterPlayer>().ApplyForces(-velocity.normalized);
        }
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
