using UnityEngine;

/// <summary>
/// Handles the camera movement responsible for rendering the board.
/// <para></para>
/// Middle mouse drag pan the camera, and scroll wheel changes the camera fov (zoom).
/// </summary>
public class BoardCamera : MonoBehaviour
{
    [SerializeField] private float _panSpeed = 800f; // Base speed at which the camera pans
    [SerializeField] private float _zoomSpeed = 10f; // Speed at which the camera zooms
    [SerializeField] private float _minFov = 40f; // Minimum field of view
    [SerializeField] private float _maxFov = 100f; // Maximum field of view
    [SerializeField] private Vector2 _panLimit; // Limits for panning

    private Vector3 _dragOrigin; // Stores the initial position when panning starts

    private Camera _camera;

    private void OnEnable()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        // Handle zooming with mouse scroll, clamping the field of view to the defined limits
        float fov = _camera.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
        fov = Mathf.Clamp(fov, _minFov, _maxFov);
        _camera.fieldOfView = fov;

        // Adjust pan speed based on the current field of view to maintain consistent panning
        float adjustedPanSpeed = _panSpeed * (_camera.fieldOfView / 70f); // Normalized to a base FOV of 70

        // Start panning when the middle mouse button is pressed
        if (Input.GetMouseButtonDown(2))
        {
            _dragOrigin = Input.mousePosition;
            return;
        }

        // Continue panning while the middle mouse button is held down
        if (Input.GetMouseButton(2))
        {
            // Calculate the movement based on mouse position change since the drag started
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
            Vector3 move = new Vector3(pos.x * adjustedPanSpeed, pos.y * adjustedPanSpeed, 0);

            // Apply the movement to the camera's position
            transform.Translate(-move, Space.World);

            // Clamp the camera's position to prevent it from moving out of bounds
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -_panLimit.x, _panLimit.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -_panLimit.y, _panLimit.y);
            transform.position = clampedPosition;

            // Update drag origin for the next frame
            _dragOrigin = Input.mousePosition;
        }
    }
}