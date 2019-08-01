using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Atuvu.Pooling
{
    static class PoolingEditor
    {
        [InitializeOnLoadMethod]
        static void EnsurePoolSettingsExist()
        {
            if (!PoolManagerSettings.settingsFileExists)
            {
                var assets = AssetDatabase.FindAssets("t:PoolManagerSettings");
                PoolManagerSettings settings = null;
                foreach (var assetPath in assets)
                {
                    settings = AssetDatabase.LoadAssetAtPath<PoolManagerSettings>(assetPath);
                    if (settings != null)
                        break;
                }

                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<PoolManagerSettings>();
                    AssetDatabase.CreateAsset(settings, "Assets/PoolingSettings.asset");

                }

                var preLoadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
                preLoadedAssets.Add(settings);
                PlayerSettings.SetPreloadedAssets(preLoadedAssets.ToArray());
            }
        }
    }
}