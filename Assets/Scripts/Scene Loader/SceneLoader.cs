using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public HealthSystem PlayerHealthSystem;
    public GameObject BlackPanel;
    public SceneMonitor SceneMonitor;

    // Private / Hidden variables..
    private int CurrentScene;
    private int SceneToLoad;

    // Singletons..
    public static SceneLoader Instance;


    public void StartLoadingLevel(int Index)
    {
        SceneToLoad = Index;
        Fader.Instance.OnFadeIn += Load;
        Fader.Instance.FadeIn(10f);
    }

    public void Load()
    {
        BlackPanel?.SetActive(true);
        SceneMonitor?.CleanScene();

        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);

        try {
            SceneManager.UnloadSceneAsync(CurrentScene);
        }
        
        catch (System.ArgumentException) {
            // Swallow it, it doesn't really matter if the scene is invalid.
        }

        Fader.Instance.OnFadeIn -= Load;
        CurrentScene = SceneToLoad;

        BlackPanel?.SetActive(false);

        if (PlayerHealthSystem != null) StartCoroutine(PlayerHealthSystem.TeleportToRandomSpawnpoint(true));
        Fader.Instance.FadeOut(10f);
    }


    private void Awake()
    {
        if (Instance != this && Instance != null) Destroy(this);
        Instance = this;
    }
}
