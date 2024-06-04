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
    public GameObject ballPrefab;
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

        // Generate a transparent shell around the sphere
        HollowSphere hollowSphere = new();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        hollowSphere.GenerateHollowSphere(BaseObj, meshFilter, meshRenderer);
    }

    public void OnEllipsoidButtonPressed()
    {

        ToggleCanvasComponents();
        Instantiate(ellipsoidePrefab, initPos, Quaternion.identity);
        GameObject BaseObj = GameObject.Find("Ellipsoid(Clone)");

        // Start the timer
        timer.StartTimer();

        // Generate walls
        WallGenerator wallGenerator = new();
        wallGenerator.Generate(BaseObj);

        Vector3 scale = BaseObj.transform.localScale;

        // Generate a transparent shell around the sphere
        HollowSphere hollowSphere = new();
        hollowSphere.SetScale(scale / 5);
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        hollowSphere.GenerateHollowSphere(BaseObj, meshFilter, meshRenderer, BaseObj);

        BaseObj.transform.rotation = Quaternion.Euler(0, 0, 90);
        MeshCollider sphereCollider = BaseObj.GetComponent<MeshCollider>();
        // Scale the collider
        sphereCollider.transform.localScale = scale;
        Instantiate(ballPrefab, new Vector3(0, 3.625f, 0), Quaternion.identity, BaseObj.transform); // ball on the top of sphere
    }

    private void ToggleCanvasComponents()
    {
        foreach (Transform child in canvas.transform)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }
}
