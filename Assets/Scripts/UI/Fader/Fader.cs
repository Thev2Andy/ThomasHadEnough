using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public float FadeOutThreshold;
    public float FadeInThreshold;
    public Image FaderImage;

    // Private / Hidden variables..
    private bool IsFadingOut;
    private bool IsFadingIn;
    private float InterpolationTime;

    // Events..
    public event Action OnFadeOut;
    public event Action OnFadeIn;

    // Singletons..
    public static Fader Instance;

    
    public void FadeOut(float InterpolationTime = 0.5f) {
        this.InterpolationTime = InterpolationTime;
        this.IsFadingOut = true;
        this.IsFadingIn = false;

        FaderImage.color = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, 1f);
    }

    public void FadeIn(float InterpolationTime = 0.5f) {
        this.InterpolationTime = InterpolationTime;
        this.IsFadingIn = true;
        this.IsFadingOut = false;

        FaderImage.color = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, 0f);
    }


    private void Update()
    {
        if (IsFadingOut)
        {
            FaderImage.color = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, Mathf.Lerp(FaderImage.color.a, 0f, (1 - Mathf.Exp(-InterpolationTime * Time.deltaTime))));
            if (FaderImage.color.a <= FadeOutThreshold)
            {
                FaderImage.color = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, 0f);
                InterpolationTime = 0f;
                IsFadingOut = false;
                OnFadeOut?.Invoke();
            }
        }

        if (IsFadingIn)
        {
            FaderImage.color = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, Mathf.Lerp(FaderImage.color.a, 1f, (1 - Mathf.Exp(-InterpolationTime * Time.deltaTime))));
            if (FaderImage.color.a >= (1f - FadeInThreshold))
            {
                FaderImage.color = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, 1f);
                InterpolationTime = 0f;
                IsFadingIn = false;
                OnFadeIn?.Invoke();
            }
        }
    }

    private void Awake()
    {
        if (Instance != this && Instance != null) Destroy(this);
        Instance = this;

        this.FadeOut(0.95f);
    }
}
