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

    private Button qButton;
    private Button leftButton;
    private Button rightButton;
    private Button upButton;
    private Button downButton;

    void Start()
    {
        yaw = transform.eulerAngles.x + 90.0f;
        pitch = transform.eulerAngles.y;

        qButton = GameObject.Find("QButton").GetComponent<Button>();
        leftButton = GameObject.Find("LeftButton").GetComponent<Button>();
        rightButton = GameObject.Find("RightButton").GetComponent<Button>();
        upButton = GameObject.Find("UpButton").GetComponent<Button>();
        downButton = GameObject.Find("DownButton").GetComponent<Button>();

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            HideMobileButtons();
        }
        else
        {
            ShowMobileButtons();
        }
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

        // // (Unfinished) Handle button input for mobile
        // qButton.onClick.AddListener(() => MoveSphere(KeyCode.Q));
        // leftButton.onClick.AddListener(() => MoveSphere(KeyCode.LeftArrow));
        // rightButton.onClick.AddListener(() => MoveSphere(KeyCode.RightArrow));
        // upButton.onClick.AddListener(() => MoveSphere(KeyCode.UpArrow));
        // downButton.onClick.AddListener(() => MoveSphere(KeyCode.DownArrow));
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

    // // (Unfinished)
    // void MoveSphere(KeyCode key)
    // {
    //     float moveHorizontal = 0;
    //     float moveVertical = 0;
    //     bool pitchKeyPressed = false;

    //     switch (key)
    //     {
    //         case KeyCode.Q:
    //             pitchKeyPressed = true;
    //             break;
    //         case KeyCode.LeftArrow:
    //             moveHorizontal = -1;
    //             break;
    //         case KeyCode.RightArrow:
    //             moveHorizontal = 1;
    //             break;
    //         case KeyCode.UpArrow:
    //             moveVertical = 1;
    //             break;
    //         case KeyCode.DownArrow:
    //             moveVertical = -1;
    //             break;
    //     }

    //     MoveSphere(moveHorizontal, moveVertical, pitchKeyPressed);
    // }

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

    void HideMobileButtons()
    {
        qButton.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        upButton.gameObject.SetActive(false);
        downButton.gameObject.SetActive(false);
    }

    void ShowMobileButtons()
    {
        qButton.gameObject.SetActive(true);
        leftButton.gameObject.SetActive(true);
        rightButton.gameObject.SetActive(true);
        upButton.gameObject.SetActive(true);
        downButton.gameObject.SetActive(true);
    }
}
