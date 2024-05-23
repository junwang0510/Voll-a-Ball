using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    public string introSceneName = "Intro";
    public string mainSceneName = "Main";
    public string tutorialSceneName = "Tutorial";

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
}
