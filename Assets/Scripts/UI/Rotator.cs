using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private float rotationSpeed = 0.1f;
    private float yaw = 90.0f; // Horizontal rotation
    private float pitch = 70.0f; // Vertical rotation
    public Vector2 pitchLimits = new Vector2(-89, 89);

    void Start()
    {
        yaw = transform.eulerAngles.x + 90.0f;
        pitch = transform.eulerAngles.y;
    }

    void Update()
    {
        // If user use mobile device
        if (SystemInfo.deviceType != DeviceType.Desktop)
        {
            if (Input.touchCount > 0)
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
        }
        else
        {
            InputSystem();
        }
    }

    void InputSystem()
    {
        // Input system for PC
        if (SystemInfo.deviceType == DeviceType.Desktop)
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
                    pitchMatrix.SetRow(1, new Vector4(0, Mathf.Cos(pitchRad), -Mathf.Sin(pitchRad), 0));
                    pitchMatrix.SetRow(2, new Vector4(0, Mathf.Sin(pitchRad), Mathf.Cos(pitchRad), 0));
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
}
