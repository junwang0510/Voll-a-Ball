using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private float rotationSpeed = 0.2f;
    private Vector2 lastTouchPosition;

    void Update()
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
}
