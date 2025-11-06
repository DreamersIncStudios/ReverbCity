using DreamersInc;
using MotionSystem.Components;
using MotionSystem.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;
[assembly: RegisterUnityEngineComponentType(typeof(Animator), ConcreteType = typeof(Animator))]
namespace MotionSystem
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(AnimatorUpdate))]
    public partial struct GroundCheckSystem : ISystem
    {
        private NativeParallelMultiHashMap<int, QuadrantData> quadrantMultiHashMap;
        private const int QuadrantZMultiplier = 1000;
        private const int QuadrantCellSize = 250;
        private EntityQuery query;

        private struct QuadrantData
        {
            public Entity Entity;
            public float3 Position;
        }

        private static int GetPositionHashMapKey(float3 position)
        {
            return (int)(Mathf.Floor(position.x / QuadrantCellSize) + (QuadrantZMultiplier * Mathf.Floor(position.z / QuadrantCellSize)));
        }

        private static NativeArray<int> GetNeighboringHashMapKeys(int centerHashKey)
        {
            var neighbors = new NativeArray<int>(8, Allocator.Temp);
            neighbors[0] = centerHashKey + 1; // Right
            neighbors[1] = centerHashKey - 1; // Left
            neighbors[2] = centerHashKey + QuadrantZMultiplier; // Up
            neighbors[3] = centerHashKey - QuadrantZMultiplier; // Down
            neighbors[4] = centerHashKey + 1 + QuadrantZMultiplier; // Up-Right
            neighbors[5] = centerHashKey - 1 + QuadrantZMultiplier; // Up-Left
            neighbors[6] = centerHashKey + 1 - QuadrantZMultiplier; // Down-Right
            neighbors[7] = centerHashKey - 1 - QuadrantZMultiplier; // Down-Left
            return neighbors;
        }

        private int GetEntityCountInHashMap(NativeParallelMultiHashMap<int, QuadrantData> quadrantMap, int hashMapKey)
        {
            var count = 0;
            if (!quadrantMap.TryGetFirstValue(hashMapKey, out _,
                    out var iterator)) return count;
            do
            {
                count++;
            }
            while (quadrantMap.TryGetNextValue(out _, ref iterator));
            return count;
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            quadrantMultiHashMap = new NativeParallelMultiHashMap<int, QuadrantData>(0, Allocator.Persistent);
            query = state.GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadWrite(typeof(LocalToWorld)),
                    ComponentType.ReadWrite(typeof(CharControllerE)),
                    ComponentType.ReadWrite(typeof(Animator))
                }
            });
        }


        public void OnDestroy(ref SystemState state)
        {
            quadrantMultiHashMap.Dispose();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (query.CalculateEntityCount() > quadrantMultiHashMap.Capacity)
            {
                quadrantMultiHashMap.Clear();
                quadrantMultiHashMap.Capacity = query.CalculateEntityCount();

                new SetQuadrantDataHashMapJob
                {
                    QuadrantMap = quadrantMultiHashMap.AsParallelWriter()
                }.ScheduleParallel(query);
            }

            if (!SystemAPI.TryGetSingletonEntity<Player_Control>(out var entityPlayer)) return;
            var playerPosition = SystemAPI.GetComponent<LocalToWorld>(entityPlayer).Position;
            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var world = physicsWorldSingleton.CollisionWorld;
            state.Dependency = new GroundCheckJob
            {
                HashKey = GetPositionHashMapKey((int3)playerPosition),
                World = world
            }.ScheduleParallel(state.Dependency);
        }




        [BurstCompile]
        public partial struct GroundCheckJob : IJobEntity
        {
            [ReadOnly] public CollisionWorld World;
            [ReadOnly] public int HashKey;
            private void Execute(ref LocalToWorld transform, ref CharControllerE control)
            {
                if (control.SkipGroundCheck)
                {
                    return;
                }

                var neighbors = GetNeighboringHashMapKeys(HashKey);
                control.IsGrounded = GroundCheck(transform, control, HashKey);

                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (GroundCheck(transform, control, neighbors[i]))
                    {
                        control.IsGrounded = true;
                        break;
                    }
                }
                neighbors.Dispose();
            }

            private bool GroundCheck(LocalToWorld transform, CharControllerE control, int hashKey)
            {
                if (hashKey != GetPositionHashMapKey((int3)transform.Position)) return false;
                var groundRays = new NativeList<RaycastInput>(Allocator.Temp);
                var filter = new CollisionFilter
                {
                    BelongsTo = ((1 << 10)),
                    CollidesWith = ((1 << 6) | (1 << 26)) | ((1 << 27) | (1 << 28)),
                    GroupIndex = 0
                };
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(0, .5f, 0),
                    End = transform.Position + new float3(0, -control.GroundCheckDistance, 0),
                    Filter = filter
                });
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(0, .5f, .25f),
                    End = transform.Position + new float3(0, -control.GroundCheckDistance, .25f),
                    Filter = filter
                });
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(0, .5f, -.25f),
                    End = transform.Position + new float3(0, -control.GroundCheckDistance, -.25f),
                    Filter = filter
                });
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(.25f, .5f, 0),
                    End = transform.Position + new float3(.25f, -control.GroundCheckDistance, 0),
                    Filter = filter
                });
                groundRays.Add(new RaycastInput
                {
                    Start = transform.Position + new float3(-.25f, .5f, 0),
                    End = transform.Position + new float3(-.25f, -control.GroundCheckDistance, 0),
                    Filter = filter
                });

                foreach (var ray in groundRays)
                {

                    var raycastArray = new NativeList<RaycastHit>(Allocator.Temp);

                    if (!World.CastRay(ray, ref raycastArray)) continue;
                    groundRays.Dispose();
                    return true;
                }
                return false;

            }
        }
        [BurstCompile]
        private partial struct SetQuadrantDataHashMapJob : IJobEntity
        {
            public NativeParallelMultiHashMap<int, QuadrantData>.ParallelWriter QuadrantMap;

            private void Execute(Entity entity, [ReadOnly] in LocalToWorld transform)
            {
                var hashMapKey = GetPositionHashMapKey(transform.Position);
                QuadrantMap.Add(hashMapKey, new QuadrantData
                {
                    Entity = entity,
                    Position = transform.Position
                });
            }
        }
    }
}
