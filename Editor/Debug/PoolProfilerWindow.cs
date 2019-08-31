using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;

namespace Atuvu.Pooling
{
    internal sealed class PoolProfilerWindow : EditorWindow
    {
        sealed class PoolData
        {
            public Guid id { get; }
            public string name;
            public int size;

            public PoolData(Guid id, string name, int size)
            {
                this.name = name;
                this.id = id;
                this.size = size;
            }
        }

        [MenuItem("Window/Analysis/Pools")]
        static void OpenWindow()
        {
            GetWindow<PoolProfilerWindow>();
        }
        
        EventId m_InitEventId;
        EventId m_ResizeEventId;
        EventId m_PoolListResultId;

        int m_LoadingPoolsRequest = 0;
        List<PoolData> m_Pools = new List<PoolData>();

        bool poolLoading => m_LoadingPoolsRequest > 0;

        void OnEnable()
        {
            m_InitEventId = PoolProfilerDriver.RegisterEvent<PoolInitializedEvent>(OnNewPoolInitialized);
            m_ResizeEventId = PoolProfilerDriver.RegisterEvent<PoolResizeEvent>(OnPoolResized);
            m_PoolListResultId = PoolProfilerDriver.RegisterEvent<PoolListRequestResult>(OnPoolListRequestCompleted);

            m_LoadingPoolsRequest = 0;
            if (PoolProfilerDriver.activePlayer == null)
            {
                SetActivePlayer(new ConnectedPlayer(ProfilerDriver.connectedProfiler));
            }
            else
            {
                LoadData();
            }
        }

        void OnDisable()
        {
            PoolProfilerDriver.UnregisterEvent<PoolInitializedEvent>(m_InitEventId);
            PoolProfilerDriver.UnregisterEvent<PoolResizeEvent>(m_ResizeEventId);
            PoolProfilerDriver.UnregisterEvent<PoolListRequestResult>(m_PoolListResultId);
        }

        void OnGUI()
        {
            //TODO test ui
            /**/

            if (poolLoading)
            {
                GUILayout.Label("-Pool Loading-");
                return;
            }

            foreach (var pool in m_Pools)
            {
                GUILayout.Label(pool.name + ": " + pool.size);
            }

            /**/
        }

        void OnPoolListRequestCompleted(PoolListRequestResult result)
        {
            m_Pools.Clear();
            --m_LoadingPoolsRequest;
            foreach (var data in result.data)
            {
                m_Pools.Add(new PoolData(data.id, data.name, data.size));
            }
        }

        void OnNewPoolInitialized(PoolInitializedEvent evt)
        {
            var pool = GetPoolData(evt.poolId);
            if (pool != null)
            {
                pool.name = evt.poolName;
            }
            else
            {
                m_Pools.Add(new PoolData(evt.poolId, evt.poolName, evt.poolSize));
            }
        }

        void OnPoolResized(PoolResizeEvent evt)
        {
            var pool = GetPoolData(evt.poolId);
            if (pool != null)
            {
                pool.size = evt.poolSize;
            }
        }

        PoolData GetPoolData(Guid id)
        {
            foreach (var pool in m_Pools)
            {
                if (pool.id == id)
                    return pool;
            }

            return null;
        }

        void SetActivePlayer(ConnectedPlayer player)
        {
            if (PoolProfilerDriver.activePlayer != null && player != null 
                && PoolProfilerDriver.activePlayer.playerId == player.playerId)
                return;

            PoolProfilerDriver.activePlayer = player;
            LoadData();
        }

        void LoadData()
        {
            m_Pools.Clear();

            if (PoolProfilerDriver.activePlayer != null)
            {
                --m_LoadingPoolsRequest;
                PoolProfilerDriver.RequestPoolList();
            }
        }
    }
}