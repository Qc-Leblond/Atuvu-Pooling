using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Atuvu.Pooling
{
    public static class PoolManager
    {
        static readonly List<Pool> s_Pools = new List<Pool>();

        [RuntimeInitializeOnLoadMethod]
        static void AutoInitialize()
        {
            if (settings.autoInitializePoolsManager)
                Initialize();
        }

        internal static IReadOnlyList<Pool> poolsInternal => s_Pools;

        public static Pool[] GetPools()
        {
            return s_Pools.ToArray();
        }

        public static void GetPools(List<Pool> results)
        {
            foreach (var pool in s_Pools)
            {
                results.Add(pool);
            }
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

        internal static void RegisterPool(Pool pool)
        {
            s_Pools.Add(pool);
        }

        internal static void UnregisterPool(Pool pool)
        {
            s_Pools.Remove(pool);
        }

        static void EnsureInitialize()
        {
            if (s_Instance != null)
                return;

            if (settings.autoInitializePoolsManager)
                Initialize();
            else
                throw new Exception("Trying to access a pool or the pool manager without having the Pool Manager Initialized");
        }
    }
}
