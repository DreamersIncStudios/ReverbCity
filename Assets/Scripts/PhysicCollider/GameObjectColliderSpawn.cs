using System;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using BoxCollider = UnityEngine.BoxCollider;
using CapsuleCollider = UnityEngine.CapsuleCollider;
using Collider = UnityEngine.Collider;
using MeshCollider = UnityEngine.MeshCollider;
using Object = UnityEngine.Object;

namespace DreamersInc.PhysicsSpawnSystem
{
    /// <summary>
    /// Class responsible for spawning colliders based on the given ColliderSpawn component.
    /// </summary>
    [DisallowMultipleComponent]
    public class GameObjectColliderSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject GO;
        class ColliderAuthorBaker : Baker<GameObjectColliderSpawn>
        {
            public override void Bake(GameObjectColliderSpawn authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
               if(authoring.GO)
                   AddComponentObject(entity, new ColliderGameObjectData()
                {
                    ColliderGO = authoring.GO
                });
      
            }
        }
    }

    public interface IAddMonoBehaviourToEntityOnAnimatorInstantiation
    {
    }

    class ColliderInstantiationData : IComponentData
    {
        public GameObject ColliderGameObject;
    }

    /// <summary>
    /// The ColliderSystem is responsible for creating various colliders on entities.
    /// </summary>
    partial struct ColliderSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            CreateColliderGo(ref state);
        }

        private void CreateColliderGo(ref SystemState state)
        {
            foreach (var entity in SystemAPI.QueryBuilder().WithAll<ColliderGameObjectData>()
                         .WithNone<GameObjectCleanup>()
                         .Build().ToEntityArray(state.WorldUpdateAllocator))
            {
                var goData = SystemAPI.ManagedAPI.GetComponent<ColliderGameObjectData>(entity);
                var entityTransform = SystemAPI.GetComponent<LocalToWorld>(entity);
                var spawnedCollider = Object.Instantiate(goData.ColliderGO.Value);
               
                SetColliderTransform(spawnedCollider.transform, entityTransform);


                state.EntityManager.AddComponentObject(entity, spawnedCollider);

                state.EntityManager.AddComponentData(entity, new GameObjectCleanup()
                {
                    DestroyThisGameObject = spawnedCollider
                });
            }
        }

        /// <summary>
        /// Sets the transform of a collider based on the position, rotation, and scale of the entity.
        /// </summary>
        /// <param name="colliderTransform">The transform component of the collider.</param>
        /// <param name="entityTransform">The LocalToWorld component of the entity.</param>
        private void SetColliderTransform(Transform colliderTransform, LocalToWorld entityTransform)
        {
            colliderTransform.position = entityTransform.Position;
            colliderTransform.rotation = entityTransform.Rotation;
            colliderTransform.localScale = entityTransform.Value.Scale();
        }
        
    }
}

