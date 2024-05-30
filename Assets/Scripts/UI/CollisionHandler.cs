using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Load the end scene when the ball reaches the end (i.e. collide with the red cylinder)
/// </summary>
public class CollisionHandler : MonoBehaviour
{
    private LoadScenes loadScenesScript;

    void Start()
    {
        loadScenesScript = FindObjectOfType<LoadScenes>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("RedCylinder"))
        {
            if (loadScenesScript != null)
            {
                loadScenesScript.LoadEnd();
            }
            else
            {
                Debug.LogError("LoadScenes script not found!");
            }
        }
    }
}
