using IAUS.ECS.Component;
using IAUS.ECS.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace DreamersInc.ReverbCity.GameCode.Entities
{
    [UpdateInGroup(typeof(IAUSUpdateGroup))]

    public partial struct GotTargetReactiveSystem : ISystem
    {
        ComponentLookup<IAUSBrain> brainLookup;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            brainLookup = state.GetComponentLookup<IAUSBrain>(true);
        }
        public void OnUpdate(ref SystemState state)
        {
            brainLookup.Update(ref state);
            var depends = state.Dependency;
            depends = new CopyJob()
            {
                BrainLookup = brainLookup,
                Buffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged)
            }.Schedule(depends);
            state.Dependency = depends;

        }
        [WithChangeFilter(typeof(TargetThisCommand))]
        partial struct CopyJob : IJobEntity
        {
            [ReadOnly]public ComponentLookup<IAUSBrain> BrainLookup;
            public EntityCommandBuffer Buffer;
            void Execute(DynamicBuffer<Child> children, in TargetThisCommand info)
            {
                Entity brain = Entity.Null;
                foreach (var child in children)
                {
                    if (!BrainLookup.HasComponent(child.Value))
                        continue;
                    brain = child.Value;
                    break;
                }
                if (brain != Entity.Null)
                {
                    Buffer.AddComponent(brain, new TargetThisCommand(info.Target, info.LastKnownPosition));
                }
            }
        }
    }
}