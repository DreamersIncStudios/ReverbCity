using System;
using System.Collections.Generic;
using System.Linq;
using DreamersInc.ComboSystem;
using IAUS.ECS.Component;
using IAUS.ECS.Component.Attacking;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utilities.ReactiveSystem;


[assembly: RegisterGenericComponentType(typeof(AIReactiveSystemBuffer<AttackActionTag, StateData, IAUS.ECS.Systems.Reactive.AttackTagReactor>.StateComponent))]
[assembly: RegisterGenericJobType(typeof(AIReactiveSystemBuffer<AttackActionTag, StateData, IAUS.ECS.Systems.Reactive.AttackTagReactor>.ManageComponentAdditionJob))]

namespace IAUS.ECS.Systems.Reactive
{

    public partial struct AttackTagReactor : IComponentReactorTagsForAIBuffer<AttackActionTag, StateData>
    {
        public void ComponentAdded(Entity entity, ref AttackActionTag newAITag, DynamicBuffer<StateData> AIStateCompoment)
        {
            for (int i = 0; i < AIStateCompoment.Length; i++)
            {
                if (AIStateCompoment[i].State != AIStates.WanderQuadrant)
                    continue;
                var temp = AIStateCompoment[i];
                temp.SetStatus(ActionStatus.Running);
                AIStateCompoment[i] = temp;
            }
        }

        public void ComponentRemoved(Entity entity, DynamicBuffer<StateData> AIStateCompoment, in AttackActionTag oldComponent)
        {
            for (int i = 0; i < AIStateCompoment.Length; i++)
            {
                if (AIStateCompoment[i].State != AIStates.WanderQuadrant)
                    continue;
                var temp = AIStateCompoment[i];
                temp.SetStatus(ActionStatus.Success);
                temp.ResetTime = 15;
                AIStateCompoment[i] = temp;
            }
        }



        public partial class ReactiveSystem : AIReactiveSystemBuffer<AttackActionTag, StateData, AttackTagReactor>
        {
            protected override AttackTagReactor CreateComponentReactor()
            {
                return new AttackTagReactor();
            }
        }
        [UpdateInGroup(typeof(IAUSUpdateGroup))]
        public partial class AttackUpdateSystem : SystemBase
        {

            private BeginSimulationEntityCommandBufferSystem.Singleton ecb;
            protected override void OnCreate()
            {
                ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            }

            [Obsolete("Obsolete")]
            protected override void OnUpdate()
            {
                var depends = Dependency;

                depends = new DetermineAttackAction()
                {
                    DeltaTime = SystemAPI.Time.DeltaTime,
                    ECB = ecb.CreateCommandBuffer(World.Unmanaged).AsParallelWriter(),
                }.Schedule(depends);

                depends = new ExecuteAttackAction()
                {
                    DeltaTime = SystemAPI.Time.DeltaTime,
                    ECB = ecb.CreateCommandBuffer(World.Unmanaged).AsParallelWriter(),
                }.Schedule(depends);

                Dependency = depends;

                Entities.WithoutBurst().WithStructuralChanges().ForEach(
                    (Entity entity, NPCAttack comboList, in SelectAndAttack select, in Parent root) =>
                    {
                        var handler = EntityManager.GetComponentObject<Command>(root.Value);
                        var anim = EntityManager.GetComponentObject<Animator>(root.Value);
                        handler.InputQueue ??= new Queue<AnimationTrigger>();
                        if (anim.IsInTransition(0)) return;
                        handler.InputQueue.Enqueue(
                            comboList.AttackSequence.PickAttack(IAttackSequence.AttackType.Melee)[0]);

                        EntityManager.RemoveComponent<SelectAndAttack>(entity);

                    }).Run();
            }
        }


    }
}
