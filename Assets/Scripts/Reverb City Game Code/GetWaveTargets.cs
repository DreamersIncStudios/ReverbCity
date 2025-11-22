using AISenses;
using DreamersInc.ReverbCity.GameCode.Entities;
using DreamersIncStudio.GAIACollective;
using Global.Component;
using IAUS.ECS.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Resources = AISenses.Resources;

namespace DreamersInc.ReverbCity.GameCode.Entities
{

    [UpdateInGroup(typeof(IAUSUpdateGroup))]
    public partial struct GetWaveTargets : ISystem
    {
        private EntityQuery targets;
        private ComponentLookup<TargetThisCommand> targetLookup;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<RunningTag>();
            targets = state.GetEntityQuery(new EntityQueryDesc()
            {
                All = new[]
                {
                    ComponentType.ReadOnly<AITarget>(),
                    ComponentType.ReadOnly<LocalToWorld>()
                },
                Any = new[]
                {
                    ComponentType.ReadOnly<Speaker>(),
                    ComponentType.ReadOnly<BatteryCell>(),
                    ComponentType.ReadOnly<Antenna>(),
                }
            });
            targetLookup = state.GetComponentLookup<TargetThisCommand>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            targetLookup.Update(ref state);
            var depends = state.Dependency;
            depends = new GetTargetEnemiesJob()
            {
                Targets = targets.ToEntityArray(Allocator.TempJob),
                AITargets = targets.ToComponentDataArray<AITarget>(Allocator.TempJob),
                Positions = targets.ToComponentDataArray<LocalToWorld>(Allocator.TempJob)
            }.ScheduleParallel(depends);

            depends = new GetTargetResourcesJob()
            {
                Targets = targets.ToEntityArray(Allocator.TempJob),
                AITargets = targets.ToComponentDataArray<AITarget>(Allocator.TempJob),
                Positions = targets.ToComponentDataArray<LocalToWorld>(Allocator.TempJob)
            }.ScheduleParallel(depends);
            depends = new GetCurrentTargets()
            {
                TargetLookup = targetLookup
            }.ScheduleParallel(depends);
            depends = new AssignTarget()
            {
                TargetLookup = targetLookup,
                Writer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged)
            }.Schedule(depends);
            state.Dependency = depends;
        }

        public void OnDestroy(ref SystemState state)
        {
            targets.Dispose();
        }

        [WithAll(typeof(Pack))]
        private partial struct GetTargetEnemiesJob : IJobEntity
        {
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> Targets;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<AITarget> AITargets;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<LocalToWorld> Positions;
            void Execute(DynamicBuffer<Enemies> enemies, in LocalToWorld transform)
            {
                enemies.Clear();
                for (int i = 0; i < Targets.Length; i++)
                {
                    var dist = Vector3.Distance(Positions[i].Position, transform.Position);
                    if (dist <= 75f)
                        enemies.Add(new Enemies()
                        {
                            Target = new Target()
                            {
                                Entity = Targets[i],
                                DistanceTo = dist,
                                LastKnownPosition = Positions[i].Position,
                                TargetInfo = AITargets[i]
                            },

                        });
                }
            }
        }
        
        [WithAll(typeof(Pack))]
        private partial struct GetTargetResourcesJob : IJobEntity
        {
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> Targets;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<AITarget> AITargets;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<LocalToWorld> Positions;
            void Execute(DynamicBuffer<Resources> resource, in LocalToWorld transform)
            {
                resource.Clear();
                for (int i = 0; i < Targets.Length; i++)
                {
                    var dist = Vector3.Distance(Positions[i].Position, transform.Position);
                    if (dist <= 75f)
                        resource.Add(new Resources()
                        {
                            Target = new Target()
                            {
                                Entity = Targets[i],
                                DistanceTo = dist,
                                LastKnownPosition = Positions[i].Position,
                                TargetInfo = AITargets[i]
                            },

                        });
                }
            }
        }
        
                
        [WithAll(typeof(Pack))]
        partial struct GetCurrentTargets : IJobEntity
        {
            [ReadOnly] public ComponentLookup<TargetThisCommand> TargetLookup;
            void Execute(DynamicBuffer<Enemies> enemies, DynamicBuffer<Child> children)
            {

                foreach (var child in children)
                {
                    if (!TargetLookup.HasComponent(child.Value))
                        continue;
                    for (var index = 0; index < enemies.Length; index++)
                    {

                        var enemy = enemies[index];
                        if (!enemy.Target.Entity.Equals(child.Value))
                            continue;
                        enemy.NumberOfTargetingNPCs++;
                        enemies[index] = enemy;
                        break;
                    }
                }
            }
        }
        
        [WithAll(typeof(Pack))]
        partial struct AssignTarget : IJobEntity
        {
            [ReadOnly] public ComponentLookup<TargetThisCommand> TargetLookup;
            public EntityCommandBuffer Writer;
            void Execute(DynamicBuffer<Enemies> enemies, DynamicBuffer<Child> children)
            {

                foreach (var child in children)
                {
                    if (TargetLookup.HasComponent(child.Value))
                        continue;
                    for (var index = 0; index < enemies.Length; index++)
                    {

                        var enemy = enemies[index];
                        if (enemy.NumberOfTargetingNPCs > 6) continue;

                        Writer.AddComponent(child.Value, new TargetThisCommand(enemy.Target.Entity, enemy.Target.LastKnownPosition));

                        enemies[index] = enemy;
                        break;
                    }
                }
            }
        }
        
    }


    
    public struct TargetThisCommand : IComponentData
    {
        public Entity Target;
        public float3 LastKnownPosition;
        public TargetThisCommand(Entity targetEntity, float3 targetLastKnownPosition)
        {
            Target = targetEntity;
            LastKnownPosition = targetLastKnownPosition;
        }
    }
}