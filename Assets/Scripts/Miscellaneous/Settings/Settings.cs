using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerSR;

public static class Settings
{
    public static string EditorSettingsFileContent;

    // Properties..
    public static string SettingsFilePath {
        get {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.psr");
        }
    }



    public static void Set(string SettingID, System.Object SettingValue)
    { 
        string SettingsFileContent = ((!Application.isEditor) ? File.ReadAllText(SettingsFilePath) : EditorSettingsFileContent);
        SettingsFileContent = SettingsFileContent.Set(SettingID, SettingValue);


        if (!Application.isEditor) {
            File.WriteAllText(SettingsFilePath, SettingsFileContent);
        }

        else {
            EditorSettingsFileContent = SettingsFileContent;
        }
    }

    public static System.Object Get(string SettingID, System.Object Default = null)
    {
        string SettingsFileContent = ((!Application.isEditor) ? File.ReadAllText(SettingsFilePath) : EditorSettingsFileContent);
        System.Object RetrievedObject = SettingsFileContent.Get(SettingID);
        return ((RetrievedObject != null) ? RetrievedObject : Default);
    }

    public static void Delete(string SettingID)
    {
        string SettingsFileContent = ((!Application.isEditor) ? File.ReadAllText(SettingsFilePath) : EditorSettingsFileContent);
        SettingsFileContent = SettingsFileContent.Delete(SettingID);


        if (!Application.isEditor) {
            File.WriteAllText(SettingsFilePath, SettingsFileContent);
        }

        else {
            EditorSettingsFileContent = SettingsFileContent;
        }
    }
}
