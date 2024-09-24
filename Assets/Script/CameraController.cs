using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private InputController inputActions;

    [SerializeField] private Transform target;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float offset;
    [SerializeField] private float mouseSensitive = 2.0f;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        inputActions = new InputController();
    }
    void Start()
    {
        
    }

    
    void LateUpdate()
    {
        // Get mouse input
        Vector2 mouseLook = inputActions.Player.MouseLook.ReadValue<Vector2>();
        yaw += mouseLook.x * mouseSensitive;
        pitch -= mouseLook.y * mouseSensitive;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // Limit the vertical rotation

        // Set camera position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position - rotation * Vector3.forward * distance * offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update camera position and rotation
        transform.position = smoothPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // Adjust to focus on the character

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
