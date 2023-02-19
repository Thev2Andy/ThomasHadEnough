using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneMonitor : MonoBehaviour
{
    public GameObject[] AllowedRootObjects;

    public void CleanScene()
    {
        GameObject[] Roots = this.gameObject.scene.GetRootGameObjects();
        foreach (GameObject Root in Roots)
        {
            if (!AllowedRootObjects.Contains(Root))
            {
                Destroy(Root);
            }
        }
    }
}
