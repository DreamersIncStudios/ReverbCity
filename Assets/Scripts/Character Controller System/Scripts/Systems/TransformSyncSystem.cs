using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

namespace MotionSystem
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial struct TransformSyncSystem : ISystem
    {
        EntityQuery m_Query;
        ComponentLookup<LocalTransform> m_TransformLookup;

        public void OnCreate(ref SystemState state)
        {
            m_Query = SystemAPI.QueryBuilder()
                .WithNone<AgentBody>()
                .WithAll<Transform>()
                .WithAllRW<LocalTransform>()
                .Build();
            m_TransformLookup = state.GetComponentLookup<LocalTransform>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entities = m_Query.ToEntityArray(Allocator.TempJob);
            var transformAccessArray = m_Query.GetTransformAccessArray();

            m_TransformLookup.Update(ref state);

            state.Dependency = new WriteAgentTransformJob
            {
                Entities = entities,
                TransformLookup = m_TransformLookup,
            }.Schedule(transformAccessArray, state.Dependency);
        }

        [BurstCompile]
        struct WriteAgentTransformJob : IJobParallelForTransform
        {
            [DeallocateOnJobCompletion] public NativeArray<Entity> Entities;

            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> TransformLookup;

            public void Execute(int index, [ReadOnly] TransformAccess transformAccess)
            {
                Entity entity = Entities[index];

                var transform = TransformLookup[entity];
                transform.Position = transformAccess.position;
                transform.Rotation = transformAccess.rotation;
                TransformLookup[entity] = transform;
            }
        }
    }
}