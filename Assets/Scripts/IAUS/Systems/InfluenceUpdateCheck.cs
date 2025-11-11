using DreamersInc.InfluenceMapSystem;
using DreamersIncStudio.FactionSystem;
using DreamersIncStudio.FactionSystem.Authoring;
using IAUS.ECS.Component;
using IAUS.ECS.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace DreamersIncStudio.InfluenceMapSystem
{
    [UpdateBefore(typeof(IAUSBrainUpdate))]
    [UpdateInGroup(typeof(IAUSUpdateGroup))]
    public partial class InfluenceUpdateCheck : SystemBase
    {
        [ReadOnly] DynamicBuffer<Factions> factionsBuffer;
        Entity factionSingleton;
        public EntityQuery query;



        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<FactionSingleton>();
            RequireForUpdate<RunningTag>();
            //Todo add running Tag require for update
            query = SystemAPI.QueryBuilder().WithAll<InfluenceComponent, LocalToWorld>().Build();
        }

        protected override void OnUpdate()
        {
            RetrieveFactionsData();
            var depends = Dependency;
            depends = new InfluenceCalculationJob()
            {
                FactionsBuffer = factionsBuffer,
                positions =  query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob),
                influences = query.ToComponentDataArray<InfluenceComponent>(Allocator.TempJob)
            }.Schedule(depends);
            
            depends = new InfluenceUpdateCheckJob()
            {
                positions = query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob),
                influences = query.ToComponentDataArray<InfluenceComponent>(Allocator.TempJob),
                FactionsBuffer = factionsBuffer
            }.Schedule(depends);

            Dependency = depends;
        }

        private void RetrieveFactionsData()
        {
            factionsBuffer = SystemAPI.GetSingletonBuffer<Factions>();
        }


        [BurstCompile]
        partial struct InfluenceCalculationJob : IJobEntity
        {
            public DynamicBuffer<Factions> FactionsBuffer;
            void Execute(ref IAUSBrain influence, in LocalToWorld transform)
            {
                influence.InfluenceHere = CalculateNodeScore(transform.Position, influence.FactionID);
            }
                 [ReadOnly] public NativeArray<LocalToWorld> positions;
            [ReadOnly] public NativeArray<InfluenceComponent> influences;

            /// <summary>
            /// Calculates sector-based influence scores for a given position and faction.
            /// </summary>
            /// <param name="position">The position in the world space for which the sector score is calculated.</param>
            /// <param name="faction">The faction associated with the current calculation.</param>
            /// <returns>Returns an <see cref="int2"/> representing the influence scores for friendly and enemy factions in different sectors.</returns>
            private int2 SectorScore(float3 position, FactionNames faction)
            {

                var relationships = GetRelationship(faction);
                var sectorMask = int2.zero;

                for (var i = 0; i < positions.Length; i++)
                {
                    bool isFriendly = false;
                    foreach (var relationship in relationships)
                    {
                        if (relationship.Faction != influences[i].FactionID) continue;
                        if (relationship.Affinity <= 50) continue;
                        isFriendly = true;
                        break;
                    }

                    var dist = Vector3.Distance(positions[i].Position, position);
                    var direction = ((Vector3)position - (Vector3)positions[i].Position).normalized;
                    if (dist > influences[i].DetectionRadius) continue;
                    // if (IsObstructed(positions[i].Position, position + new float3(.5f, 0, .5f), entities[i])) continue;
                    int sector = GetSectorForDirection(direction);
                    int rangeValue = Mathf.FloorToInt(influences[i].InfluenceValue * GetRangeValue(dist, influences[i].DetectionRadius));
                    int sectorShift = sector * 4;
                    int currentSectorValue = isFriendly
                        ? (sectorMask.x >> sectorShift) & 0b1111
                        : (sectorMask.y >> sectorShift) & 0b1111;
                    int newSectorValue = Mathf.Min(15, currentSectorValue + rangeValue);
                    if (isFriendly)
                    {
                        sectorMask.x &= ~(0b1111 << sectorShift);
                        sectorMask.x |= newSectorValue << sectorShift;
                    }
                    else
                    {
                        sectorMask.y &= ~(0b1111 << sectorShift);
                        sectorMask.y |= newSectorValue << sectorShift;
                    }
                }

                return sectorMask;
            }

            private FixedList512Bytes<Relationship> GetRelationship(FactionNames faction)
            {
                foreach (var factions in FactionsBuffer)
                {
                    if (factions.Faction == faction) return factions.Relationships;
                }

                return default;
            }

            /// <summary>
            /// Calculates the score of a node based on its position and faction.
            /// </summary>
            /// <param name="position">The position of the node in the world space.</param>
            /// <param name="faction">The faction associated with the node.</param>
            /// <returns>Returns an <see cref="int2"/> value representing the calculated score or influence at the node.</returns>
            private int2 CalculateNodeScore(float3 position, FactionNames faction)
            {
                var sectorMask = SectorScore(position, faction);
                int2 danger = int2.zero;
                for (int sector = 0; sector < 8; sector++)
                {
                    danger += (sectorMask >> (sector * 4)) & 0b1111;
                }

                return danger;
            }



            int GetSectorForDirection(Vector3 direction)
            {
                return Mathf.FloorToInt((Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg + 360) % 360 / 45f);
            }

            float GetRangeValue(float distance, float detectionRadius)
            {
                if (distance < detectionRadius * 0.5f) return 1;
                if (distance < detectionRadius * 0.75f) return .65f;
                return distance <= detectionRadius ? .33f : 0;
            }

        }
        partial struct InfluenceUpdateCheckJob : IJobEntity
        {
            public DynamicBuffer<Factions> FactionsBuffer;
            void Execute(ref DestroyTargetTag state, in IAUSBrain influence)
            {
                state.InfluenceAtTarget = state.TargetPosition.Equals(float3.zero)
                    ? int2.zero
                    : CalculateNodeScore(state.TargetPosition, influence.FactionID);
            }

            [ReadOnly] public NativeArray<LocalToWorld> positions;
            [ReadOnly] public NativeArray<InfluenceComponent> influences;

            /// <summary>
            /// Calculates sector-based influence scores for a given position and faction.
            /// </summary>
            /// <param name="position">The position in the world space for which the sector score is calculated.</param>
            /// <param name="faction">The faction associated with the current calculation.</param>
            /// <returns>Returns an <see cref="int2"/> representing the influence scores for friendly and enemy factions in different sectors.</returns>
            private int2 SectorScore(float3 position, FactionNames faction)
            {

                var relationships = GetRelationship(faction);
                var sectorMask = int2.zero;

                for (var i = 0; i < positions.Length; i++)
                {
                    bool isFriendly = false;
                    foreach (var relationship in relationships)
                    {
                        if (relationship.Faction != influences[i].FactionID) continue;
                        if (relationship.Affinity <= 50) continue;
                        isFriendly = true;
                        break;
                    }

                    var dist = Vector3.Distance(positions[i].Position, position);
                    var direction = ((Vector3)position - (Vector3)positions[i].Position).normalized;
                    if (dist > influences[i].DetectionRadius) continue;
                    // if (IsObstructed(positions[i].Position, position + new float3(.5f, 0, .5f), entities[i])) continue;
                    int sector = GetSectorForDirection(direction);
                    int rangeValue = Mathf.FloorToInt(influences[i].InfluenceValue * GetRangeValue(dist, influences[i].DetectionRadius));
                    int sectorShift = sector * 4;
                    int currentSectorValue = isFriendly
                        ? (sectorMask.x >> sectorShift) & 0b1111
                        : (sectorMask.y >> sectorShift) & 0b1111;
                    int newSectorValue = Mathf.Min(15, currentSectorValue + rangeValue);
                    if (isFriendly)
                    {
                        sectorMask.x &= ~(0b1111 << sectorShift);
                        sectorMask.x |= newSectorValue << sectorShift;
                    }
                    else
                    {
                        sectorMask.y &= ~(0b1111 << sectorShift);
                        sectorMask.y |= newSectorValue << sectorShift;
                    }
                }

                return sectorMask;
            }

            private FixedList512Bytes<Relationship> GetRelationship(FactionNames faction)
            {
                foreach (var factions in FactionsBuffer)
                {
                    if (factions.Faction == faction) return factions.Relationships;
                }

                return default;
            }

            /// <summary>
            /// Calculates the score of a node based on its position and faction.
            /// </summary>
            /// <param name="position">The position of the node in the world space.</param>
            /// <param name="faction">The faction associated with the node.</param>
            /// <returns>Returns an <see cref="int2"/> value representing the calculated score or influence at the node.</returns>
            private int2 CalculateNodeScore(float3 position, FactionNames faction)
            {
                var sectorMask = SectorScore(position, faction);
                int2 danger = int2.zero;
                for (int sector = 0; sector < 8; sector++)
                {
                    danger += (sectorMask >> (sector * 4)) & 0b1111;
                }

                return danger;
            }



            int GetSectorForDirection(Vector3 direction)
            {
                return Mathf.FloorToInt((Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg + 360) % 360 / 45f);
            }

            float GetRangeValue(float distance, float detectionRadius)
            {
                if (distance < detectionRadius * 0.5f) return 1;
                if (distance < detectionRadius * 0.75f) return .65f;
                return distance <= detectionRadius ? .33f : 0;
            }

        }
    }

}
