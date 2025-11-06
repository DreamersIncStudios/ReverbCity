using System.Collections;
using Unity.Entities;
using DreamersInc.DamageSystem.Interfaces;
using DreamersInc.InfluenceMapSystem;
using DreamersIncStudio.FactionSystem;
using DreamersIncStudio.FactionSystem.Authoring;
using Stats;
using Stats.Entities;
using Unity.Collections;
using UnityEngine;

// ReSharper disable Unity.BurstLoadingManagedType
namespace DreamersInc.DamageSystem
{
    public partial class AdjustVitalSystems : SystemBase
    {
        DynamicBuffer<Factions> factionsBuffer;
        Entity factionSingleton;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<FactionSingleton>();
      

        }
        protected override void OnUpdate()
        {
            RetrieveFactionsData();
            var lookup = SystemAPI.GetComponentLookup<InfluenceComponent>(true);

            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity,BaseCharacterComponent character, in AdjustHealth mod, in InfluenceComponent influence) => {
              
                var affinity = GetAffinity(influence.FactionID,lookup[mod.DamageDealtByEntity].FactionID);
                //todo add check to see if friendly fire is allowed.
                if (affinity is Affinity.Positive or Affinity.Love)
                {
                    return;
                }

                character.AdjustHealth(mod.Value);
                
                if (character.CurHealth <= 0)
                {
                    EntityManager.AddComponent<EntityHasDiedTag>(entity);
                    EntityManager.AddComponentData(mod.DamageDealtByEntity, new AddXP(character.ExpGiven(mod.Level)));
                }
                EntityManager.RemoveComponent<AdjustHealth>(entity);

            }).Run();
            
            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity,BaseCharacterComponent character, in AdjustHealthRaw mod, in InfluenceComponent influence) => {
              
                var affinity = GetAffinity(influence.FactionID,lookup[mod.DamageDealtByEntity].FactionID);
                //todo add check to see if friendly fire is allowed.
                if (affinity is Affinity.Positive or Affinity.Love)
                {
                    return;
                }

                var defense = character.GetStat((int)StatName.MeleeDefense).AdjustBaseValue;
                
                
                var damageToProcess = -Mathf.FloorToInt(mod.Value * defense * Random.Range(.92f, 1.08f)*.75f);

                character.AdjustHealth(damageToProcess);
                
                if (character.CurHealth <= 0)
                {
                    EntityManager.AddComponent<EntityHasDiedTag>(entity);
                    EntityManager.AddComponentData(mod.DamageDealtByEntity, new AddXP(character.ExpGiven(mod.Level)));
                }
                EntityManager.RemoveComponent<AdjustHealth>(entity);

            }).Run();

            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity,BaseCharacterComponent mana, in AdjustMana mod) => {
                mana.AdjustMana(mod.Value);

                EntityManager.RemoveComponent<AdjustMana>(entity);
            }).Run();
            
            Entities.WithStructuralChanges().WithoutBurst().ForEach((Entity entity, BaseCharacterComponent stat,
                in AddXP xp) =>
            {
                stat.AddExp(xp.XP);
                EntityManager.RemoveComponent<AddXP>(entity);

            }).Run();

        }
        private Affinity GetAffinity(FactionNames faction, FactionNames targetFaction)
        {
            var relationships = GetRelationship(faction);
            foreach (var relationship in relationships)
            {
                if (relationship.Faction != targetFaction) continue;
                switch (relationship.Affinity)
                {
                    case < -75:
                        return Affinity.Hate;
                    case > -75 and < -35:
                        return Affinity.Negative;
                    case > -35 and < 35:
                        return Affinity.Neutral;
                    case > 35 and < 74:
                        return Affinity.Positive;
                    case > 75:
                        return Affinity.Love;
                }
            }
            return Affinity.Neutral;
        }

        private FixedList512Bytes<Relationship> GetRelationship(FactionNames faction)
        {
            foreach (var factions in factionsBuffer)
            {
                if (factions.Faction == faction) return factions.Relationships;
            }
            return default;
        }
        private void RetrieveFactionsData()
        {
            factionsBuffer = SystemAPI.GetSingletonBuffer<Factions>();
 
        }
    }
}