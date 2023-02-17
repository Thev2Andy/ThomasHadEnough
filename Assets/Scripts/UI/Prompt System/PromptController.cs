using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptController : MonoBehaviour
{
    public TMP_Text PromptText;

    // Private / Hidden variables..
    private float Timer;

    // Singletons..
    public static PromptController Instance;

    public void Show(string Text, float Duration = float.PositiveInfinity)
    {
        PromptText.text = Text;
        this.Timer = Duration;
    }

    public void Clear() {
        PromptText.text = System.String.Empty;
    }


    private void Update()
    {
        if (Timer <= 0)
        {
            this.Clear();
            Timer = 0;
        }

        else {
            Timer -= Time.deltaTime;
        }
    }



    private void Awake()
    {
        if (Instance != this && Instance != null) Destroy(this);
        Instance = this;
    }
}
