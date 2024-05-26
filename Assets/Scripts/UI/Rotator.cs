using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotate a mesh based on user input from mobile devices and desktop.
/// Mobile device: one finger for rotation
/// Desktop: arrow keys or WASD for rotation, control key for pitch adjustment.
/// </summary>
public class Rotator : MonoBehaviour
{
    private readonly float rotationSpeed = 0.2f;
    private float yaw = 90.0f; // Horizontal rotation
    private float pitch = 70.0f; // Vertical rotation
    public readonly Vector2 pitchLimits = new(-89, 89);

    void Start()
    {
        yaw = transform.eulerAngles.x + 90.0f;
        pitch = transform.eulerAngles.y;
    }

    void Update()
    {
        if (SystemInfo.deviceType != DeviceType.Desktop)
        {
            // Mobile device input
            HandleMobileInput();
        }
        else
        {
            // Desktop input
            HandleDesktopInput();
        }
    }

    void HandleMobileInput()
    {
        if (Input.touchCount == 1) // one finger for rotation
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 deltaPosition = touch.deltaPosition;
                float rotationX = deltaPosition.y * rotationSpeed;
                float rotationY = -deltaPosition.x * rotationSpeed;

                transform.Rotate(rotationX, rotationY, 0, Space.World);
            }
        }
        // else if (Input.touchCount == 2)  // two fingers for zooming
        // {
        //     Touch touch0 = Input.GetTouch(0);
        //     Touch touch1 = Input.GetTouch(1);

        //     Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
        //     Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

        //     float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
        //     float touchDeltaMag = (touch0.position - touch1.position).magnitude;

        //     float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        //     Camera.main.fieldOfView += deltaMagnitudeDiff * 0.1f;
        //     Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 0.1f, 179.9f);
        // }
    }

    void HandleDesktopInput()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");
        var crtlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            var movement = new Vector4(moveVertical, 0.0f, moveHorizontal, 0.0f);

            float yawRad = (yaw - 90) * Mathf.Deg2Rad;
            float pitchRad = pitch * Mathf.Deg2Rad;

            // Rotation matrix corresponding to the yawRad
            Matrix4x4 yawMatrix = new Matrix4x4();
            yawMatrix.SetRow(0, new Vector4(Mathf.Cos(yawRad), 0, -Mathf.Sin(yawRad), 0));
            yawMatrix.SetRow(1, new Vector4(0, 1, 0, 0));
            yawMatrix.SetRow(2, new Vector4(Mathf.Sin(yawRad), 0, Mathf.Cos(yawRad), 0));
            yawMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // Rotation matrix corresponding to the pitchRad
            Matrix4x4 pitchMatrix = new Matrix4x4();
            pitchMatrix.SetRow(0, new Vector4(1, 0, 0, 0));
            pitchMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            if (crtlPressed)
            {
                pitchMatrix.SetRow(1, new Vector4(0, -Mathf.Cos(pitchRad), Mathf.Sin(pitchRad), 0));
                pitchMatrix.SetRow(2, new Vector4(0, -Mathf.Sin(pitchRad), -Mathf.Cos(pitchRad), 0));
            }
            else
            {
                pitchMatrix.SetRow(1, new Vector4(0, Mathf.Sin(pitchRad), -Mathf.Cos(pitchRad), 0));
                pitchMatrix.SetRow(2, new Vector4(0, Mathf.Cos(pitchRad), Mathf.Sin(pitchRad), 0));
            }

            Matrix4x4 combinedMatrix = yawMatrix * pitchMatrix;
            Vector4 rotatedMovement = combinedMatrix * movement;
            Vector3 rotatedMovement3 = new Vector3(rotatedMovement.x, rotatedMovement.y, rotatedMovement.z) * rotationSpeed;
            transform.Rotate(rotatedMovement3, Space.World);
        }
    }
}
