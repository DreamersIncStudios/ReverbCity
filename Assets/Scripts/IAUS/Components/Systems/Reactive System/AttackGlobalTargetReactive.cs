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
        [UpdateInGroup(typeof(IAUSUpdateGroup))]
        public partial class AttackUpdateSystem2 : SystemBase
        {
            private BeginSimulationEntityCommandBufferSystem.Singleton ecb;
            protected override void OnCreate()
            {
                RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            }
            protected override void OnUpdate()
            {
                ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                
                var depends = Dependency;
                
                depends = new DetermineAttackActionGlobal()
                {
                    ECB = ecb.CreateCommandBuffer(World.Unmanaged).AsParallelWriter(),
                }.Schedule(depends);

                depends = new ExecuteAttackActionGlobal()
                {
                    DeltaTime = SystemAPI.Time.DeltaTime,
                    ECB = ecb.CreateCommandBuffer(World.Unmanaged).AsParallelWriter(),
                }.Schedule(depends);
            
                Dependency = depends;
                
            }

           
        }
    }
}
