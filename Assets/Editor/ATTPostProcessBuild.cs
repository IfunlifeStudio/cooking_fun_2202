using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
public class ATTPostProcessBuild
{
    const string k_TrackingDescription = "Your data will be used to provide you a better and personalized ad experience.";
    [PostProcessBuildAttribute(0)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToXcode) {
        #if UNITY_IOS
        if (buildTarget == BuildTarget.iOS) {
            AddPListValues(pathToXcode);
        }
        #endif
    }
    #if UNITY_IOS
    // Implement a function to read and write values to the plist file:
    static void AddPListValues(string pathToXcode) {
        // Retrieve the plist file from the Xcode project directory:
        string plistPath = pathToXcode + "/Info.plist";
        PlistDocument plistObj = new PlistDocument();


        // Read the values from the plist file:
        plistObj.ReadFromString(File.ReadAllText(plistPath));

        // Set values from the root object:
        PlistElementDict plistRoot = plistObj.root;

        // Set the description key-value in the plist:
        plistRoot.SetString("NSUserTrackingUsageDescription", k_TrackingDescription);

        // Save changes to the plist:
        File.WriteAllText(plistPath, plistObj.WriteToString());
    }
    #endif
}