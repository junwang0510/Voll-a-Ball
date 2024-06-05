using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    private LoadScenes loadScenesScript;
    private GameObject endCylinder = null;
    private readonly int NUM_COLLECTABLES = 1;
    private int collected;

    void Start()
    {
        loadScenesScript = FindObjectOfType<LoadScenes>();
        collected = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("RedCylinder") && collected == NUM_COLLECTABLES)
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            other.gameObject.SetActive(false);
            collected++;
        }

        if (collected == NUM_COLLECTABLES)
        {
            endCylinder.GetComponent<Renderer>().material.color = Color.red;
            Vector3 originalScale = endCylinder.transform.localScale;
            originalScale.x *= (10.0f / 7.0f);
            originalScale.z *= (10.0f / 7.0f);
            endCylinder.transform.localScale = originalScale;
            Debug.Log("Goal is now open!");
            Debug.Log("Go find the red cylinder to finish the maze!");
        }
    }

    public void SetEndCylinder(GameObject endCylinder)
    {
        this.endCylinder = endCylinder;

        if (endCylinder == null)
        {
            Debug.LogError("End cylinder not found!");
        }
    }
}
