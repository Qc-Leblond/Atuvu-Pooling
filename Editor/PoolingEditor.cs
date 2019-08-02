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
                var assetsGUID = AssetDatabase.FindAssets("t:PoolManagerSettings");
                PoolManagerSettings settings = null;
                foreach (var assetPath in assetsGUID) 
                {
                    settings = AssetDatabase.LoadAssetAtPath<PoolManagerSettings>(AssetDatabase.GUIDToAssetPath(assetPath));
                    if (settings != null) 
                        break;
                }

                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<PoolManagerSettings>();
                    AssetDatabase.CreateAsset(settings, "Assets/PoolingSettings.asset");

                }

                var preLoadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
                foreach (var preLoadedAsset in preLoadedAssets)
                {
                    if (preLoadedAsset is PoolManagerSettings)
                        return;
                }

                if (settings != null)
                {
                    preLoadedAssets.Add(settings);
                    PlayerSettings.SetPreloadedAssets(preLoadedAssets.ToArray());
                    AssetDatabase.SaveAssets();
                }  
            }
        }
    }
}