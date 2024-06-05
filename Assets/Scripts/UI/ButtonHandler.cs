using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage button presses (sphere, cube)
/// </summary>
public class ButtonHandler : MonoBehaviour
{
    public GameObject spherePrefab;
    public GameObject ellipsoidePrefab;
    public GameObject cubePrefab;
    public GameObject ballPrefab;
    public GameObject CollectablePrefab;
    public Canvas canvas;
    private Timer timer; // Reference to the Timer script
    private Vector3 initPos = new Vector3(0, 1, 0);

    private void Start()
    {
        // Ensure the Timer is initialized
        timer = FindObjectOfType<Timer>();
        if (timer == null)
        {
            Debug.LogError("Timer script not found!");
        }
    }

    public void OnSphereButtonPressed()
    {
        ToggleCanvasComponents();

        // Generate the sphere and the ball
        Instantiate(spherePrefab, initPos, Quaternion.identity);
        GameObject BaseObj = GameObject.Find("Sphere(Clone)");
        Instantiate(ballPrefab, new Vector3(0, 3.625f, 0), Quaternion.identity, BaseObj.transform); // ball on the top of sphere
        // Instantiate(ballPrefab, new Vector3(0, 1f, -2f), Quaternion.identity, BaseObj.transform); // ball on the side of sphere

        // Start the timer
        timer.StartTimer();

        // Generate walls
        WallGenerator wallGenerator = new();
        wallGenerator.Generate(BaseObj);
        wallGenerator.CreateCollectable(3, CollectablePrefab);

        // Generate a transparent shell around the sphere
        HollowSphere hollowSphere = new();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        hollowSphere.GenerateHollowSphere(BaseObj, meshFilter, meshRenderer);

        GameObject ball = GameObject.Find("Ball(Clone)");
        CollisionHandler collisionHandler = ball.GetComponent<CollisionHandler>();
        collisionHandler.SetEndCylinder(wallGenerator.GetEndCylinder());
    }

    public void OnCubeButtonPressed()
    {
        ToggleCanvasComponents();
        Instantiate(cubePrefab, initPos, Quaternion.identity);
        GameObject BaseObj = GameObject.Find("Cube(Clone)");
        CubeMeshGenerator generator = BaseObj.AddComponent<CubeMeshGenerator>();
        generator.GenerateMesh(BaseObj);

        // Start the timer
        timer.StartTimer();

        // Generate walls
        WallGenerator wallGenerator = new();
        wallGenerator.Generate(BaseObj);
        wallGenerator.CreateCollectable(3, CollectablePrefab);

        // Generate a transparent shell around the Cube
        HollowCube hollowCube = new();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        hollowCube.GenerateHollowCube(BaseObj, meshFilter, meshRenderer, BaseObj);

        Instantiate(ballPrefab, new Vector3(0.125f, 2.15f, 0.125f), Quaternion.identity, BaseObj.transform); // ball on the top of sphere
        GameObject ball = GameObject.Find("Ball(Clone)");
        ball.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        CollisionHandler collisionHandler = ball.GetComponent<CollisionHandler>();
        collisionHandler.SetEndCylinder(wallGenerator.GetEndCylinder());
    }

    private void ToggleCanvasComponents()
    {
        foreach (Transform child in canvas.transform)
        {
            if (child.gameObject.name == "BackButton") continue;
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }
}
