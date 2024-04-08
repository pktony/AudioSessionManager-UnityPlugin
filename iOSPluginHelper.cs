using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public enum AudioSessionCategory : int
{
    ambient,
    multiRoute,
    playAndRecord,
    playback,
    record,
    soloAmbient,
}

public enum AudioSessionMode : int
{
    Default,
    gameChat,
    measurement,
    moviePlayback,
    spokenAudio,
    videoChat,
    videoRecording,
    voiceChat,
    voicePrompt
}

public class iOSAudioManager
{
    #region Connection Test Functions
    [DllImport("__Internal")]
    private static extern void TestConnection();
    [DllImport("__Internal")]
    private static extern void TestParameterConnection(string mode);
    #endregion

    [DllImport("__Internal")]
    private static extern void SetAudioCategory(string category);
    [DllImport("__Internal")]
    private static extern void SetAudioMode(string mode);

    [DllImport("__Internal")]
    private static extern string GetCurrentAudioMode();
    [DllImport("__Internal")]
    private static extern string GetCurrentAudioCategory();
    [DllImport("__Internal")]
    private static extern void SetEnableHapticDuringRecording(bool isEnable);

    public static void SetIOSAudioMode(AudioSessionMode mode)
    {
        Debug.Log($"iOSAudioManager : SetIOSAudioMode - {mode.ToString()}");
        SetAudioMode(mode.ToString());
    }

    public static void SetIOSAudioCategory(AudioSessionCategory category)
    {
        Debug.Log($"iOSAudioManager : SetIOSAudioCategory - {category.ToString()}");
        SetAudioCategory(category.ToString());
    }

    public static AudioSessionMode GetCurrentIOSAudioMode()
    {
        string currentMode = GetCurrentAudioMode();

        var result = Enum.TryParse(typeof(AudioSessionMode), currentMode, out var mode);
        if (!result)
        {
            Debug.LogWarning($"iOSAudioManager : GetCurrentAudioMode - {currentMode} is not a valid AudioSessionMode");
            return AudioSessionMode.Default;
        }

        return (AudioSessionMode)mode;
    }

    public static AudioSessionCategory GetCurrentIOSAudioCategory()
    {
        string currentCategory = GetCurrentAudioCategory();

        var result = Enum.TryParse(typeof(AudioSessionCategory), currentCategory, out var category);
        if (!result)
        {
            Debug.LogWarning($"iOSAudioManager : GetCurrentAudioCategory - {currentCategory} is not a valid AudioSessionCategory");
            return AudioSessionCategory.soloAmbient;
        }

        return (AudioSessionCategory)category;
    }

    public static void SetEnableIOSHapticDuringRecording(bool isEnable)
    {
        SetEnableHapticDuringRecording(isEnable);
    }
}
