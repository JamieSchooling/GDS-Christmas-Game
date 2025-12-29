using UnityEditor;
using UnityEditor.Build.Reporting;

[System.Serializable]
public struct BuildInfo
{
    public string Version;
    public int BuildNumber;

    public static BuildInfo GetCurrent()
    {
        string path = UnityEngine.Application.dataPath + "/../ProjectSettings/BuildInfo.json";

        BuildInfo buildInfo = new();
        string buildInfoJSON;

        if (!System.IO.File.Exists(path))
        {
            buildInfo = new BuildInfo
            {
                Version = "0.1.0",
                BuildNumber = 0,
            };
            Update(buildInfo);

            return buildInfo;
        }

        buildInfoJSON = System.IO.File.ReadAllText(path);
        buildInfo = UnityEngine.JsonUtility.FromJson<BuildInfo>(buildInfoJSON);
        return buildInfo;
    }

    public static void Update(BuildInfo buildInfo)
    {
        string path = UnityEngine.Application.dataPath + "/../ProjectSettings/BuildInfo.json";

        string buildInfoJSON = UnityEngine.JsonUtility.ToJson(buildInfo, prettyPrint: true);
        System.IO.File.WriteAllText(path, buildInfoJSON);
    }
}

public static class BuildScripts
{
    // Helper to get all enabled scenes in Build Settings
    static string[] GetEnabledScenes()
    {
        // Get all enabled scenes from Build Settings
        var scenesInSettings = EditorBuildSettings.scenes;
        var enabledScenes = new System.Collections.Generic.List<string>();

        // Iterate through all scenes and add the enabled ones
        for (int i = 0; i < scenesInSettings.Length; i++)
        {
            if (scenesInSettings[i].enabled)
                enabledScenes.Add(scenesInSettings[i].path);
        }

        return enabledScenes.ToArray();
    }

    // General build method
    static void BuildForTarget(BuildTarget target, string outputPath, bool runBuild = true)
    {

        string[] scenes = GetEnabledScenes();

        BuildInfo buildInfo = BuildInfo.GetCurrent();
        buildInfo.BuildNumber++;
        PlayerSettings.bundleVersion = $"{buildInfo.Version}.{buildInfo.BuildNumber}";

        outputPath = $"{outputPath}/v{buildInfo.Version}/{PlayerSettings.productName}.exe";

        // Build player options
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = target,
            options = runBuild ? BuildOptions.AutoRunPlayer : BuildOptions.ShowBuiltPlayer
        };

        // Execute the build
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (!CheckBuildResult(report, outputPath))
        {
            buildInfo.BuildNumber--;
            PlayerSettings.bundleVersion = $"{buildInfo.Version}.{buildInfo.BuildNumber}";
        }

        BuildInfo.Update(buildInfo);
    }

    [MenuItem("Build/Windows/Build %#W")]
    public static void BuildWindows()
    {
        BuildForTarget(BuildTarget.StandaloneWindows64, "Build/Windows", false);
    }

    [MenuItem("Build/Windows/Build And Run %W")]
    public static void BuildAndRunWindows()
    {
        BuildForTarget(BuildTarget.StandaloneWindows64, "Build/Windows");
    }

    // Helper to validate and log the build result
    static bool CheckBuildResult(BuildReport report, string outputPath)
    {
        // Log the build summary
        var summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("Build succeeded at: " + outputPath + " (" + summary.totalSize + " bytes)");
            return true;
        }
        else
        {
            UnityEngine.Debug.LogException(new System.Exception("Build failed: " + report.SummarizeErrors()));
            return false;
        }
    }
}
