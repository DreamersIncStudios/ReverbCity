using DreamersInc;
using DreamersIncStudio.FactionSystem;
using DreamersIncStudio.FactionSystem.Authoring;
using IAUS.ECS.Component;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace AISenses.VisionSystems
{
    [UpdateInGroup(typeof(VisionTargetingUpdateGroup))]
    [UpdateAfter(typeof(TargetingQuadrantSystem))]
    public partial class CheckTargetThreatSystem : SystemBase
    {
        Entity factionSingleton;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<FactionSingleton>();
        }

        protected override void OnUpdate()
        {
            var factionsBuffer = SystemAPI.GetSingletonBuffer<Factions>();
            var depends = Dependency;
            depends = new CheckBuffers()
            {
                FactionsBuffer = factionsBuffer
            }.Schedule(depends);

            Dependency = depends;
        }
        partial struct CheckBuffers : IJobEntity
        {
            [ReadOnly] public DynamicBuffer<Factions> FactionsBuffer;

            void Execute(
                DynamicBuffer<Enemies> enemies,
                DynamicBuffer<Allies> allies,
                DynamicBuffer<PlacesOfInterest> placesOfInterests,
                DynamicBuffer<Resources> resources,
                ref Vision brain)
            {
                var relationshipsForBrain = GetRelationshipsForBrain((FactionNames)brain.factionID);

                UpdateTargetAffinity(enemies, relationshipsForBrain);
                UpdateTargetAffinity(allies, relationshipsForBrain);
                UpdateTargetAffinity(resources, relationshipsForBrain);
                UpdateTargetAffinity(placesOfInterests, relationshipsForBrain);
            }

            // ... existing code ...

            private FixedList512Bytes<Relationship> GetRelationshipsForBrain(FactionNames factionId)
            {
                foreach (var factions in FactionsBuffer)
                {
                    if (factions.Faction == factionId)
                    {
                        return factions.Relationships;
                    }
                }
                return default;
            }

            private static void UpdateTargetAffinity<T>(DynamicBuffer<T> buffer, FixedList512Bytes<Relationship> relationships)
                where T : unmanaged, IBufferElementData, AISenses.IInteractable
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    var item = buffer[i];
                    var target = item.Target; // property from IInteractable

                    foreach (var relationship in relationships)
                    {
                        if (relationship.Faction != target.TargetInfo.FactionID) continue;

                        target.Affinity = ComputeAffinity(relationship.Affinity);
                        item.Target = target;
                        buffer[i] = item; // safe for Enemies/Allies/Resources/PlacesOfInterest via implicit ops
                        break;
                    }
                }
            }

            private static Affinity ComputeAffinity(int relationshipAffinity)
            {
                return relationshipAffinity switch
                {
                    < -75 => Affinity.Hate,
                    > -75 and < -35 => Affinity.Negative,
                    > -35 and < 35 => Affinity.Neutral,
                    > 35 and < 74 => Affinity.Positive,
                    > 75 => Affinity.Love,
                    _ => Affinity.Neutral
                };
            }
        }
    }
}
