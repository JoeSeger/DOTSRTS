using System;
using DOTSRTS.Events.Controls;
using DOTSRTS.Utilities.CSharp.Controls;
using DOTSRTS.Utilities.DOTSGrid.Data;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Events.Editor
{
    [InitializeOnLoad]
    public static class EditorMouseEventManager
    {
        public static float2 CurrentMousePosition { get; private set; }
        public static float2 LastMousePosition { get; private set; }

        public static float2 ScreenPosition { get; private set; }
        public static bool RightButtonPressed { get; private set; }
        public static bool LeftButtonPressed { get; private set; }


        static EditorMouseEventManager()
        {
            EditorEventManager.EditorEventAggregator.Subscribe<OnEditorEventTriggered>(OnEditorEventTriggered);
            CurrentMousePosition = float2.zero;
            LastMousePosition = float2.zero;
        }

        private static void OnEditorEventTriggered(OnEditorEventTriggered eventTriggered)
        {
            var currentEvent = eventTriggered.CurrentEvent;
            if (currentEvent == null) return;
            LastMousePosition = CurrentMousePosition;
            CurrentMousePosition = currentEvent.mousePosition;

            var currentEventType = eventTriggered.CurrentEventType;
            if (!IsMouseEventType(currentEventType)) return;

            var isMouseDownEvent = currentEventType == EventType.MouseDown;
            RightButtonPressed = isMouseDownEvent && currentEvent.button == (int)MouseKey.Right;
            LeftButtonPressed = isMouseDownEvent && currentEvent.button == (int)MouseKey.Left;
            ScreenPosition = HandleUtility.GUIPointToScreenPixelCoordinate(CurrentMousePosition);
            EditorEventManager.EditorEventAggregator.Publish(new OnMouseEventChanged
            {
                MouseEventType = ConvertToMouseEventType(currentEventType),
                CurrentMousePosition = CurrentMousePosition,
                ScreenPosition = ScreenPosition,
                LastPosition = LastMousePosition,
                RightButtonPressed = RightButtonPressed,
                LeftButtonPressed = LeftButtonPressed
            });
        }

        public static bool IsMouseEventType(EventType eventType) =>
            Enum.IsDefined(typeof(MouseEventType), (int)eventType);


        public static MouseEventType ConvertToMouseEventType(EventType eventType)
        {
            // Check if the EventType corresponds to a defined MouseEventType
            if (Enum.IsDefined(typeof(MouseEventType), (int)eventType))
            {
                return (MouseEventType)(int)eventType;
            }

            // If the EventType is not a MouseEventType, return null
            return default;
        }


        // Add specific mouse-related events and logic here if needed
    }
}