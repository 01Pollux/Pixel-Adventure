using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EventSystem
{
    [HideInInspector]
    public class EventMessenger<EventBaseClass, EventType> :
        MonoBehaviour
        where EventBaseClass : MonoBehaviour
        where EventType : Enum
    {
        protected Dictionary<EventType, Action<object, EventArgs>> m_EventTable = new();

        private static Lazy<EventMessenger<EventBaseClass, EventType>> Instance = new(CreateInstance);

        private static EventMessenger<EventBaseClass, EventType> CreateInstance()
        {
            var obj = FindObjectOfType<EventMessenger<EventBaseClass, EventType>>();
            if (!obj)
            {
                GameObject tmp = new GameObject(typeof(EventBaseClass).Name);
                obj = tmp.AddComponent<EventBaseClass>() as EventMessenger<EventBaseClass, EventType>;
                DontDestroyOnLoad(tmp);
            }
            return obj;
        }


        public static void AddListener(EventType id, Action<object, EventArgs> callback)
        {
            var messenger = Instance.Value;
            if (!messenger)
                return;

            if (!messenger.m_EventTable.ContainsKey(id))
                messenger.m_EventTable.Add(id, callback);
            else
                messenger.m_EventTable[id] += callback;
        }

        public static void RemoveListener(EventType id, Action<object, EventArgs> callback)
        {
            var messenger = Instance.Value;
            if (!messenger)
                return;

            if (messenger.m_EventTable.ContainsKey(id))
                messenger.m_EventTable[id] -= callback;
        }


        public static void Raise(EventType id, object sender, EventArgs args = null)
        {
            var messenger = Instance.Value;
            if (!messenger)
                return;

            var @event = messenger.m_EventTable[id];
            if (@event == null)
                return;

            @event.Invoke(sender, args);
        }

        public static void RaiseAsync(EventType id, object sender, EventArgs args = null)
        {
            Task.Run(() =>
            {
                Raise(id, sender, args);
            });
        }
    }
}