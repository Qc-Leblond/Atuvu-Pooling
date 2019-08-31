using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine.Events;
using UnityEngine.Networking.PlayerConnection;

namespace Atuvu.Pooling
{
    internal struct EventId
    {
        static int m_CurrentId = 0;

        readonly int m_Id;

        EventId(int id)
        {
            m_Id = id;
        }

        public static EventId GetUnique()
        {
            var eventId = new EventId(++m_CurrentId);
            return eventId;
        }

        public override int GetHashCode()
        {
            return m_Id.GetHashCode();
        }
    }

    internal sealed class PoolProfilerDriver : ScriptableSingleton<PoolProfilerDriver>
    {
        const string k_ActivePlayerPref = "Atuvu.Pooling.ActivePlayer";

        static readonly Dictionary<Type, Guid> s_EventToId = new Dictionary<Type, Guid>();
        static readonly Dictionary<EventId, UnityAction<MessageEventArgs>> s_Callbacks = new Dictionary<EventId, UnityAction<MessageEventArgs>>();

        readonly List<ConnectedPlayer> m_ConnectedDevices = new List<ConnectedPlayer>();
        ConnectedPlayer m_ActiveDevice;

        void OnEnable()
        {
            m_ConnectedDevices.Clear();
            m_ConnectedDevices.AddRange(EditorConnection.instance.ConnectedPlayers);
            EditorConnection.instance.RegisterConnection(OnDeviceConnected);
            EditorConnection.instance.RegisterDisconnection(OnDeviceDisconnected);
        }

        void OnDeviceConnected(int id)
        {
            m_ConnectedDevices.Add(new ConnectedPlayer(id));
        }

        void OnDeviceDisconnected(int id)
        {
            for (int i = 0; i < m_ConnectedDevices.Count; ++i)
            {
                ConnectedPlayer device = m_ConnectedDevices[i];
                if (device.playerId == id)
                {
                    if (m_ActiveDevice != null && m_ActiveDevice.playerId == id)
                        m_ActiveDevice = null;

                    m_ConnectedDevices.RemoveAt(i);
                    break;
                }
            }
        }

        public static IReadOnlyList<ConnectedPlayer> connectedPlayers => instance.m_ConnectedDevices;

        public static ConnectedPlayer activePlayer
        {
            get => instance.m_ActiveDevice;
            set
            {
                foreach (var device in instance.m_ConnectedDevices)
                {
                    if (device.playerId == value.playerId)
                    {
                        instance.m_ActiveDevice = device;
                        return;
                    }
                }

                //If couldn't find a device with id this is an outdated info, put null instead
                instance.m_ActiveDevice = null;
            } 
        }

        public static void RequestPoolList()
        {
            EditorConnection.instance.Send(PoolProfiler.poolListRequested, null);
        }

        public static EventId RegisterEvent<T>(Action<T> callback) where T : struct, IPoolProfilerEvent<T>
        {
            return instance.RegisterEvent_Impl(callback);
        }

        EventId RegisterEvent_Impl<T>(Action<T> callback) where T : struct, IPoolProfilerEvent<T>
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            UnityAction<MessageEventArgs> callbackWrapper = (args) =>
            {
                if (m_ActiveDevice == null || m_ActiveDevice.playerId != args.playerId)
                    return;

                callback(new T().FromByte(args.data));
            };

            var id = EventId.GetUnique();
            s_Callbacks.Add(id, callbackWrapper);
            EditorConnection.instance.Register(GetEventId<T>(), callbackWrapper);

            return id;
        }

        public static void UnregisterEvent<T>(EventId id) where T : struct, IPoolProfilerEvent<T>
        {
            //TODO not safe to only check if id is valid and not if id is related to type
            if (s_Callbacks.TryGetValue(id, out UnityAction<MessageEventArgs> callback))
            {
                s_Callbacks.Remove(id);
                EditorConnection.instance.Unregister(GetEventId<T>(), callback);
            }
        }

        static Guid GetEventId<T>() where T : struct, IPoolProfilerEvent<T>
        {
            var type = typeof(T);
            if (s_EventToId.TryGetValue(type, out Guid guid))
            {
                return guid;
            }

            guid = new T().GetEventId();
            s_EventToId.Add(type, guid);
            return guid;
        }
    }
}