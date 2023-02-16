using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerSR;

public static class Settings
{
    // Events..
    public static event Action SettingsChanged;

    // Properties..
    public static string SettingsFileContent
    {
        get {
            return PlayerPrefs.GetString("SettingsFileContent", String.Empty);
        }

        set {
            PlayerPrefs.SetString("SettingsFileContent", value);
        }
    }



    public static void Set(string SettingID, System.Object SettingValue)
    {
        Settings.SettingsFileContent = Settings.SettingsFileContent.Set(SettingID, SettingValue);
        SettingsChanged?.Invoke();
    }

    public static System.Object Get(string SettingID, System.Object Default = null)
    {
        System.Object RetrievedObject = Settings.SettingsFileContent.Get(SettingID);
        return ((RetrievedObject != null) ? RetrievedObject : Default);
    }

    public static void Delete(string SettingID)
    {
        Settings.SettingsFileContent = Settings.SettingsFileContent.Delete(SettingID);
        SettingsChanged?.Invoke();
    }
}
