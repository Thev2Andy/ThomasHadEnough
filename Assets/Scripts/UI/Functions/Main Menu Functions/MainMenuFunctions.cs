using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunctions : MonoBehaviour
{
    public int GameSceneIndex;

    public void Play() {
        SceneManager.LoadScene(GameSceneIndex);
    }

    public void Quit() { 
        Application.Quit();
    }
}
