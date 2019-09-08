using UnityEditor;

namespace Atuvu.Pooling
{
    sealed class PoolProfilerWindow : EditorWindow
    {
        [MenuItem("Window/Analysis/Pool Profiler")]
        static void OpenWindow()
        {
            GetWindow<PoolProfilerWindow>("Pool Profiler");
        }

        void OnEnable()
        {
            EditorPoolProfilerManager.SetActiveDevice(EditorPoolProfilerManager.devices[0]);
        }

        void OnDisable()
        {
            EditorPoolProfilerManager.SetActiveDevice(PoolProfilerDevice.none);
        }

        void OnGUI()
        {
            for (var i = 0; i < EditorPoolProfilerManager.pools.Count; ++i)
            {
                var pool = EditorPoolProfilerManager.pools[i];
                EditorGUILayout.LabelField($"{pool.name}| initial Size: {pool.initialSize} | current size: {pool.currentSize}");
            }
        }
    }
}
