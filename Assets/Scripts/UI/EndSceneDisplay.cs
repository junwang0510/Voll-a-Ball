using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

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

            // Read the past elapsed time stored PlayerHistory.txt
            string path = Application.dataPath + "/PlayerHistory.txt";
            float pastElapsedTime = -0.1f;
            if (System.IO.File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                if (lines.Length > 0)
                {
                    pastElapsedTime = float.Parse(lines[lines.Length - 1]);
                }
            }

            float elapsedTime = timer.GetElapsedTime();
            float bestTime = (pastElapsedTime > 0) ? Mathf.Min(pastElapsedTime, elapsedTime) : elapsedTime;
            timeText.text = "Total time: " + elapsedTime.ToString("F2") + " seconds\n";
            timeText.text += "Best time: " + bestTime.ToString("F2") + " seconds";

            // Store the best time in PlayerHistory.txt; Overwrite the file
            System.IO.File.WriteAllText(path, bestTime.ToString("F2"));
        }
        else
        {
            Debug.LogError("Timer script not found!");
        }
    }
}
