using System;
using System.Text;
using UnityEngine;

namespace Atuvu.Pooling
{
    [Serializable]
    public struct PoolInitializedEvent : IPoolProfilerEvent<PoolInitializedEvent>
    {
        static readonly Guid k_EventId = new Guid("8ae71ff6-d900-4256-b229-e45f3f657840");

        [SerializeField] string m_PoolName;
        [SerializeField] Guid m_PoolId;
        [SerializeField] int _mPoolSize;

        public string poolName => m_PoolName;
        public Guid poolId => m_PoolId;
        public int poolSize => _mPoolSize;

        public PoolInitializedEvent(string poolName, Guid poolId, int poolSize)
        {
            m_PoolName = poolName;
            m_PoolId = poolId;
            _mPoolSize = poolSize;
        }

        public byte[] ToByte()
        {
            return Encoding.ASCII.GetBytes(JsonUtility.ToJson(this));
        }

        public PoolInitializedEvent FromByte(byte[] raw)
        {
            return JsonUtility.FromJson<PoolInitializedEvent>(Encoding.ASCII.GetString(raw));
        }

        public Guid GetEventId()
        {
            return k_EventId;
        }
    }
}