using UnityEngine;

namespace Atuvu.Pooling
{
    [AddComponentMenu("")]
    internal sealed class PoolManagerObject : MonoBehaviour
    {
        void Awake()
        {
            for (var i = 0; i < PoolManager.settings.preloadedPool.Count; ++i)
            {
                var pool = PoolManager.settings.preloadedPool[i];
                pool.Initialize();
            }
        }
    }
}
