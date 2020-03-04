using UnityEngine;
using UnityEditor;

sealed public class AssetBundleExporter : Editor
{
    [MenuItem("EmoteRain/BuildBundles")]
    static void BuildAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(Application.dataPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
    }
    [MenuItem("EmoteRain/ClearFlags")]
    static void ClearFlags()
    {
        foreach (string name in AssetDatabase.GetAllAssetBundleNames())
        {
            AssetDatabase.RemoveAssetBundleName(name, true);
        }
    }
}