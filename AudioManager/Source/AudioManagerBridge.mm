//
//  AudioManagerBridge.m
//  AudioManager
//
//  Created by illuni_mac_005 on 4/2/24.
//

#import <Foundation/Foundation.h>
#include "AudioManager/AudioManager-Swift.h"
//#include "AudioManager/AudioManager.h"

extern "C"
{

#pragma mark - TEST FUNCTIONS
void TestConnection(){
    NSLog(@"Connected");
}

void TestParameterConnection(const char *parameter) {
    NSString *parameterString = [NSString stringWithUTF8String:parameter];
//    [[AudioManager shared] TestParameterConnection:parameterString];
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
