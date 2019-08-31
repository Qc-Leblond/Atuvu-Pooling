using System;

namespace Atuvu.Pooling
{
    struct PoolResizeEvent : IPoolProfilerEvent<PoolResizeEvent>
    {
        static readonly Guid k_EventId = new Guid("1f06413b-50e8-4b3e-848e-35e094e8b4c0");
        
        public Guid poolId { get; }
        public int poolSize { get; }
        
        public PoolResizeEvent(Guid poolId, int poolSize)
        {
            this.poolId = poolId;
            this.poolSize = poolSize;
        }

        public byte[] ToByte()
        {
            return EventByteConverter.ToByte(this);
        }

        public PoolResizeEvent FromByte(byte[] raw)
        {
            return EventByteConverter.FromByte<PoolResizeEvent>(raw);
        }

        public Guid GetEventId()
        {
            return k_EventId;
        }
    }
}