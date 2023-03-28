using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayFunctions : MonoBehaviour
{
    [Header("Button References")]
    public TMP_Text ScreenshakeButton;
    public TMP_Text DifficultyButton;
    public TMP_Text OverheadArrowButton;


    public void ChangeScreenshake()
    {
        ScreenshakePresets.Intensity[] ScreenShakePresets = ((ScreenshakePresets.Intensity[])System.Enum.GetValues(typeof(ScreenshakePresets.Intensity)));

        float CurrentScreenshakePreset = float.Parse((Settings.Get("Screenshake Intensity", 1f).ToString()));
        float SmallestDifference = float.PositiveInfinity;
        int IndexOfClosest = -1;

        for (int I = 0; I < ScreenShakePresets.Length; I++)
        {
            float Difference = Mathf.Abs((CurrentScreenshakePreset - ScreenshakePresets.RetrieveValue(ScreenShakePresets[I])));
            if (Difference < SmallestDifference)
            {
                SmallestDifference = Difference;
                IndexOfClosest = I;
            }
        }


        float NextScreenshakePreset = ScreenshakePresets.RetrieveValue(ScreenShakePresets[((IndexOfClosest != (ScreenShakePresets.Length - 1)) ? (IndexOfClosest + 1) : 0)]);
        Settings.Set("Screenshake Intensity", NextScreenshakePreset);
    }

    public void ChangeDifficulty()
    {
        DifficultyPresets.Difficulty[] Difficulties = ((DifficultyPresets.Difficulty[])System.Enum.GetValues(typeof(DifficultyPresets.Difficulty)));

        int CurrentDifficultyIndex = int.Parse((Settings.Get("Difficulty", 1).ToString()));
        int NextDifficultyIndex = (((CurrentDifficultyIndex + 1) < Difficulties.Length && (CurrentDifficultyIndex + 1) >= 0) ? (CurrentDifficultyIndex + 1) : 0);

        Settings.Set("Difficulty", NextDifficultyIndex);
    }

    public void ToggleOverheadArrow()
    {
        bool EnableOverheadArrow = System.Convert.ToBoolean((Settings.Get("Enable Overhead Arrow", true).ToString()));
        Settings.Set("Enable Overhead Arrow", !EnableOverheadArrow);
    }


    public void RefreshUI()
    {
        float CurrentCameraShakePreset = float.Parse((Settings.Get("Screenshake Intensity", 1f).ToString()));
        ScreenshakeButton.text = $"Screenshake Intensity: {(CurrentCameraShakePreset * 100f)}%";

        int CurrentDifficultyIndex = int.Parse((Settings.Get("Difficulty", 1).ToString()));
        DifficultyButton.text = $"Difficulty: {DifficultyPresets.RetrieveIdentifier(((DifficultyPresets.Difficulty)CurrentDifficultyIndex))}";

        bool EnableOverheadArrow = System.Convert.ToBoolean((Settings.Get("Enable Overhead Arrow", true).ToString()));
        OverheadArrowButton.text = $"Overhead Arrow: {((EnableOverheadArrow) ? "On" : "Off")}";
    }


    private void Awake() {
        Settings.SettingsChanged += RefreshUI;
        this.RefreshUI();
    }

    private void OnDestroy() {
        Settings.SettingsChanged -= RefreshUI;
    }
}
