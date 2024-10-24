using System;
using System.Linq;
using DOTSRTS.Utilities.DOTSGrid.Components;
using DOTSRTS.Utilities.DOTSGrid.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


namespace DOTSRTS.Utilities.DOTSGrid.Tools
{
    using Unity.Mathematics;


    public static class GridTools
    {
        public static quaternion ApplyAxisRotation(Axis rotationAxis)
        {
            return rotationAxis switch
            {
                // X-axis: rotates the grid so it lies on the YZ plane
                Axis.X => quaternion.Euler(0f, 0f, math.radians(90f)),

                // Y-axis: rotates the grid within the XZ plane around the Y-axis
                Axis.Y => quaternion.Euler(0f, math.radians(90f), 0f),

                // Z-axis: rotates the grid so it lies on the XY plane
                Axis.Z => quaternion.Euler(math.radians(90f), 0f, 0f),

                _ => quaternion.identity,
            };
        }


        public static bool HasComponents(Entity entity, EntityManager entityManager = default, params Type[] componentTypes)
        {
            var gettingEntityManager = entityManager == default
                ? World.DefaultGameObjectInjectionWorld.EntityManager
                : entityManager;
            
            // Check if the entity manager and entity are valid
            if (!gettingEntityManager.Exists(entity) || componentTypes == null || componentTypes.Length == 0 || entity == Entity.Null)
            {
                return false;
            }

            // Loop through each component type and check if the entity has it
            foreach (var type in componentTypes)
            {
                if (!gettingEntityManager.HasComponent(entity, type))
                {
                    return false; // If any component is missing, return false
                }
            }

            // All components were found
            return true;
        }
        


        public static GridComponent GetActiveGridGridComponent(Allocator allocator = Allocator.Temp, EntityManager entityManager = default)
        {
            var gettingEntityManager = entityManager == default
                ? World.DefaultGameObjectInjectionWorld.EntityManager
                : entityManager;
            var activeGridEntity = GetActiveGridEntity(allocator, gettingEntityManager);
            return gettingEntityManager.GetComponentData<GridComponent>(activeGridEntity);
        }
        public static Entity GetActiveGridEntity(Allocator allocator = Allocator.Temp, EntityManager entityManager = default)
        {
            var gettingEntityManager = entityManager == default
                ? World.DefaultGameObjectInjectionWorld.EntityManager
                : entityManager;

            using var entities = gettingEntityManager.GetAllEntities(allocator);
            foreach (var entity in entities.Where(entity => HasComponents(entity,entityManager,typeof(ActiveGrid),typeof(FocusGrid),typeof(GridComponent))))
            {
                return entity;

            }
            
            return Entity.Null;
        }
        
        


    }
}