using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTrigger : MonoBehaviour
{
    public int SceneToLoad;
    private bool Loading;

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.transform.CompareTag("Player") && !Loading)
        {
            SceneLoader.Instance.StartLoadingLevel(SceneToLoad);
            SceneLoader.Instance.PlayerHealthSystem.transform.position = new Vector3(0, 100000, 0);
            if (SceneLoader.Instance.PlayerHealthSystem.Health < SceneLoader.Instance.PlayerHealthSystem.InitialHealth) {
                SceneLoader.Instance.PlayerHealthSystem.Health = SceneLoader.Instance.PlayerHealthSystem.InitialHealth;
            }

            Loading = true;
        }
    }
}
