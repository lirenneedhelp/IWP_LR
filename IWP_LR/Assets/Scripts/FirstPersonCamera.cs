using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 2.0f;
    public float moveSpeed = 5.0f;

    // Limit the pitch (X rotation) of the camera
    public float maxPitch = 90.0f;
    public float minPitch = -90.0f;

    private Vector2 currentRotation = Vector2.zero;

    private void Update()
    {
        // Mouse rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Smoothly interpolate the rotation changes
        currentRotation.x -= mouseY;
        currentRotation.y += mouseX;
        currentRotation.x = Mathf.Clamp(currentRotation.x, minPitch, maxPitch);

        // Apply the smoothed rotation
        transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);

        // Keyboard movement
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        // Translate the camera based on keyboard input
        Vector3 movement = new Vector3(horizontalMovement, 0f, verticalMovement) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }
}
