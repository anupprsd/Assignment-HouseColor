using System;
using UnityEngine;

// Ensures that the GameObject this script is attached to has a Camera component
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private WallColorController wallController; // Reference to wall controller script for selecting walls
    [SerializeField] private float mobileLookSensitivity = 0.3f; // Sensitivity for looking around on mobile
    [SerializeField] private float pcLookSensitivity = 2f;       // Sensitivity for looking around on PC
    [SerializeField] private GameObject crossHair;               // Crosshair UI element for PC

    private Camera fpsCamera; 
    private float mouseX;     
    private float mouseY;     
    private float touchX;     
    private float touchY;     

    // Tap detection settings
    private float tapMaxTime = 0.2f;      
    private float tapMaxMovement = 10f;   
    private float touchStartTime;         
    private Vector2 touchStartPos;        

    private void Start()
    {
        // Lock cursor to center of screen (for PC)
        Cursor.lockState = CursorLockMode.Locked;

        fpsCamera = GetComponent<Camera>();

        // Enable crosshair only if platform is NOT Android or iOS
        if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            crossHair.SetActive(true);
        }

        Application.targetFrameRate = 100;
    }

    private void Update()
    {
        // Do nothing if the game is in UI state
        if (ColourPicker.state == GameState.UI) return;

        // Use mobile or PC input depending on platform
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            HandleMobileInput();
        }
        else
        {
            HandlePCInput();
        }
    }

    private void HandleMobileInput()
    {
        if (Input.touchCount > 0) // Check if there is at least one touch
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Record touch start time and position for tap detection
                    touchStartTime = Time.time;
                    touchStartPos = touch.position;
                    break;

                case TouchPhase.Moved:
                    // Update camera rotation based on finger drag
                    touchX += touch.deltaPosition.x * mobileLookSensitivity;
                    touchY -= touch.deltaPosition.y * mobileLookSensitivity;

                    // Clamp vertical rotation to avoid flipping camera upside down
                    touchY = Mathf.Clamp(touchY, -90, 90);

                    // Apply rotation to camera
                    Vector3 targetEulerRotation = new Vector3(touchY, touchX);
                    fpsCamera.transform.rotation = Quaternion.Euler(targetEulerRotation);
                    break;

                case TouchPhase.Ended:
                    // Check if touch qualifies as a tap
                    float touchDuration = Time.time - touchStartTime;
                    float touchDistance = (touch.position - touchStartPos).magnitude;

                    if (touchDuration <= tapMaxTime && touchDistance <= tapMaxMovement)
                    {
                        // If tap detected select wall at tapped position
                        wallController.SelectWall(touch.position);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private void HandlePCInput()
    {
        // Update rotation based on mouse movement
        mouseX += Input.GetAxis("Mouse X") * pcLookSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * pcLookSensitivity;

        // Clamp vertical rotation to avoid flipping camera upside down
        mouseY = Mathf.Clamp(mouseY, -90, 90);

        // Apply rotation to camera
        Vector3 targetEulerRotation = new Vector3(mouseY, mouseX);
        fpsCamera.transform.rotation = Quaternion.Euler(targetEulerRotation);
         
        // Left click selects a wall
        if (Input.GetMouseButtonDown(0))
        {
            wallController.SelectWall(Input.mousePosition);
        }
    }
}
