using System.Collections.Generic;
using Combinators.Attack_AI;
using DreamersIncStudio.GAIACollective;
using DreamersIncStudio.GAIACollective.Squad_Base_AI_Tools;
using Global.Component;
using IAUS.ECS.Component;
using IAUS.ECS.Component.Attacking;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace IAUS.ECS.Systems
{
    [UpdateInGroup(typeof(AISenses.VisionSystems.VisionTargetingUpdateGroup))]
    public partial struct FindCoverSystem : ISystem
    {
        EntityQuery coverQuery;
        ComponentLookup<AITarget> ailookup;
        ComponentLookup<GlobalAITarget> globalAIlookup;
        ComponentLookup<PhysicsInfo> physicsInfoLookup;
        BufferLookup<MeleeAttackPosition> meleePositionLookup;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.EntityManager.CompleteDependencyBeforeRO<PhysicsWorldSingleton>();

            coverQuery = state.GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly(typeof(Cover)),
                    ComponentType.ReadOnly(typeof(LocalToWorld))
                }
            });
            ailookup = state.GetComponentLookup<AITarget>(true);
            globalAIlookup = state.GetComponentLookup<GlobalAITarget>(true);
            physicsInfoLookup = state.GetComponentLookup<PhysicsInfo>(true);
            meleePositionLookup = state.GetBufferLookup<MeleeAttackPosition>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            var depends = state.Dependency;
            ailookup.Update(ref state);
            globalAIlookup.Update(ref state);
            physicsInfoLookup.Update(ref state);
            meleePositionLookup.Update(ref state);
            state.EntityManager.CompleteDependencyBeforeRO<PhysicsWorldSingleton>();
            var world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            depends = new CoverJobAttack()
            {
                Covers = coverQuery.ToComponentDataArray<Cover>(Allocator.TempJob),
                AITargetInfo = ailookup,
                CoversPositions = coverQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob),
                World = world,
                PhysicsInfoLookup = physicsInfoLookup

            }.Schedule(depends);
            depends = new CoverJobTerror()
            {
                AITargetData = ailookup,
                Covers = coverQuery.ToComponentDataArray<Cover>(Allocator.TempJob),
                World = world,
                PhysicsInfoLookup = physicsInfoLookup

            }.Schedule(depends);

            depends = new CoverJobDestroy()
            {
                AITargetData = globalAIlookup,
                Covers = coverQuery.ToComponentDataArray<Cover>(Allocator.TempJob),
                CoversPositions = coverQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob),
                World = world,
                PhysicsInfoLookup = physicsInfoLookup

            }.Schedule(depends);
            depends = new PackAttackPositionLocator()
            {
                MeleeAttackPositions = meleePositionLookup
            }.Schedule(depends);

            state.Dependency = depends;
        }

        [WithNone(typeof(PackMember))]
        partial struct CoverJobAttack : IJobEntity
        {
            [ReadOnly] public CollisionWorld World;
            [ReadOnly] public NativeArray<Cover> Covers;
            [ReadOnly] public NativeArray<LocalToWorld> CoversPositions;
            [ReadOnly] public ComponentLookup<AITarget> AITargetInfo;
            [ReadOnly] public ComponentLookup<PhysicsInfo> PhysicsInfoLookup;
            void Execute(AttackActionTag aspect, in LocalToWorld transform, in Parent parent)
            {
                if (aspect.TargetEntity == Entity.Null) return;
                if (aspect.AttackPlans[0] != AttackPlan.GetAttackLocation) return;

                var physicsInfo = PhysicsInfoLookup[parent.Value];
                var offset = AITargetInfo[aspect.TargetEntity].CenterOffset;
                var ctx = new PositionCTX(transform.Position, transform.Forward, 75,
                    aspect.TargetPosition, offset, aspect.TargetEntity, World,
                    new CollisionFilter()
                    {
                        BelongsTo = ((1 << 11)),
                        CollidesWith = physicsInfo.CollidesWith.Value,
                        GroupIndex = 0
                    });

                var coverData = new List<CoverData>();

                for (int i = 0; i < Covers.Length; i++)
                {
                    foreach (var point in Covers[i].CoverPoints)
                        coverData.Add(new CoverData(point, Covers[i]));
                }

                var coverInRange = PredChain.Start(new InEffectiveAttackRange())
                    .And(new CanSeeTargetStage())
                    .Build();
                var ans = coverInRange.Test(coverData, in ctx);
            }
        }
        [WithNone(typeof(PackMember))]
        partial struct CoverJobTerror : IJobEntity
        {
            [ReadOnly] public CollisionWorld World;
            [ReadOnly] public NativeArray<Cover> Covers;
            [ReadOnly] public ComponentLookup<AITarget> AITargetData;
            [ReadOnly] public ComponentLookup<PhysicsInfo> PhysicsInfoLookup;
            void Execute(TerrorizeAspect aspect, in LocalToWorld transform, in Parent parent)
            {
                if (Covers.Length == 0) return;
                if (aspect.TargetEntity == Entity.Null) return;
                if (aspect.CurAttackStep != AttackPlan.GetAttackLocation) return;

                var physicsInfo = PhysicsInfoLookup[parent.Value];
                var offset = AITargetData[aspect.TargetEntity].CenterOffset;
                var ctx = new PositionCTX(transform.Position, transform.Forward, 75,
                    aspect.TargetPosition, offset, aspect.TargetEntity, World,
                    new CollisionFilter()
                    {
                        BelongsTo = ((1 << 11)),
                        CollidesWith = physicsInfo.CollidesWith.Value,
                        GroupIndex = 0
                    });

                var coverData = new List<CoverData>();
                foreach (var cover in Covers)
                {
                    foreach (var point in cover.CoverPoints)
                    {
                        coverData.Add(new CoverData(point, cover));
                    }

                }

                var coverInRange = PredChain.Start(new InEffectiveAttackRange())
                    .And(new CanSeeTargetStage())
                    .Build();
                var ans = coverInRange.Test(coverData, in ctx);
                Debug.Log(ans.Count);
            }
        }

        [WithNone(typeof(PackMember))]
        partial struct CoverJobDestroy : IJobEntity
        {
            [ReadOnly] public CollisionWorld World;
            [ReadOnly] public NativeArray<Cover> Covers;
            [ReadOnly] public NativeArray<LocalToWorld> CoversPositions;
            [ReadOnly] public ComponentLookup<GlobalAITarget> AITargetData;
            [ReadOnly] public ComponentLookup<PhysicsInfo> PhysicsInfoLookup;

            void Execute(DestroyTargetTag aspect, in LocalToWorld transform, in Parent parent)
            {
                if (aspect.AttackPlans[0] != AttackPlan.GetAttackLocation) return;
                if (aspect.TargetEntity == Entity.Null) return;
                var physicsInfo = PhysicsInfoLookup[parent.Value];

                var offset = AITargetData[aspect.TargetEntity].CenterOffset;
                var ctx = new PositionCTX(transform.Position, transform.Forward, 75,
                    aspect.TargetPosition, offset, aspect.TargetEntity, World,
                    new CollisionFilter()
                    {
                        BelongsTo = ((1 << 11)),
                        CollidesWith = physicsInfo.CollidesWith.Value,
                        GroupIndex = 0
                    });

                var coverData = new List<CoverData>();
                for (int i = 0; i < Covers.Length; i++)
                {
                    coverData.Add(new CoverData(CoversPositions[i].Position, Covers[i]));
                }
                if (coverData.Count == 0) return;
                var coverInRange = PredChain.Start(new InEffectiveAttackRange())
                    .And(new CanSeeTargetStage())
                    .Build();
                var ans = coverInRange.Test(coverData, in ctx);
            }
        }
    }
}
