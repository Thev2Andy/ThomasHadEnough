using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Background;
    public GameObject PauseUI;
    public GameObject[] OtherUserInterfaces;
    public int MenuSceneIndex;

    // Private / Hidden variables..
    private float OldTimescale;

    // Singletons..
    public static PauseMenu Instance;

    // Properties..
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if(Instance != this) Destroy(Instance);
        Instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) this.InvokePause();
    }

    public void InvokePause()
    {
        if(!IsPaused) {
            this.Pause();
        }
        
        else {
            this.Resume();
        }
    }

    public void Pause()
    {
        Background.SetActive(true);
        PauseUI.SetActive(true);
        for (int I = 0; I < OtherUserInterfaces.Length; I++)
        {
            OtherUserInterfaces[I].SetActive(false);
        }

        OldTimescale = Time.timeScale;
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resume()
    {
        Background.SetActive(false);
        PauseUI.SetActive(false);
        for (int I = 0; I < OtherUserInterfaces.Length; I++)
        {
            OtherUserInterfaces[I].SetActive(false);
        }

        Time.timeScale = OldTimescale;
        IsPaused = false;
    }

    public void Quit() {
        SceneManager.LoadSceneAsync(MenuSceneIndex);
    }

    private void OnDisable() {
        this.Resume();
    }

}
