using System.Collections.Generic;
using System.Linq;
using IAUS.ECS.Component;
using IAUS.ECS.Component.Attacking;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utilities.ReactiveSystem;


[assembly: RegisterGenericComponentType(typeof(AIReactiveSystemBuffer<AttackGlobalTag, StateData, IAUS.ECS.Systems.Reactive.AttackGlobalTagReactor>.StateComponent))]
[assembly: RegisterGenericJobType(typeof(AIReactiveSystemBuffer<AttackGlobalTag, StateData, IAUS.ECS.Systems.Reactive.AttackGlobalTagReactor>.ManageComponentAdditionJob))]

namespace IAUS.ECS.Systems.Reactive
{
    public partial struct AttackGlobalTagReactor : IComponentReactorTagsForAIBuffer<AttackGlobalTag, StateData>
    {
        public void ComponentAdded(Entity entity, ref AttackGlobalTag newComponent, DynamicBuffer<StateData> AIStateCompoment)
        {
            for (int i = 0; i < AIStateCompoment.Length; i++)
            {
                if (AIStateCompoment[i].State != AIStates.AttackGlobalTarget)
                    continue;
                var temp = AIStateCompoment[i];
                temp.SetStatus(ActionStatus.Running);
                AIStateCompoment[i] = temp;
            }
        }

        public void ComponentRemoved(Entity entity, DynamicBuffer<StateData> AIStateCompoment, in AttackGlobalTag oldComponent)
        {
            for (int i = 0; i < AIStateCompoment.Length; i++)
            {
                if (AIStateCompoment[i].State != AIStates.AttackGlobalTarget)
                    continue;
                var temp = AIStateCompoment[i];
                temp.SetStatus(ActionStatus.Success);
                temp.ResetTime = 15;
                AIStateCompoment[i] = temp;
            }
        }

        public partial class ReactiveSystem : AIReactiveSystemBuffer<AttackGlobalTag, StateData, AttackGlobalTagReactor>
        {
            protected override AttackGlobalTagReactor CreateComponentReactor()
            {
                return new AttackGlobalTagReactor();
            }
        }
        public partial class AttackUpdateSystem : SystemBase
        {
            private BeginSimulationEntityCommandBufferSystem.Singleton ecb;
            protected override void OnCreate()
            {
                ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            }
            protected override void OnUpdate()
            {
                var depends = Dependency;
                
                Dependency = depends;
            }

            partial struct GetAttackPosition : IJobEntity
            {
                [ReadOnly] public BufferLookup<MeleeAttackPosition> MeleeAttackPositions;
                [ReadOnly] public BufferLookup<Child> ChildBufferLookup;
                [NativeDisableParallelForRestriction] public BufferLookup<ReserveLocationTag> ReserveLocationBuffer;

                void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref LocalTransform transform,
                    ref AttackGlobalTag state, in TargetThisCommand command)
                {
                    if (state.AttackPlans.IsEmpty) return;
                    if (state.AttackPlans[0] != AttackPlan.GetAttackLocation)
                        return;
                    state.TargetEntity = command.Target;
                    var child = ChildBufferLookup[command.Target][0].Value;
                    state.TargetPosition = float3.zero;

                    List<DistCheck> dist = new();
                    var buffer = MeleeAttackPositions[child];
                    for (var i = 0; i < buffer.Length - 1; i++)
                    {
                        var index = i;
                        ;
                        dist.Add(
                            new DistCheck()
                            {
                                Distance =
                                    Vector3.Distance(transform.Position, buffer[i].Position),
                                Index = index
                            });
                    }

                    var orderBy = dist.OrderBy(x => x.Distance);

                    foreach (var check in orderBy)
                    {
                        if (buffer[check.Index].State != OccupiedState.Vacant) continue;
                        ReserveLocationBuffer[child].Add(new ReserveLocationTag()
                        {
                            ReserveEntity = entity,
                            ID = check.Index
                        });
                        break;
                    }
                }

                class DistCheck
                {
                    public float Distance;
                    public int Index;
                }
            }
        }
    }
}
