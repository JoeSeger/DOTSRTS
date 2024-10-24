using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace DOTSRTS.Events
{
    public interface IEventAggregator
    {
        void Subscribe<T>(params Action<T>[] listeners);
        void Unsubscribe<T>(params Action<T>[] listeners);
        void Clear();
        void Publish<T>(T eventData);
    }

    public interface IEventAggregatorHandle
    {
        EventAggregator EventAggregator { get; }

        void Subscribe<T>(params Action<T>[] listeners) => EventAggregator.Subscribe(listeners);
        void Unsubscribe<T>(params Action<T>[] listeners) => EventAggregator.Unsubscribe(listeners);
        void Clear() => EventAggregator.Clear();
        void Publish<T>(T eventData) => EventAggregator.Publish(eventData);
    }

    public abstract class EventAggregator : SerializedScriptableObject
    {
        [ShowInInspector, OdinSerialize] private readonly Dictionary<Type, List<Delegate>> _eventListeners = new();
        protected static readonly object Lock = new();
       
        public void Subscribe<T>(params Action<T>[] listeners)
        {
            if (listeners == null || listeners.Length == 0) return;

            lock (Lock)
            {
                if (!_eventListeners.TryGetValue(typeof(T), out var listenerActions))
                {
                    listenerActions = new List<Delegate>();
                    _eventListeners[typeof(T)] = listenerActions;
                }

                foreach (var listener in listeners)
                {
                    if (!listenerActions.Contains(listener))
                    {
                        listenerActions.Add(listener);
                    }
                }
            }
        }

        public void Unsubscribe<T>(params Action<T>[] listeners)
        {
            if (listeners == null || listeners.Length == 0) return;

            lock (Lock)
            {
                if (!_eventListeners.TryGetValue(typeof(T), out var listenerActions)) return;
                foreach (var listener in listeners)
                {
                    listenerActions.Remove(listener);
                }

                if (listenerActions.Count == 0)
                {
                    _eventListeners.Remove(typeof(T));
                }
            }
        }

        private void UnsubscribeAll()
        {
            lock (Lock)
            {
                _eventListeners.Clear(); // Clears all event types and listeners
            }
        }

        public void Clear() => UnsubscribeAll(); // Reuse UnsubscribeAll for clearing

        public void Publish<T>(T eventData)
        {
            lock (Lock)
            {
                if (!_eventListeners.TryGetValue(typeof(T), out var listeners)) return;
                var listenersCopy = new List<Delegate>(listeners);

                foreach (var listener in listenersCopy)
                {
                    ((Action<T>)listener)?.Invoke(eventData);
                }
            }

            // Creating a copy to avoid modification during iteration
        }
    }
}