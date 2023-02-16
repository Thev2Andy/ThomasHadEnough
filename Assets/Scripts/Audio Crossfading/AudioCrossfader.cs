using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCrossfader : MonoBehaviour
{
    public int VolumeChangesPerSecond = 15;
    
    // Private / Hidden variables..
    private AudioSource[] Players;
    private IEnumerator[] Faders = new IEnumerator[2];
    private int ActivePlayer = 0;
    private float FadeDuration = 1.0f;
    [Range(0.0f, 1.0f)] private float Volume = 1.0f;

    // Properties..
    public bool Mute
    {
        get {
            return Players[ActivePlayer].mute;
        }

        set {
            foreach (AudioSource Source in Players) {
                Source.mute = value;
            }
        }
    }

    // Singletons..
    public static AudioCrossfader Instance;


    public void Play(AudioClip Clip, float FadeDuration)
    {
        if (Clip != Players[ActivePlayer].clip)
        {
            this.FadeDuration = FadeDuration;

            foreach (IEnumerator I in Faders)
            {
                if (I != null)
                {
                    StopCoroutine(I);
                }
            }

            if (Players[ActivePlayer].volume > 0)
            {
                Faders[0] = this.FadeAudioSource(Players[ActivePlayer], this.FadeDuration, 0.0f, () => { Faders[0] = null; });
                StartCoroutine(Faders[0]);
            }

            int NextPlayer = ((ActivePlayer + 1) % Players.Length);
            Players[NextPlayer].clip = Clip;
            Players[NextPlayer].Play();
            Faders[1] = this.FadeAudioSource(Players[NextPlayer], this.FadeDuration, Volume, () => { Faders[1] = null; });
            StartCoroutine(Faders[1]);

            ActivePlayer = NextPlayer;
        }
    }

    IEnumerator FadeAudioSource(AudioSource Player, float Duration, float TargetVolume, System.Action FinishedCallback)
    {
        int Steps = ((int)(VolumeChangesPerSecond * Duration));
        float StepTime = Duration / Steps;
        float StepSize = ((TargetVolume - Player.volume) / Steps);

        for (int I = 1; I < Steps; I++)
        {
            Player.volume += StepSize;
            yield return new WaitForSeconds(StepTime);
        }

        Player.volume = TargetVolume;

        if (FinishedCallback != null)
        {
            FinishedCallback();
        }
    }

    private void Awake()
    {
        if (Instance != this && Instance != null) Destroy(this);
        Instance = this;

        Players = new AudioSource[2] {
            this.gameObject.AddComponent<AudioSource>(),
            this.gameObject.AddComponent<AudioSource>()
        };

        foreach (AudioSource Source in Players)
        {
            Source.loop = true;
            Source.playOnAwake = false;
            Source.volume = 0.0f;
        }
    }
}
