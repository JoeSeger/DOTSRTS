using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DOTSRTS.Utilities.DOTS.Editor
{
    public static class SystemEditorTools
    {
        // Method to update all systems that have the UpdateOnSceneGUI attribute
        public static void ForceUpdateSystemsWithAttribute<T>() where T : Attribute
        {
            // // Get the current world (you can modify this to handle multiple worlds)
            var world = World.DefaultGameObjectInjectionWorld;
            //
            // if (world == null) return;

            // Iterate through all systems in the world
            foreach (var system in world.Systems)
            {
                // Get the type of the system
                Type systemType = system.GetType();

                if (!system.ShouldRunSystem()) continue;
                // Check if the system has the UpdateOnSceneGUI attribute
                if (systemType.GetCustomAttribute<T>() == null) continue;
                // Update the system if it should run
                if (system.Enabled)
                {
                    system.Update();
                }
            }
        }


        public static bool IsMouseEventTriggered(int id,out float2 mousePosition,EventType eventType = EventType.MouseDown)
        {
            var sceneEvent = Event.current;

            if (sceneEvent == null)
            {
                mousePosition = default;
                return false;
            }

            var eventTriggered = (sceneEvent.type == eventType && sceneEvent.button == id);
            mousePosition = sceneEvent.mousePosition;
            // Check for left mouse button click
            
           // Event.current.Use();
            return eventTriggered;
        }

        public static bool IsMouseOverSceneView()
        {
            var mouseOverWindowData = EditorWindow.mouseOverWindow;
            return mouseOverWindowData != null && mouseOverWindowData.wantsMouseMove;
        }

        [MenuItem("Tools/IP")]
        public static void PrintLocalIPAddress()
        {
            string localIP = GetLocalIPAddress();
            if (!string.IsNullOrEmpty(localIP))
            {
                Debug.Log("Your local IP address is: " + localIP);
            }
            else
            {
                Debug.LogError("Could not find local IP address.");
            }
        }

        private static string GetLocalIPAddress()
        {
            string localIP = string.Empty;

            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily != AddressFamily.InterNetwork) continue;
                localIP = ip.ToString();
                break;
            }

            return localIP;
        }
    }
}