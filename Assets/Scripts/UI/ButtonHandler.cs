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
    public GameObject cubePrefab;
    public GameObject ballPrefab;
    public Canvas canvas;
    private Vector3 initPos = new Vector3(0, 1, 0);

    public void OnSphereButtonPressed()
    {
        // Generate the sphere and the ball
        Instantiate(spherePrefab, initPos, Quaternion.identity);
        GameObject BaseObj = GameObject.Find("Sphere(Clone)");
        Instantiate(ballPrefab, new Vector3(0, 3.625f, 0), Quaternion.identity, BaseObj.transform); // ball on the top of sphere
        // Instantiate(ballPrefab, new Vector3(0, 1f, -2f), Quaternion.identity, BaseObj.transform); // ball on the side of sphere

        ToggleCanvasComponents();

        // Generate walls
        WallGenerator wallGenerator = new();
        wallGenerator.Generate(BaseObj);

        // Generate a transparent shell around the sphere
        HollowSphere hollowSphere = new();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        hollowSphere.GenerateHollowSphere(BaseObj, meshFilter, meshRenderer);
    }

    public void OnCubeButtonPressed()
    {
        Instantiate(cubePrefab, initPos, Quaternion.identity);
        ToggleCanvasComponents();
    }

    private void ToggleCanvasComponents()
    {
        foreach (Transform child in canvas.transform)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }
}
