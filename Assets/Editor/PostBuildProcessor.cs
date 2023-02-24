using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
public class PostBuildProcessor : MonoBehaviour
{
    [PostProcessBuildAttribute]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
#if UNITY_IOS
        if (target == BuildTarget.iOS)
        {
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            PlistElementDict rootDict = plist.root;
            rootDict.SetBoolean("AppsFlyerShouldSwizzle", true);
            File.WriteAllText(plistPath, plist.WriteToString());
            Debug.Log("Info.plist updated with AppsFlyerShouldSwizzle");
        }
        if (target != BuildTarget.iOS)
            return;
        var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        // Adds entitlement depending on the Unity version used
#if UNITY_2019_3_OR_NEWER
            var project = new PBXProject();
            project.ReadFromString(System.IO.File.ReadAllText(projectPath));
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
            manager.AddSignInWithAppleWithCompatibility(project.GetUnityFrameworkTargetGuid());
            manager.WriteToFile();
#endif
#endif
    }
#if UNITY_IOS

    [PostProcessBuild(int.MaxValue)]
    static void OnBuildDone(BuildTarget target, string pathToBuiltProject)
    {
        ModifyInfoPList(pathToBuiltProject);
    }

    static void ModifyInfoPList(string pathToBuiltProject)
    {
        string path = pathToBuiltProject + "/Info.plist";
        var plist = new PlistDocument();
        plist.ReadFromFile(path);
        var root = plist.root;
        CFBundleLocalizations(root);
        plist.WriteToFile(path);
    }

    static void CFBundleLocalizations(PlistElementDict root)
    {
        var rootDic = root.values;
        var localizations = new string[] { "en", "fr","de","ru","id","es","ja","ko","pt","zh","it","vi" };
        var array = root.CreateArray("CFBundleLocalizations");
        foreach (var localization in localizations) array.AddString(localization);
    }
#endif
}
