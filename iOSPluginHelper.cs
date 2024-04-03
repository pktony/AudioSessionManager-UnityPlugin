using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

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

public class iOSPluginHelper : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SetAudioCategory(string category);
    [DllImport("__Internal")]
    private static extern void SetAudioMode(string mode);
    [DllImport("__Internal")]
    private static extern void TestConnection();
    [DllImport("__Internal")]
    private static extern void TestParameterConnection(string mode);


    public static iOSPluginHelper Instance ;

    private void Awake()
    {
        Instance = this;

        TestConnection();
        TestParameterConnection("Hello");
    }

    public void SetIOSAudioCategory(string category)
    {
        Utility.Log(this, "SetIOSAudioCategory");
        SetAudioCategory(category);
    }

    public void SetIOSAudioMode(string mode)
    {
        Utility.Log(this, "SetIOSAudioMode");
        SetAudioMode(mode);
    }
}
