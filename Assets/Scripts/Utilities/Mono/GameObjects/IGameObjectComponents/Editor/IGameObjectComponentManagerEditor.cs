// using System.Collections.Generic;
// using System.Linq;
// using DOTSRTS.Utilities.Mono.Singletons;
// using DOTSRTS.Utilities.Mono.Transforms;
// using Unity.Scenes;
// using UnityEditor;
// using UnityEngine;
//
// namespace DOTSRTS.Utilities.Mono.GameObjects.IGameObjectComponents.Editor
// {
//     
//     
//     [InitializeOnLoad]
//     public static class GameObjectComponentManagerEditor
//     {
//         
//         static GameObjectComponentManagerEditor()
//         {
//             // Subscribe to the hierarchyChanged event
//             EditorApplication.hierarchyChanged += OnHierarchyChanged;
//             EditorApplication.quitting += Clear;
//             
//             OnHierarchyChanged();
//
//         }
//
//         private static void OnHierarchyChanged()
//         {
//
//             var iGameObjectManagers = new Query<IGameObjectComponentManager>().FindObjectsOfType();
//             var previousComponents = new Query<IGameObjectComponent>().FindInterfacesOfType();
//             var subScenes = new Query<SubScene>().FindInterfacesOfType();
//
//             //     var IGameObjectComponentManager 
//             //     // Get the current list of IGameObjectComponent in the scene using TransformTools
//             //     var currentComponents = TransformTools.FindAllInterfacesInScene<IGameObjectComponent>();
//             //
//             //     
//             //     // Find and handle added components
//             //     foreach (var component in currentComponents.Where(component => !_previousComponents.Contains(component)))
//             //     {
//             //         _iGameObjectManager.AddGameObject(component);
//             //     }
//             //
//             //     // Find and handle removed components
//             //     foreach (var component in _previousComponents.Where(component => !currentComponents.Contains(component)))
//             //     {
//             //         _iGameObjectManager.RemoveGameObject(component);
//             //         component.Dispose(); // Dispose removed component
//             //     }
//             //
//             //     // Update the previous list to match the current one
//             //     _previousComponents = currentComponents.ToList();
//             
//             subScenes.Dispose();
//             previousComponents.Dispose();
//             iGameObjectManagers.Dispose();
//             
//         }
//
//         private static void Clear()
//         {
//             EditorApplication.hierarchyChanged -= OnHierarchyChanged;
//             EditorApplication.quitting -= Clear;
//         }
//     }
// }