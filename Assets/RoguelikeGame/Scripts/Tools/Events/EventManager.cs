using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Phoenix.Tools
{
    public struct GameEvent
    {
        public string EventName;
        public GameEvent(string eventName)
        {
            EventName = eventName;
        }
        static GameEvent e;
        public static void Trigger(string newName)
        {
            e.EventName = newName;
            EventManager.TriggerEvent(e);
        }
    }

    [ExecuteAlways]
    public static class EventManager
    {
        private static Dictionary<Type, List<IEventListenerBase>> _subscribersList;

        static EventManager()
        {
            _subscribersList = new Dictionary<Type, List<IEventListenerBase>> ();
        }

        public static void AddListener<EventType>(IEventListener<EventType> listener) where EventType : struct
        {
            Type eventType = typeof(EventType);

            if (!_subscribersList.ContainsKey(eventType))
            {
                _subscribersList[eventType] = new List<IEventListenerBase> ();
            }
            if (!ExistingListeners(eventType, listener))
            {
                _subscribersList[eventType].Add(listener);
            }
        }

        /// <summary>
        /// Removes a listener from a given event.
        /// </summary>
        /// <typeparam name="EventType">The event type.</typeparam>
        /// <param name="listener">The listener to remove.</param>
        public static void RemoveListener<EventType>(IEventListener<EventType> listener) where EventType : struct
        {
            Type eventType = typeof (EventType);

            if (!_subscribersList.ContainsKey(eventType)) { return; }

            List<IEventListenerBase> subscriberList = _subscribersList[eventType];

            for (int i = 0; i < subscriberList.Count; i++)
            {
                if (subscriberList[i] == listener)
                {
                    subscriberList.Remove(subscriberList[i]);

                    if(subscriberList.Count == 0)
                    {
                        _subscribersList.Remove(eventType);
                    }

                    return;
                }
            }
        }

        public static void TriggerEvent<EventType>(EventType newEvent) where EventType : struct
        {
            List<IEventListenerBase> listeners;
            if (!_subscribersList.TryGetValue(typeof(EventType), out listeners)) { return; }

            for (int i = 0; i < listeners.Count; i++)
            {
                (listeners[i] as IEventListener<EventType>).OnEvent(newEvent);
            }
        }

        private static bool ExistingListeners(Type type, IEventListenerBase listener)
        {
            List<IEventListenerBase> listeners;

            if (!_subscribersList.TryGetValue(type, out listeners)) return false;

            bool existingListeners = false;

            for (int i = 0; i < listeners.Count; i++)
            {
                if (listeners[i] == listener)
                {
                    existingListeners = true;
                    break;
                }
            }
            return existingListeners;
        }

    }

    public static class EventRegister
    {
        public delegate void Delegate<T>(T eventType);

        public static void EventListeningStart<EventType>(this IEventListener<EventType> caller) where EventType : struct
        {
            EventManager.AddListener<EventType>(caller);
        }

        public static void EventListeningStop<EventType>(this IEventListener<EventType> caller) where EventType : struct
        {
            EventManager.RemoveListener<EventType>(caller);
        }
    }

    public interface IEventListenerBase { };

    public interface IEventListener<T> : IEventListenerBase
    {
        void OnEvent(T eventType);
    }
}
