using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OverJump : MonoBehaviour
{
    private InputController inputActions;
    private CharacterController characterController;
    private Animator animator;

    [SerializeField] private float duoblePressTime = 0.3f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSfx;

    private float lastJumpPressTime = 0f;
    private bool overJumpTriggered = false;

    private void Awake()
    {
        inputActions = new InputController();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void HandleOverJump()
    {
        if (inputActions.Player.OverJump.triggered)
        {
            if (Time.time - lastJumpPressTime <= duoblePressTime)
            {
                overJumpTriggered = true;
                animator.SetTrigger("SpecialJump");
                PlayerJumpSound();
            }
            else
            {
                overJumpTriggered = false;
            }

            lastJumpPressTime = Time.time;
            
        }
    }

    private void PlayerJumpSound()
    {
        if (audioSource != null && jumpSfx != null)
        {
            audioSource.PlayOneShot(jumpSfx); // Play jump sound effect
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
