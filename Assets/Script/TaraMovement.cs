using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaraMovement : MonoBehaviour
{
    private InputController inputActions;
    private Animator animator;
    private CharacterController characterController;

    [SerializeField] private OverJump jumpController;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private Transform cameraTransform;
    [Space]
    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckRedius = 0.4f;

    [Space]
    [Header("Audio Setting")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footStepClips;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.4f;

    private float turnSmoothVelocity;
    private Vector3 velocity;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isRunning;
    private bool wasGrounded;
    private Vector3 direction;
    private float stepTimer;

    private void Awake()
    {
        inputActions = new InputController();
    }
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stepTimer = walkStepInterval; // Initialize the step timer
    }
    
    void Update()
    {
        HandleMovement();
        HandleJumping();

        jumpController.HandleOverJump();
    }

    private void HandleMovement()
    {
        // Movement Input
        Vector2 inputVector = inputActions.Player.Movement.ReadValue<Vector2>();
        float horizontal = inputVector.x;
        float vertical = inputVector.y;
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Check if player is running
        isRunning = inputActions.Player.Sprint.IsPressed();

        // Check if moving
        if (direction.magnitude >= 0.1f)
        {
            // Calculate target angle based on camera direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotate character smoothly
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            // Calculate movement direction relative to the camera
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Set movement speed (run or walk)
            float speed = isRunning ? runSpeed : moveSpeed;
            characterController.Move(moveDirection * speed * Time.deltaTime);

            //Adjust the step interval based on whether the player is running or walk
            float currentStepInterval = isRunning ? runStepInterval : walkStepInterval;

            // Set animation blend values
            animator.SetFloat("speed", speed);

            // Play footstep sound if grounded and moving
            if (isGrounded)
            {
                stepTimer -= Time.deltaTime; // Descrease  timer

                if (stepTimer <= 0)
                {
                    PlayFootStepSound(); // Play random step sound
                    stepTimer = currentStepInterval; // Reset timer
                }

            }
        }
        else
        {
            animator.SetFloat("speed", 0f); // Idle animation
            stepTimer = walkStepInterval; // Reset timer when idle
        }

    }

    private void HandleJumping()
    {
        // Check if the character is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRedius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset velocity when grounded
            animator.SetBool("isFall", false); // Stop falling animation

            // Play landing sound effect if player was inthe air
            if (!wasGrounded)
            {
                PlayLandSound();
            }
        }

        wasGrounded = isGrounded; // Keep track of whether the player was grounded

        if (isGrounded)
        {
            animator.SetBool("isJump", false); //Stop jump animation
            animator.SetBool("isFall", false); // Stop falling animation

            // Trigger jump
            if (inputActions.Player.Jump.triggered)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Jump Calculate
                animator.SetBool("isJump", true); // Play jump animation
                PlayJumpSound();
            }
        }

        else
        {
            // if not grounded, apply gravity
            velocity.y += gravity * Time.deltaTime;

            // Handle falling animationn
            if (velocity.y < 5f)
            {
                animator.SetBool("isFall", true);
            }
        }

        // Apply gravity over time
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }

    private void PlayFootStepSound()
    {
        if (footStepClips.Length > 0)
        {
            // Randomly select a clip from the array
            int randomIndex = UnityEngine.Random.Range(0, footStepClips.Length);
            AudioClip footStepClip = footStepClips[randomIndex];

            // Play the selected footstep sound
            audioSource.PlayOneShot(footStepClip);
        }
    }

    private void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    private void PlayLandSound()
    {
        if (landSound != null)
        {
            audioSource.PlayOneShot(landSound);
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
