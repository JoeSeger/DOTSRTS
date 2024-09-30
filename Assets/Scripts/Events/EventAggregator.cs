using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace DOTSRTS.Events
{

    public interface IEventAggregator
    {
        public void Subscribe<T>(params Action<T>[] listeners) where T : struct;

        public void Unsubscribe<T>(params Action<T>[] listeners) where T : struct;

        public void Clear();

        public void Publish<T>(T eventData) where T : struct;

    }
    public interface IEventAggregatorHandel 
    {
        public EventAggregator EventAggregator { get; }

        public void Subscribe<T>(params Action<T>[] listeners) where T : struct =>  EventAggregator.Subscribe(listeners);

        public void Unsubscribe<T>(params Action<T>[] listeners) where T : struct =>
            EventAggregator.Subscribe(listeners);

        public void Clear() => EventAggregator.Clear();

        public void Publish<T>(T eventData) where T : struct => EventAggregator.Publish<T>(eventData);

    }
    public abstract class EventAggregator : SerializedScriptableObject
    {
        [ShowInInspector, OdinSerialize]
        private Dictionary<Type, List<Delegate>> _eventListeners = new();

        public void Subscribe<T>(params Action<T>[] listeners) where T : struct
        {
            foreach (var listener in listeners)
            {
                if (_eventListeners.TryGetValue(typeof(T), out var listenerActions))
                {
                    if (!listenerActions.Contains(listener))
                    {
                        listenerActions.Add(listener);
                    }
                }
                else
                {
                    _eventListeners[typeof(T)] = new List<Delegate> { listener };
                }
            }
        }

        public void Clear()
        {
            _eventListeners = new Dictionary<Type, List<Delegate>>();
        }
        public void Unsubscribe<T>(params Action<T>[] listeners) where T : struct
        {
            foreach (var listener in listeners)
            {
                if (_eventListeners.TryGetValue(typeof(T), out var listenersActions))
                {
                    listenersActions.Remove(listener);
                }
            }
        }

        public void Publish<T>(T eventData) where T : struct
        {
            if (!_eventListeners.TryGetValue(typeof(T), out var listeners)) return;
            foreach (var listener in listeners)
            {
                ((Action<T>)listener)?.Invoke(eventData);
            }
        }
    }
}