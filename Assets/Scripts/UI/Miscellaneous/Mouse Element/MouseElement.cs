using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseElement : MonoBehaviour
{
    public RectTransform ElementTransform;

    private void Update()
    {
        if (!PauseMenu.Instance.IsPaused) {
            ElementTransform.position = Input.mousePosition;
        }
    }
}
