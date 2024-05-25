using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject spherePrefab;
    public GameObject cubePrefab;
    public GameObject ballPrefab;
    public Canvas canvas;
    private Vector3 pos = new Vector3(0, 1, 0);

    public void OnSphereButtonPressed()
    {
        Instantiate(spherePrefab, pos, Quaternion.identity);
        GameObject BaseObj = GameObject.Find("Sphere(Clone)");
        Instantiate(ballPrefab, new Vector3(0, 3.625f, 0), Quaternion.identity, BaseObj.transform);

        ToggleCanvasComponents();

        WallGenerator wallGenerator = new();
        wallGenerator.Generate(BaseObj);

        HollowSphere hollowSphere = new();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        hollowSphere.GenerateHollowSphere(BaseObj, meshFilter, meshRenderer);

    }

    public void OnCubeButtonPressed()
    {
        Instantiate(cubePrefab, pos, Quaternion.identity);
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
