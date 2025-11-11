using IAUS.ECS.Component;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utilities.ReactiveSystem;

[assembly:
    RegisterGenericComponentType(
        typeof(AIReactiveSystemBuffer<DestroyTargetTag, StateData, IAUS.ECS.Systems.Reactive.DestroyTargetTagReactor>.
            StateComponent))]
[assembly:
    RegisterGenericJobType(
        typeof(AIReactiveSystemBuffer<DestroyTargetTag, StateData, IAUS.ECS.Systems.Reactive.DestroyTargetTagReactor>.
            ManageComponentAdditionJob))]


namespace IAUS.ECS.Systems.Reactive
{
    public partial struct DestroyTargetTagReactor : IComponentReactorTagsForAIBuffer<DestroyTargetTag, StateData>
    {

        public void ComponentAdded(Entity entity, ref DestroyTargetTag newComponent, DynamicBuffer<StateData> AIStateCompoment)
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
        public void ComponentRemoved(Entity entity, DynamicBuffer<StateData> AIStateCompoment, in DestroyTargetTag oldComponent)
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


        public partial class ReactiveSystem : AIReactiveSystemBuffer<DestroyTargetTag, StateData, DestroyTargetTagReactor>
        {
            protected override DestroyTargetTagReactor CreateComponentReactor()
            {
                return new DestroyTargetTagReactor();
            }
        }

        [UpdateInGroup(typeof(IAUSUpdateGroup))]
        public partial class DestroyUpdateSystem : SystemBase
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
                depends = new DetermineDestroyTargetLocationAction()
                {
                }.Schedule(depends);
                depends = new ExecuteDestroyTargetLocationAction()
                {
                    DeltaTime = SystemAPI.Time.DeltaTime,
                    ECB = ecb.CreateCommandBuffer(World.Unmanaged).AsParallelWriter(),
                }.Schedule(depends);
                Dependency = depends;

            }
        }
    }
}
