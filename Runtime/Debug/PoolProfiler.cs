using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Atuvu.Pooling
{
    internal struct PoolEventData
    {
        public string name { get; }
        public Guid id { get; }
        public int initialSize { get; }
        public int currentSize { get; }

        public PoolEventData(Pool pool)
        {
            name = pool.name;
            id = pool.id;
            initialSize = pool.originalCapacity;
            currentSize = pool.capacity;
        }
    }

    internal struct PoolListReturnEvent
    {
        public PoolEventData[] pools { get; }

        public PoolListReturnEvent(IReadOnlyList<Pool> pools)
        {
            this.pools = new PoolEventData[pools.Count];
            for (int i = 0; i < pools.Count; ++i)
            {
                this.pools[i] = new PoolEventData(pools[i]);
            }
        }
    }

    internal struct PoolInitializedEvent
    {
        public PoolEventData pool { get; }

        public PoolInitializedEvent(Pool pool)
        {
            this.pool = new PoolEventData(pool);
        }
    }

    internal struct PoolResizeEvent
    {
        public Guid poolId { get; }
        public int newSize { get; }

        public PoolResizeEvent(Pool pool)
        {
            newSize = pool.capacity;
        }
    }

    internal static class PoolProfiler
    {
#if UNITY_EDITOR
        internal static event Action<PoolInitializedEvent> editorPoolInitialized;
        internal static event Action<PoolResizeEvent> editorPoolResized;
#endif

        [Conditional("ENABLE_PROFILER")]
        public static void RecordPoolInitialized(Pool pool)
        {
#if UNITY_EDITOR
            editorPoolInitialized?.Invoke(new PoolInitializedEvent(pool));
#endif
        }

        [Conditional("ENABLE_PROFILER")]
        public static void RecordPoolResize(Pool pool)
        {
#if UNITY_EDITOR
            editorPoolResized?.Invoke(new PoolResizeEvent(pool));
#endif
        }
    }
}