using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject spherePrefab;
    public GameObject cubePrefab;
    public Canvas canvas;
    private Vector3 pos = new Vector3(0, 1, 0);

    public void OnSphereButtonPressed()
    {
        Instantiate(spherePrefab, pos, Quaternion.identity);
        ToggleCanvasComponents();
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
