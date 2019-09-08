using System;
using System.Collections.Generic;

namespace Atuvu.Pooling
{
    sealed class ProfilingPoolData
    {
        public Guid id { get; }
        public string name;
        public int initialSize;
        public int currentSize;

        public ProfilingPoolData(Guid id, string name, int initSize)
        {
            this.id = id;
            this.name = name;
            initialSize = initSize;
            currentSize = initialSize;
        }
    }

    abstract class PlayerBridgeBase
    {
        readonly List<ProfilingPoolData> m_Pools = new List<ProfilingPoolData>();

        public IReadOnlyList<ProfilingPoolData> pools => m_Pools;

        void AddPool(PoolEventData pool)
        {
            var result = GetPool(pool.id);
            if (result != null)
            {
                result.name = pool.name;
                result.currentSize = pool.currentSize;
                return;
            }

            m_Pools.Add(new ProfilingPoolData(pool.id, pool.name, pool.initialSize));
        }

        protected void PoolListReceived(PoolListReturnEvent evt)
        {
            foreach (var pool in evt.pools)
            {
                AddPool(pool);
            }
        }

        protected void PoolInitializedReceived(PoolInitializedEvent evt)
        {
            AddPool(evt.pool);
        }

        protected void PoolResizeReceived(PoolResizeEvent evt)
        {
            var result = GetPool(evt.poolId);
            if (result != null)
                result.currentSize = evt.newSize;
        }

        public ProfilingPoolData GetPool(Guid id)
        {
            foreach (var pool in m_Pools)
            {
                if (pool.id == id)
                    return pool;
            }

            return null;
        }

        public void Initialize()
        {
            OnInitialize();

            RequestPoolList();
        }

        public void Destroy()
        {
            OnDestroy();
        }

        protected abstract void RequestPoolList();
        protected abstract void OnInitialize();
        protected abstract void OnDestroy();
    }

    struct PoolProfilerDevice
    {
        public const int noneId = -1;
        public const int editorId = 0;

        public static readonly PoolProfilerDevice none = new PoolProfilerDevice(noneId, "<none>");

        public int id { get; }
        public string name { get; }

        public PoolProfilerDevice(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    static class EditorPoolProfilerManager
    {
        sealed class NoneBridge : PlayerBridgeBase
        {
            protected override void RequestPoolList() {}
            protected override void OnInitialize() { }
            protected override void OnDestroy() { }
        }

        static readonly List<PoolProfilerDevice> s_Devices = new List<PoolProfilerDevice>();

        static EditorPoolProfilerManager()
        {
            s_Devices.Add(new PoolProfilerDevice(PoolProfilerDevice.editorId, "Editor"));
        }

        public static IReadOnlyList<PoolProfilerDevice> devices => s_Devices;
        public static PoolProfilerDevice activeDevice { get; private set; } = PoolProfilerDevice.none;
        public static IReadOnlyList<ProfilingPoolData> pools => s_PlayerBridge.pools;

        static PlayerBridgeBase s_PlayerBridge = new NoneBridge();

        public static void SetActiveDevice(PoolProfilerDevice device)
        {
            if (activeDevice.id == device.id)
                return;

            s_PlayerBridge.Destroy();
            activeDevice = device;
            switch (device.id)
            {
                case PoolProfilerDevice.noneId:
                    s_PlayerBridge = new NoneBridge();
                    break;

                case PoolProfilerDevice.editorId:
                    s_PlayerBridge = new EditorPlayerBridge();
                    break;

                default:
                    //TODO replace with proper bridge for builds
                    s_PlayerBridge = new NoneBridge();
                    break;
            }
            s_PlayerBridge.Initialize();
        }
    }
}