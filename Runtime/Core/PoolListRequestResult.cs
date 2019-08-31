using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Atuvu.Pooling
{
    [Serializable]
    struct PoolListRequestResult : IPoolProfilerEvent<PoolListRequestResult>
    {
        static readonly Guid k_EventGuid = new Guid("ea586808-e515-429d-8356-34b344a906bc");

        public List<PoolProfiler.PoolData> data;

        public byte[] ToByte()
        {
            return Encoding.ASCII.GetBytes(JsonUtility.ToJson(this));
        }

        public PoolListRequestResult FromByte(byte[] raw)
        {
            return JsonUtility.FromJson<PoolListRequestResult>(Encoding.ASCII.GetString(raw));
        }

        public Guid GetEventId()
        {
            return k_EventGuid;
        }
    }
}