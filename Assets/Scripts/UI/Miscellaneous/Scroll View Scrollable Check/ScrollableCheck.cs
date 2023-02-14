using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollableCheck : MonoBehaviour
{
    public RectTransform ScrollViewContent;
    public RectTransform ScrollView;

    public GameObject[] ObjectsToEnableIfScrollable;


    private void Update()
    {
        for (int I = 0; I < ObjectsToEnableIfScrollable.Length; I++)
        {
            ObjectsToEnableIfScrollable[I].SetActive((ScrollViewContent.rect.height > ScrollView.rect.height));
        }
    }
}
