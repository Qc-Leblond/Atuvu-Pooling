using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Networking.PlayerConnection;

namespace Atuvu.Pooling
{
    internal interface IPoolProfilerEvent<out T> where T : struct, IPoolProfilerEvent<T>
    {
        byte[] ToByte();
        T FromByte(byte[] raw);
        Guid GetEventId();
    }

    internal static class PoolProfiler
    { 
        [Serializable]
        public struct PoolData
        {
            public string name;
            public Guid id;
            public int size;
        }

        internal static readonly Guid poolListRequested = new Guid("e9d7846c-285a-4a8b-b155-64a184a34a9d");
        static readonly List<PoolData> s_PoolListData = new List<PoolData>(32);

#if ENABLE_PROFILER
        static PoolProfiler()
        {
            PlayerConnection.instance.Register(poolListRequested, (args) =>
            {
                RecordEntirePoolList();
            });
        }
#endif

        [Conditional("ENABLE_PROFILER")]
        public static void RecordPoolInit(Pool pool)
        {
            RecordEvent(new PoolInitializedEvent(pool.name, pool.id, pool.capacity));
        }

        [Conditional("ENABLE_PROFILER")]
        public static void RecordPoolResize(Pool pool)
        {
            RecordEvent(new PoolResizeEvent(pool.id, pool.capacity));
        }

        [Conditional("ENABLE_PROFILER")]
        public static void RecordEntirePoolList()
        {
            s_PoolListData.Clear();
            IReadOnlyList<Pool> pools = PoolManager.allPools;
            for (int i = 0; i < pools.Count; ++i)
            {
                var pool = pools[i];
                s_PoolListData.Add(new PoolData
                {
                    id = pool.id,
                    size = pool.capacity,
                    name = pool.name,
                });
            }

            RecordEvent(new PoolListRequestResult { data = s_PoolListData });
        }

        [Conditional("ENABLE_PROFILER")]
        static void RecordEvent<T>(T evt) where T : struct, IPoolProfilerEvent<T>
        {
            PlayerConnection.instance.Send(evt.GetEventId(), evt.ToByte());
        }
    }
}