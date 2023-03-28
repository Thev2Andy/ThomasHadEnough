using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioFunctions : MonoBehaviour
{
    [Header("Button References")]
    public TMP_Text VolumeButton;
    public TMP_Text MuteButton;


    public void ChangeVolume()
    {
        AudioPresets.Volume[] AudioVolumePresets = ((AudioPresets.Volume[])System.Enum.GetValues(typeof(AudioPresets.Volume)));

        float CurrentVolumePreset = float.Parse((Settings.Get("Master Volume", 1f).ToString()));
        float SmallestDifference = float.PositiveInfinity;
        int IndexOfClosest = -1;

        for (int I = 0; I < AudioVolumePresets.Length; I++)
        {
            float Difference = Mathf.Abs((CurrentVolumePreset - AudioPresets.RetrieveValue(AudioVolumePresets[I])));
            if (Difference < SmallestDifference)
            {
                SmallestDifference = Difference;
                IndexOfClosest = I;
            }
        }


        float NextVolumePreset = AudioPresets.RetrieveValue(AudioVolumePresets[((IndexOfClosest != (AudioVolumePresets.Length - 1)) ? (IndexOfClosest + 1) : 0)]);
        Settings.Set("Master Volume", NextVolumePreset);
    }

    public void ToggleMute()
    {
        bool MuteAudio = System.Convert.ToBoolean((Settings.Get("Mute Audio", false).ToString()));
        Settings.Set("Mute Audio", !MuteAudio);
    }


    public void RefreshUI()
    {
        float CurrentVolume = float.Parse((Settings.Get("Master Volume", 1f).ToString()));
        VolumeButton.text = $"Volume: {(CurrentVolume * 100f)}%";

        bool MuteAudio = System.Convert.ToBoolean((Settings.Get("Mute Audio", false).ToString()));
        MuteButton.text = $"Mute: {((MuteAudio) ? "On" : "Off")}";
    }


    private void Apply() {
        float CurrentVolume = float.Parse((Settings.Get("Master Volume", 1f).ToString()));
        bool MuteAudio = System.Convert.ToBoolean((Settings.Get("Mute Audio", false).ToString()));
        AudioListener.volume = ((!MuteAudio) ? CurrentVolume : 0f);
    }

    private void Awake() {
        Settings.SettingsChanged += RefreshUI;
        Settings.SettingsChanged += Apply;

        this.RefreshUI();
        this.Apply();
    }

    private void OnDestroy() {
        Settings.SettingsChanged -= RefreshUI;
        Settings.SettingsChanged -= Apply;
    }
}
