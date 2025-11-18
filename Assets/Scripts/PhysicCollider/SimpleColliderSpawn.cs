using Stats;
using Stats.Entities;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DreamersInc.PhysicsSpawnSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class SimpleColliderSpawn : MonoBehaviour
    {
        [SerializeField] private Collider col;
        [SerializeField] private bool damageable = false;

        private void OnValidate()
        {
            col = GetComponent<Collider>();
        }
        private class ColliderAuthorBaker : Baker<SimpleColliderSpawn>
        {
            public override void Bake(SimpleColliderSpawn authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                switch (authoring.col)
                {
                    case BoxCollider box:
                        AddComponent(entity,
                            new BoxColliderData(box, authoring.col.gameObject.transform, authoring.damageable));
                        break;
                    case CapsuleCollider capsule:
                        AddComponent(entity,
                            new CapsuleColliderData(capsule, authoring.col.gameObject.transform, authoring.damageable));
                        break;
                    case MeshCollider mesh:
                        AddComponent(entity,
                            new MeshColliderData(mesh, authoring.col.gameObject.transform, authoring.damageable));
                        break;
                }
            }
        }
    }

    partial class SetupColliderSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithNone<ColliderCleanup>().ForEach((ref BoxColliderData boxData, ref LocalTransform transform) =>
            {
                transform.Position = boxData.Position;
                transform.Rotation = boxData.Rotation;
            }).ScheduleParallel();
            Entities.WithNone<ColliderCleanup>()
                .ForEach((ref CapsuleColliderData boxData, ref LocalTransform transform) =>
                {
                    transform.Position = boxData.Position;
                    transform.Rotation = boxData.Rotation;
                }).ScheduleParallel();

            Entities.WithNone<ColliderCleanup>().ForEach((ref MeshColliderData boxData, ref LocalTransform transform) =>
            {
                transform.Position = boxData.Position;
                transform.Rotation = boxData.Rotation;
            }).ScheduleParallel();
        }
    }

    [UpdateAfter(typeof(SetupColliderSystem))]
    partial struct BasicColliderSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var commandBufferParallel = ecb.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            CreateCapsuleCollider(ref state, commandBufferParallel);
            CreateBoxCollider(ref state, commandBufferParallel);
            CreateMeshCollider(ref state, commandBufferParallel);
        }


        private void CreateBoxCollider(ref SystemState state, EntityCommandBuffer.ParallelWriter commandBufferParallel)
        {
            foreach (var entity in SystemAPI.QueryBuilder().WithAll<BoxColliderData>()
                         .WithNone<ColliderCleanup>()
                         .Build().ToEntityArray(state.WorldUpdateAllocator))
            {
                var boxData = SystemAPI.GetComponent<BoxColliderData>(entity);
                var spawnedCollider = new GameObject().AddComponent<BoxCollider>();
                spawnedCollider.gameObject.layer = LayerMask.NameToLayer("Environment");
                spawnedCollider.center = boxData.Center;
                spawnedCollider.size = boxData.Size;
                spawnedCollider.transform.localScale = boxData.Scale;
                SetColliderTransform(spawnedCollider.transform, boxData.Position, boxData.Rotation);

                state.EntityManager.AddComponentObject(entity, spawnedCollider);

                state.EntityManager.AddComponentData(entity, new ColliderCleanup()
                {
                    DestroyThisCollider = spawnedCollider
                });
            }
        }

        private void CreateCapsuleCollider(ref SystemState state,
            EntityCommandBuffer.ParallelWriter commandBufferParallel)
        {
            foreach (var entity in SystemAPI.QueryBuilder().WithAll<CapsuleColliderData>()
                         .WithNone<ColliderCleanup>()
                         .Build().ToEntityArray(state.WorldUpdateAllocator))
            {
                var capsuleData = SystemAPI.GetComponent<CapsuleColliderData>(entity);
                var spawnedCollider = new GameObject().AddComponent<CapsuleCollider>();

                spawnedCollider.gameObject.layer = LayerMask.NameToLayer("Environment");

                spawnedCollider.center = capsuleData.Center;
                spawnedCollider.height = capsuleData.Height;
                spawnedCollider.radius = capsuleData.Radius;
                spawnedCollider.transform.localScale = capsuleData.Scale;
                SetColliderTransform(spawnedCollider.transform, capsuleData.Position, capsuleData.Rotation);

                state.EntityManager.AddComponentObject(entity, spawnedCollider);

                state.EntityManager.AddComponentData(entity, new ColliderCleanup()
                {
                    DestroyThisCollider = spawnedCollider
                });
            }
        }

        private void CreateMeshCollider(ref SystemState state, EntityCommandBuffer.ParallelWriter commandBufferParallel)
        {
            foreach (var entity in SystemAPI.QueryBuilder().WithAll<MeshColliderData>()
                         .WithNone<ColliderCleanup>()
                         .Build().ToEntityArray(state.WorldUpdateAllocator))
            {
                var meshColliderData = SystemAPI.GetComponent<MeshColliderData>(entity);
                var spawnedCollider = new GameObject().AddComponent<MeshCollider>();
                spawnedCollider.gameObject.layer = LayerMask.NameToLayer("Environment");

                spawnedCollider.transform.localScale = meshColliderData.Scale;

                spawnedCollider.sharedMesh = meshColliderData.mesh;

                SetColliderTransform(spawnedCollider.transform, meshColliderData.Position, meshColliderData.Rotation);

                state.EntityManager.AddComponentObject(entity, spawnedCollider);

                state.EntityManager.AddComponentData(entity, new ColliderCleanup()
                {
                    DestroyThisCollider = spawnedCollider
                });
            }
        }

        private void SetColliderTransform(Transform colliderTransform, float3 position, Quaternion rotation)
        {
            colliderTransform.position = position;
            colliderTransform.rotation = rotation;
        }
    }
}