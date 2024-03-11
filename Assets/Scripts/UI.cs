using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public void nextLevel(string next)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(next);
    }

    public void exit() 
    {
        Application.Quit();
    }
}
