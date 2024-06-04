using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndSceneDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    private void Start()
    {
        // Find the Timer script
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            // Stop the timer and display the elapsed time
            timer.StopTimer();
            float elapsedTime = timer.GetElapsedTime();
            timeText.text = "Total time: " + elapsedTime.ToString("F2") + " seconds";
        }
        else
        {
            Debug.LogError("Timer script not found!");
        }
    }
}
