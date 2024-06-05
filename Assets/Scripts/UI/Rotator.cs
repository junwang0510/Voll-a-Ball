using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        HandleDesktopInput();
    }

    void HandleDesktopInput()
    {
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");
        var pitchKeyPressed = Input.GetKey(KeyCode.Q);

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            MoveSphere(moveHorizontal, moveVertical, pitchKeyPressed);
        }
    }

    void MoveSphere(float moveHorizontal, float moveVertical, bool pitchKeyPressed)
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

        if (pitchKeyPressed)
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
