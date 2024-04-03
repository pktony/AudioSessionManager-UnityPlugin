using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

public static class PostBuildScript
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if( buildTarget != BuildTarget.iOS) return;

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

        const string defaultLocationInProj = "Plugins";
        const string frameworkName = "AudioManager.framework";
        var frameworkPath = Path.Combine(defaultLocationInProj, frameworkName);
        Debug.Log($"FullPath : {frameworkPath}");
        string fileGuid = proj.AddFile(frameworkPath, Path.Combine("Frameworks", frameworkPath), PBXSourceTree.Sdk);
        Debug.Log($"File GUID : {fileGuid}");
        proj.AddFileToEmbedFrameworks(targetGuid, fileGuid);
        proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");

        // string fullPath = Path.Combine(buildPath, defaultLocationInProj, coreFrameworkName);
        // string fileGuid = proj.AddFile(fullPath, "Frameworks/" + fullPath, PBXSourceTree.Sdk);
        // proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
        // // proj.AddFileToBuild(targetGuid, fileGuid);
        // proj.AddFileToEmbedFrameworks(targetGuid, fileGuid);
        // // PBXProjectExtensions.AddFileToEmbedFrameworks(proj, targetGuid, fileGuid);
        // proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");

        proj.WriteToFile(projPath);
    }
}
