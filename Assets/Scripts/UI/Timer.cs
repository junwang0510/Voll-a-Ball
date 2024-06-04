using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private static Timer instance;
    private float startTime;
    private float elapsedTime;
    private bool isRunning;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetTimer();
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        startTime = 0f;
        elapsedTime = 0f;
        isRunning = false;
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime = Time.time - startTime;
        }
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
