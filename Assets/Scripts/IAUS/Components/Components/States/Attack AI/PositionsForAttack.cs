using System;
using System.Collections.Generic;
using System.Linq;
using DreamersInc.InfluenceMapSystem;
using DreamersIncStudio.GAIACollective;
using Global.Component;
using IAUS.ECS.Systems;
using IAUS.ECS.Systems.Reactive;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace IAUS.ECS.Component.Attacking
{
    public struct AttackPosition : IBufferElementData
    {
      
        public float3 Offset;
        public OccupiedState State;
        public readonly RangeType Range;
        public readonly Entity TargetEntity;
        public Entity AttackerEntity;
        public AttackPosition(float3 buildRequestOffset, OccupiedState vacant, RangeType buildRequestRange, Entity buildRequestTargetEntity)
        {
            Offset = buildRequestOffset;
            State = vacant;
            Range = buildRequestRange;
            TargetEntity = buildRequestTargetEntity;
            AttackerEntity = Entity.Null;
        }



    }

    public enum OccupiedState
    {
        Vacant,
        Reserved,
        Occupied
    }
    public enum RangeType
    {
        Close, Near, Mid, Far
    }

    public partial struct AttackPositionManagementSystem : ISystem
    {
        ComponentLookup<LocalToWorld> TransformLookup;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RunningTag>();
            TransformLookup = state.GetComponentLookup<LocalToWorld>(true);
        }
        public void OnDestroy(ref SystemState state)
        {

        }
        public void OnUpdate(ref SystemState state)
        {
            TransformLookup.Update(ref state);
            var depends = state.Dependency;
            depends = new AreAttackPositionsStillValid().Schedule(depends);
         
            depends = new CreateAttackPositions()
            {
                TransformLookup = TransformLookup
            }.Schedule(depends);
            state.Dependency = depends;
        }

        partial struct AreAttackPositionsStillValid : IJobEntity
        {
            void Execute(DynamicBuffer<PackTargets> targets, DynamicBuffer<AttackPosition> attackPositions)
            {
                if (targets.IsEmpty || attackPositions.IsEmpty)
                    return;
                for (int i = 0; i < attackPositions.Length; i++)
                {
                    foreach (var target in targets)
                    {
                        if (target.Target.Equals(attackPositions[i].TargetEntity))
                            break;
                    }
                    attackPositions.RemoveAt(i);
                }
            }
        }
      
        partial struct CreateAttackPositions : IJobEntity
        {
            [ReadOnly] public ComponentLookup<LocalToWorld> TransformLookup;

            // Centralized range definitions for readability
            private static readonly Dictionary<RangeType, (int Count, float Radius)> RangeDefinitions = new()
            {
                { RangeType.Close, (4, 2f) },
                { RangeType.Near, (8, 5f) },
                { RangeType.Mid, (16, 10f) },
                { RangeType.Far, (32, 20f) }
            };

            void Execute(DynamicBuffer<PackTargets> targets, DynamicBuffer<AttackPosition> attackPositions)
            {
                if (targets.IsEmpty) return;

                var requests = new List<BuildRequest>();

                foreach (var target in targets)
                {
                    if (IsAttackPositionIncluded(target.Target, attackPositions)) continue;
                    requests.Add(new BuildRequest(target.Target, TransformLookup[target.Target].Position));
                }

                foreach (var request in requests)
                {
                    foreach (RangeType rangeType in Enum.GetValues(typeof(RangeType)))
                    {
                        var positions = GenerateAttackPositions(request.Position, request.TargetEntity, rangeType);
                        foreach (var position in positions)
                        {
                            attackPositions.Add(position);
                        }
                    }
                }
            }

            private bool IsAttackPositionIncluded(Entity target, DynamicBuffer<AttackPosition> attackPositions)
            {
                foreach (var position in attackPositions)
                {
                    if (position.TargetEntity.Equals(target))
                        return true;
                }
                return false;
            }

            private List<AttackPosition> GenerateAttackPositions(float3 targetPosition, Entity targetEntity, RangeType range)
            {
                var (count, radius) = RangeDefinitions[range];
                var angleIncrement = 360f / count;
                var positions = new List<AttackPosition>(count);

                for (int i = 0; i < count; i++)
                {
                    float angle = math.radians(i * angleIncrement);
                    var offset = new float3(math.cos(angle) * radius, 0, math.sin(angle) * radius);
                    positions.Add(new AttackPosition(targetPosition + offset, OccupiedState.Vacant, range, targetEntity));
                }
                return positions;
            }

            private struct BuildRequest
            {
                public readonly float3 Position;
                public readonly Entity TargetEntity;

                public BuildRequest(Entity targetEntity, float3 position)
                {
                    TargetEntity = targetEntity;
                    Position = position;
                }
            }
        }
    }
}
