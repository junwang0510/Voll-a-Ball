using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Load scenes (intro, main, tutorial, end)
/// </summary>
public class LoadScenes : MonoBehaviour
{
    public string introSceneName = "Intro";
    public string mainSceneName = "Main";
    public string tutorialSceneName = "Tutorial";
    public string endSceneName = "End";

    public void LoadIntro()
    {
        SceneManager.LoadScene(introSceneName);
    }

    public void LoadMain()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void LoadEnd()
    {
        SceneManager.LoadScene(endSceneName);
    }
}
