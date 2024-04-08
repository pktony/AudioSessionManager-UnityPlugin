- [Overview](#overview)
- [Does Unity Support Swift ?](#does-unity-support-swift-)
- [Why Swift ?](#why-swift-)
- [Implementation](#implementation)
  * [Swift Project](#swift-project)
  * [Bridge](#bridge)
  * [Unity Integration](#unity-integration)
  * [Build Automation](#build-automation)
- [Review](#review)


## Overview

This document provides an in-depth guide on how to create an iOS native plugin using Swift for Unity. It covers why one should consider Swift over Object-C, how to implement it, and how to integrate it into Unity. It goes further to explain how to automate the build process. The guide includes code examples for each step of the process, making it a valuable resource for developers exploring this area.

I am not an expert neither in Swift and Object-C. As a client programmer using Unity, I’m offerring a way to implement native plugin using Swift.

## Does Unity Support Swift ?

The answer is no. Unity does not officially support Native plugin made with swift. So, we need a bridge between Swift and Unity. Fortunately, in a single framework project on Xcode, Swift and Object C can communicate one another. Using Object-C, which Unity supports as a native plugin, we should 

## Why Swift ?

Apple seems to be moving from Object-C to Swift. However, there are lots of outdated information on the internet. Although Apple offers both Object-C and Swift official documents, I think it’s worth thinking about implementing native plugin using Swift.

For those who are familiar with Object-C, and have no reason to use Swift, it is time saving to choose Object-C. It is a much smarter choice. But people who are new to making native plugins, I think it is better to start off with Swift.

## Implementation

### Swift Project

Make a iOS Framework project. Make sure to choose for iOS platform.

![image](https://github.com/pktony/AudioSessionManager-UnityPlugin/assets/66198932/fd3a3189-bf03-4c46-8e7d-bbfe919eb6ca)

### Bridge

![image](https://github.com/pktony/AudioSessionManager-UnityPlugin/assets/66198932/b6f85ee5-b6e6-4f4a-8b8c-580eb461b27d)


Create 3 files :

1. **Header file**
    1. Usually you don’t have to worry about header file. It will be written automatically.
2. **Swift file**
    1. Swift file contains the main logic. Implement as your needs.
    2. Put @objc attribute in front of the function. It makes the code available to Object-C.

Here’s an example of a plugin that handles part of the AVAudioSession.

```swift
@objc public class AudioManager : NSObject{
    
    @objc public static let shared = AudioManager()
    
    @objc public func SetAudioCategory(category: String)
    {
        let audioSession = AVAudioSession.sharedInstance()
        do
        {
            let audioCategory = AVAudioSession.Category.fromString(category)
            if (audioCategory == nil)
            {
                print("AudioManager : Category not exist - " + category)
                return
            }
            
            try audioSession.setCategory(audioCategory!)
        } catch
        {
            print("AudioManager : Error setting audio category - " + category)
        }
    }
    
    @objc public func SetAudioMode(mode: String)
    {
        let audioSession = AVAudioSession.sharedInstance()
        do
        {
            let audioMode = AVAudioSession.Mode.fromString(mode)
            if (audioMode == nil)
            {
                print("AudioManager : Mode not exist - " + mode)
                return
            }
            
            try audioSession.setMode(audioMode!)
        } catch
        {
            print("AudioManager : Error setting audio mode - " + mode + "error" + error.localizedDescription)
        }
    }
    
    @objc public func TestParameterConnection(text: String)
    {
        print("Connection Success - " + text);
    }
    
    @objc public func GetCurrentAudioMode() -> String
    {
        let audioSession = AVAudioSession.sharedInstance()
        print("GetCurrentAudioMode - " + audioSession.mode.toString)
        return audioSession.mode.toString
    }
    
    @objc public func GetCurrentAudioCategory() -> String
    {
        let audioSession = AVAudioSession.sharedInstance()
        print("GetCurrentAudioCategory - " + audioSession.mode.toString)
        return audioSession.category.toString
    }
    
    @objc public func SetEnableHapticDuringRecording(isEnable: Bool)
    {
        let audioSession = AVAudioSession.sharedInstance()
        
        if #available(iOS 13.0, *) {
            do
            {
                try audioSession.setAllowHapticsAndSystemSoundsDuringRecording(isEnable)
            }
            catch{
                print("AudioManager: failed to SetEnableHapticDuringRecording");
            }
        }
    }
}
```

1. **Object-C file**
    1. This is bridge file that connects between Swift and Object-C.
    2. Change it’s extension to **.mm**
    3. .mm allows the file to use Object-C and C++, while .m allows the file to use Swift and Object-C. You need C++ code to use extern feature.
    4. Use #include "{framworkName}/{headerfileName}" to import the Swift file.
    
    ```objectivec
    #import <Foundation/Foundation.h>
    #include "AudioManager/AudioManager-Swift.h"
    
    extern "C"
    {
    
    #pragma mark - TEST FUNCTIONS
    void TestConnection(){
        NSLog(@"Connected");
    }
    
    void TestParameterConnection(const char *parameter) {
        NSString *parameterString = [NSString stringWithUTF8String:parameter];
        [[AudioManager shared] TestParameterConnectionWithText:parameterString];
    }
    
    #pragma mark - BridgeFunctions
    void SetAudioCategory(const char *category){
        NSString *parameterString = [NSString stringWithUTF8String:category];
        [[AudioManager shared] SetAudioCategoryWithCategory:parameterString];
    }
    
    void SetAudioMode(const char *mode){
        NSString *parameterString = [NSString stringWithUTF8String:mode];
        [[AudioManager shared] SetAudioModeWithMode:parameterString];
    }
    
    char * GetCurrentAudioMode(){
        NSString* audioMode = [[AudioManager shared] GetCurrentAudioMode];
        const char *mode = [audioMode UTF8String];
        return strdup(mode);
    }
    
    char * GetCurrentAudioCategory(){
        NSString* audioCategory = [[AudioManager shared] GetCurrentAudioCategory];
        const char *category = [audioCategory UTF8String];
        return strdup(category);
    }
    
    void SetEnableHapticDuringRecording(BOOL setEnable){
        [[AudioManager shared] SetEnableHapticDuringRecordingWithIsEnable:setEnable];
    }
    }
    ```
    

Here’s the tricky part. When you call Swift function in bridge file, usually the types are different. On the example above, GetCurrentAudioCategory returns String in Swift, but receives NSString*. However, as far as I know, because C# cannot recognize NSString, you have to convert NSString to char*. If you miscast a type, the Unity will **freeze or crash**. Be careful on the type casting. For me, I converted NSString by UTF8String encoding then returned it.

it is very confusing concept. I am still not 100% sure what NSString is, why it’s made in the first place. All I know is NSString is used in Object-C, where char * represents string in C.

### Unity Integration

Make a utility script that loads the object bridge file. This is just an example. If you have any better idea, I’m always open for the feedback.

```csharp
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
```

### Build Automation

It is quite annoying job to include framework every time you build an Unity app. We are lazy. Let’s make a build automation process to save time.

In my case, the framework was included and signed ready once I built the app. If so, skip the part where you add file to the xcode manually. (I think FRAMERWORK_SEARCH_PATHS property automatically finds all the framework from the directory written in the value). But for those who aren’t, make a PostProcessBuild script to manually include the plugin in Xcode project.

```csharp
public static class PostBuildScript
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget != BuildTarget.iOS) return;

        var projPath = PBXProject.GetPBXProjectPath(buildPath);
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);

        var targetGuid = proj.GetUnityMainTargetGuid();

        proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        
        proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRDIDGING_HEADER", "Libraries/Plugins/AudioManager-Bridging-Header.h");
        proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "AudioManager-Swift.h");

        proj.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks $(PROJECT_DIR)/lib/$(CONFIGURATION) $(inherited)");
        proj.AddBuildProperty(targetGuid, "FRAMERWORK_SEARCH_PATHS",
            "$(inherited) $(PROJECT_DIR) $(PROJECT_DIR)/Frameworks");
        proj.AddBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        proj.AddBuildProperty(targetGuid, "DYLIB_INSTALL_NAME_BASE", "@rpath");
        proj.AddBuildProperty(targetGuid, "LD_DYLIB_INSTALL_NAME",
            "@executable_path/../Frameworks/$(EXECUTABLE_PATH)");
        proj.AddBuildProperty(targetGuid, "DEFINES_MODULE", "YES");
        proj.AddBuildProperty(targetGuid, "SWIFT_VERSION", "4.0");
        proj.AddBuildProperty(targetGuid, "COREML_CODEGEN_LANGUAGE", "Swift");

        const string frameworkName = "AudioManager.framework";
        var frameworkPath = Path.Combine("Plugins", frameworkName);
        string fileGuid = proj.AddFile(frameworkPath, Path.Combine("Frameworks", frameworkPath), PBXSourceTree.Sdk);
        proj.AddFileToEmbedFrameworks(targetGuid, fileGuid);
        proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");

        proj.WriteToFile(projPath);
    }
}
```

The key part I think is this. You should explicitly indicate directory and name of the of your header.

```csharp
proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRDIDGING_HEADER", "Libraries/Plugins/AudioManager-Bridging-Header.h");
proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "AudioManager-Swift.h");
```

SWIFT_OBJC_BRDIDGING_HEADER is a Xcode build setting that allows Swift and Object-C to communicate each other. 

## Review

Writing this guide was a challenging but rewarding process. Usually, I made native plug in using Object-C, but information was getting outdated, people are starting to move from Object-C to Swift in terms of iOS developing. I thought this was a great chance to research about Swift. Information on integrating Swift with Unity is scarce and often outdated. However, after a lot of trial and error, I was able to create a bridge between Swift and Unity. I hope this guide will help other developers on the same journey and save them some time.
