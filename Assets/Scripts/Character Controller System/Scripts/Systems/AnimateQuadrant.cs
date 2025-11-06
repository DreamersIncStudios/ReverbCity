using DreamersInc.QuadrantSystems;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using MotionSystem.Components;
using Unity.Mathematics;
using Unity.Collections;
using DreamersInc;
using Unity.Burst;

namespace MotionSystem.Systems
{
    public partial class AnimateQuadrant : GenericQuadrantSystem
    {
        EntityQuery withTag;
        EntityQuery withoutTag;
        protected override void OnCreate()
        {
            base.OnCreate();
            Query = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(LocalTransform)), 
            ComponentType.ReadWrite(typeof(Animator))},
                Any = new ComponentType[] { ComponentType.ReadWrite(typeof(CharControllerE)), ComponentType.ReadWrite(typeof(BeastControllerComponent)) }
            });
            withTag = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(LocalTransform)), ComponentType.ReadWrite(typeof(Animator)), ComponentType.ReadOnly(typeof(AnimateTag))},
                None = new ComponentType[] { ComponentType.ReadOnly(typeof(Player_Control)) },
                Any = new ComponentType[] { ComponentType.ReadWrite(typeof(CharControllerE)), ComponentType.ReadWrite(typeof(BeastControllerComponent)) }

            });

            withoutTag = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[] { ComponentType.ReadWrite(typeof(LocalTransform)), ComponentType.ReadWrite(typeof(Animator)), },
                None = new ComponentType[] { ComponentType.ReadOnly(typeof(AnimateTag)) },
                Any = new ComponentType[] { ComponentType.ReadWrite(typeof(CharControllerE)), ComponentType.ReadWrite(typeof(BeastControllerComponent)) }


            });

        }
        protected override void OnUpdate()
        {
            if(Query.CalculateEntityCount()==0)return;
            base.OnUpdate();
            ManageAnimatorTags();
        }

        private void ManageAnimatorTags()
        {
            var test = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.DefaultGameObjectInjectionWorld.Unmanaged);
            if (SystemAPI.TryGetSingletonEntity<Player_Control>(out Entity entityPlayer))
            {
                var playerPosition = EntityManager.GetComponentData<LocalToWorld>(entityPlayer).Position;
                var systemDeps = Dependency;
                systemDeps = new AnimatorAddJob()
                {
                    HashKey = GetPositionHashMapKey((int3)playerPosition),
                    Writer = test.AsParallelWriter()
                }.ScheduleParallel(withoutTag, systemDeps);
                systemDeps.Complete();
                systemDeps = new AnimatorRemoveJob()
                {
                    HashKey = GetPositionHashMapKey((int3)playerPosition),
                    Writer = test.AsParallelWriter()
                }.ScheduleParallel(withTag, systemDeps);

                systemDeps.Complete();
                Dependency = systemDeps;
            }
        }

        [BurstCompile]
        private partial struct AnimatorAddJob : IJobEntity
        {

            public int HashKey;
            public EntityCommandBuffer.ParallelWriter Writer;

            private void Execute(Entity entity, [EntityIndexInChunk] int index, [ReadOnly] in LocalTransform transform)
            {
                if (HashKey == GetPositionHashMapKey((int3)transform.Position))
                {
                    Writer.AddComponent(index, entity, new AnimateTag());
                }
            }
        }

        [BurstCompile]
        private partial struct AnimatorRemoveJob : IJobEntity
        {

            public int HashKey;
            public EntityCommandBuffer.ParallelWriter Writer;
            private void Execute(Entity entity, [EntityIndexInChunk] int index, [ReadOnly] in LocalTransform transform)
            {

                if (HashKey != GetPositionHashMapKey((int3)transform.Position))
                {
                    Writer.RemoveComponent<AnimateTag>(index, entity);
                }
            }
        }
    }
    public struct AnimateTag : IComponentData { }

}