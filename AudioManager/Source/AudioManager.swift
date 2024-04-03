//
//  AudioManager.swift
//  AudioManager
//
//  Created by illuni_mac_005 on 4/2/24.
//

import Foundation
import AVFoundation

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
    
    @objc public func GetAvailableModes() -> [String]
    {
        let audioSession = AVAudioSession.sharedInstance()
        
        let availableModes = audioSession.availableModes.map {
                mode in
            return mode.toString
        }
        
        return availableModes
    }
    
    @objc public func GetAvailableCategories() -> [String]
    {
        let audioSession = AVAudioSession.sharedInstance()
        
        let availableCategories = audioSession.availableCategories.map {
                category in
            return category.toString
        }
        
        return availableCategories
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

extension AVAudioSession.Category
{
    static let stringToCategory: [String: AVAudioSession.Category] = [
        "ambient": .ambient,
        "multiRoute": .multiRoute,
        "playAndRecord": .playAndRecord,
        "playback": .playback,
        "record": .record,
        "soloAmbient": .soloAmbient
    ]
    
    var toString: String{
        switch self {
        case .ambient: return "ambient"
        case .multiRoute: return "multiRoute"
        case .playAndRecord: return "playAndRecord"
        case .record: return "record"
        case .soloAmbient: return "soloAmbient"
        default: return "Unknown"
        }
    }
    
    static func fromString(_ string: String) -> AVAudioSession.Category?{
        return stringToCategory[string]
    }
}

extension AVAudioSession.Mode
{
    static let stringToMode: [String: AVAudioSession.Mode] = [
        "default": .default,
        "gameChat": .gameChat,
        "measurement": .measurement,
        "moviePlayback": .moviePlayback,
        "spokenAudio": .spokenAudio,
        "videoChat": .videoChat,
        "videoRecording": .videoRecording,
        "voiceChat": .voiceChat,
        "voicePrompt": .voicePrompt
    ]
    
    var toString: String{
        switch self{
        case .default: return "default"
        case .gameChat: return "gameChat"
        case .measurement: return "measurement"
        case .moviePlayback: return "moviePlayback"
        case .spokenAudio: return "spokenAudio"
        case .videoChat: return "videoChat"
        case .videoRecording: return "videoRecording"
        case .voiceChat: return "voiceChat"
        case .voicePrompt: return "voicePrompt"
        default: return "Unknown"
        }
    }
    
    static func fromString(_ string: String) -> AVAudioSession.Mode?{
        return stringToMode[string]
    }
}
