using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleableObject : MonoBehaviour
{
    public string SettingToDetect;
    public bool Default;

    [Space]

    public GameObject ObjectToToggle;


    public void Toggle() {
        ObjectToToggle.SetActive(System.Convert.ToBoolean(Settings.Get(SettingToDetect, Default)));
    }


    private void Awake() {
        Settings.SettingsChanged += Toggle;
        this.Toggle();
    }

    private void OnDestroy() {
        Settings.SettingsChanged -= Toggle;
    }
}
