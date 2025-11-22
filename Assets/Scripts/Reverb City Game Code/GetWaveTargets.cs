using AISenses;
using DreamersInc.ReverbCity.GameCode.Entity;
using DreamersIncStudio.GAIACollective;
using Global.Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Resources = AISenses.Resources;

public partial struct GetWaveTargets : ISystem
{
    private EntityQuery _targets;
    public void OnCreate(ref SystemState state)
    {
        _targets = state.GetEntityQuery(new EntityQueryDesc()
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
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var depends = state.Dependency;
        depends = new GetTargetEnemiesJob()
        {
            Targets = _targets.ToEntityArray(Allocator.TempJob),
            aiTargets = _targets.ToComponentDataArray<AITarget>(Allocator.TempJob),
            Positions = _targets.ToComponentDataArray<LocalToWorld>(Allocator.TempJob)
        }.ScheduleParallel(depends);
        
        depends = new GetTargetResourcesJob()
        {
            Targets = _targets.ToEntityArray(Allocator.TempJob),
            aiTargets = _targets.ToComponentDataArray<AITarget>(Allocator.TempJob),
            Positions = _targets.ToComponentDataArray<LocalToWorld>(Allocator.TempJob)
        }.ScheduleParallel(depends);

        state.Dependency = depends;
    }

    public void OnDestroy(ref SystemState state)
    {
        _targets.Dispose();
    }

    [WithAll(typeof(Pack))]
    private partial struct GetTargetEnemiesJob : IJobEntity
    {
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> Targets;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<AITarget> aiTargets;
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
                            TargetInfo = aiTargets[i]
                        },
                        
                    });
            }
        }
    }  
    [WithAll(typeof(Pack))]
    private partial struct GetTargetResourcesJob : IJobEntity
    {
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> Targets;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<AITarget> aiTargets;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<LocalToWorld> Positions;
        void Execute(DynamicBuffer<AISenses.Resources> resource, in LocalToWorld transform)
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
                            TargetInfo = aiTargets[i]
                        },
                        
                    });
            }
        }
    }
}
