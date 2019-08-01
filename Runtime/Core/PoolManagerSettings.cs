using System.Collections.Generic;
using UnityEngine;

namespace Atuvu.Pooling
{
    public interface IPoolManagerSettings
    {
        bool autoInitializePoolsManager { get; }
        Vector3 poolsPosition { get; }
        bool disableObjectInPool { get; }
        ScaleResetMode defaultScaleResetMode { get; }
        IReadOnlyList<Pool> preloadedPool { get; }
    }

    sealed class PoolManagerSettings : ScriptableObject, IPoolManagerSettings
    {
        sealed class DefaultSettings : IPoolManagerSettings
        {
            static readonly Pool[] k_EmptyPool = new Pool[0];

            public bool autoInitializePoolsManager => false;
            public Vector3 poolsPosition => new Vector3(10000, 1000, 1000);
            public bool disableObjectInPool => true;
            public ScaleResetMode defaultScaleResetMode => ScaleResetMode.Disabled;
            public IReadOnlyList<Pool> preloadedPool => k_EmptyPool;
        }

        public static IPoolManagerSettings Get()
        {
            if (s_Instance == null)
                return k_DefaultSettings;

            return s_Instance;
        }
        
        static readonly DefaultSettings k_DefaultSettings = new DefaultSettings();
        static PoolManagerSettings s_Instance;

        [SerializeField] bool m_AutoInitializedPoolsManager = k_DefaultSettings.autoInitializePoolsManager;
        [SerializeField] Vector3 m_PoolsPosition = k_DefaultSettings.poolsPosition;
        [SerializeField] bool m_DisableObjectInPool = k_DefaultSettings.disableObjectInPool;
        [SerializeField] ScaleResetMode m_DefaultScaleResetMode = k_DefaultSettings.defaultScaleResetMode;
        [SerializeField] Pool[] m_PreLoadedPool = new Pool[0];

        public bool autoInitializePoolsManager { get { return m_AutoInitializedPoolsManager; } }
        public Vector3 poolsPosition { get { return m_PoolsPosition; } }
        public bool disableObjectInPool { get { return m_DisableObjectInPool; } }
        public ScaleResetMode defaultScaleResetMode { get { return m_DefaultScaleResetMode; } }
        public IReadOnlyList<Pool> preloadedPool { get { return m_PreLoadedPool; } }
        public static bool settingsFileExists { get { return s_Instance != null; } }


        void OnEnable()
        {
            if (s_Instance == null)
                s_Instance = this;
        }
    }
}