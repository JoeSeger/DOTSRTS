using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Events.Editor
{
    public struct OnEditorSceneViewEvent
    {
        public SceneView SceneView;
    }

    public struct EditorOnHierarchyChangedEvent
    {
        // Add any relevant data for the hierarchy change event
    }

    public struct OnEditorOnProjectChangedEvent
    {
        // Add any relevant data for the project change event
    }

    public struct OnEditorOnPlayModeStateChangedEvent
    {
        public PlayModeStateChange State;
    }

    public struct OnEditorPauseStateChangedEvent
    {
        public PauseState State;
    }

    public struct OnEditorFocusChangedEvent
    {
        public bool HasFocus;
    }

    public struct OnEditorDelayCallEvent
    {
        // Add any relevant data for the delay call event
    }

    public struct OnEditorUpdateEvent
    {
        // Add any relevant data for the update event
    }

    public struct OnEditorEventTriggered
    {
        public Event CurrentEvent;
        public Event LastEvent;
        public EventType CurrentEventType;
        public EventType LastEventType;
    }

    [InitializeOnLoad]
    public static class EditorEventManager
    {
        public static Event CurrentEvent { get; private set; }
        public static Event LastEvent { get; private set; }


        public static EditorEventAggregator EditorEventAggregator
        {
            get
            {
                if (_editorEventAggregator == null)
                {
                    _editorEventAggregator = EditorEventAggregator.Instance;
                }

                return _editorEventAggregator;
            }
        }


        private static EditorEventAggregator _editorEventAggregator;


        static EditorEventManager()
        {
            _editorEventAggregator = EditorEventAggregator.Instance;
            // Subscribe to editor events
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.update += EditorUpdate;
            EditorApplication.quitting += Clear;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.projectChanged += OnProjectChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.pauseStateChanged += OnPauseStateChanged;
            EditorApplication.focusChanged += OnFocusChanged;
            EditorApplication.delayCall += OnDelayCall;

            // Initialize events and mouse positions to avoid null references
            CurrentEvent = new Event();
            LastEvent = new Event();
        }

        private static void EditorUpdate()
        {
            // Publish the update event to the aggregator
            EditorEventAggregator.Publish(new OnEditorUpdateEvent());
        }

        // Method called during the SceneView GUI event
        private static void OnSceneGUI(SceneView sceneView)
        {
            EditorEventAggregator.Publish(new OnEditorSceneViewEvent { SceneView = sceneView });

            if (Event.current == null) return;


            // Update the last event and mouse position before setting the current event
            LastEvent = CurrentEvent;
            CurrentEvent = new Event(Event.current);

            EditorEventAggregator.Publish(new OnEditorEventTriggered
            {
                LastEvent = LastEvent,
                LastEventType = LastEvent.type,
                CurrentEvent = CurrentEvent,
                CurrentEventType = CurrentEvent.type,
                
            });

            // Publish the scene view event
        }

        private static void OnHierarchyChanged()
        {
            // Publish the hierarchy change event
            EditorEventAggregator.Publish(new EditorOnHierarchyChangedEvent());
        }

        private static void OnProjectChanged()
        {
            // Publish the project change event
            EditorEventAggregator.Publish(new OnEditorOnProjectChangedEvent());
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Publish the play mode state change event
            EditorEventAggregator.Publish(new OnEditorOnPlayModeStateChangedEvent { State = state });
        }

        private static void OnPauseStateChanged(PauseState state)
        {
            // Publish the pause state change event
            EditorEventAggregator.Publish(new OnEditorPauseStateChangedEvent { State = state });
        }

        private static void OnFocusChanged(bool hasFocus)
        {
            // Publish the focus change event
            EditorEventAggregator.Publish(new OnEditorFocusChangedEvent { HasFocus = hasFocus });
        }

        private static void OnDelayCall()
        {
            // Publish the delay call event
            EditorEventAggregator.Publish(new OnEditorDelayCallEvent());
        }

        private static void Clear()
        {
            EditorEventAggregator.Clear();
            // Unsubscribe from events
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.update -= EditorUpdate;
            EditorApplication.quitting -= Clear;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.projectChanged -= OnProjectChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.pauseStateChanged -= OnPauseStateChanged;
            EditorApplication.focusChanged -= OnFocusChanged;
            EditorApplication.delayCall -= OnDelayCall;
        }
    }
}