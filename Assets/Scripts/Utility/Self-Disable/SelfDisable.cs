using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisable : MonoBehaviour
{
    public float DisableDelay;

    private void OnEnable()
    {
        StartCoroutine(DisableObject());
    }

    private IEnumerator DisableObject()
    {
        yield return new WaitForSeconds(DisableDelay);
        this.gameObject.SetActive(false);
    }
}
