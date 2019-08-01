using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Atuvu.Pooling
{
    public static class PoolManager
    {
        [RuntimeInitializeOnLoadMethod]
        static void AutoInitialize()
        {
            EnsureInitialize();
        }

        public static void Initialize(Transform targetRoot)
        {
            if (s_Instance == null)
            {
                s_Instance = CreateInstance(false);
                s_Instance.transform.SetParent(targetRoot, true);
            }
        }

        public static void Initialize()
        {
            if (s_Instance == null)
            {
                s_Instance = CreateInstance(true);
            }
        }

        static PoolManagerObject CreateInstance(bool dontDestroyOnLoad)
        {
            var obj = new GameObject("[Pool Manager]");
            var instance = obj.AddComponent<PoolManagerObject>();

            obj.transform.position = settings.poolsPosition;
            if (dontDestroyOnLoad) Object.DontDestroyOnLoad(obj);
            return instance;
        }
        
        static PoolManagerObject s_Instance;

        public static IPoolManagerSettings settings
        {
            get { return PoolManagerSettings.Get(); }
        }

        internal static Transform CreatePoolRoot(string name)
        {
            EnsureInitialize();
            Transform root = new GameObject($"[Pool: {name}]").transform;
            root.parent = s_Instance.transform;
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;
            return root;
        }

        static void EnsureInitialize()
        {
            if (settings.autoInitializePoolsManager)
                Initialize();
            else
                throw new Exception("Trying to access a pool or the pool manager without having the Pool Manager Initialized");
        }
    }
}
