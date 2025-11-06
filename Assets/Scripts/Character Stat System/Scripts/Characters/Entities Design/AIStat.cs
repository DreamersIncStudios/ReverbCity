using System.Diagnostics.CodeAnalysis;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Stats.Entities
{
    public struct AIStat : IComponentData
    {
        public float CurHealth, MaxHealth, CurMana, MaxMana;
        public float HealthRatio => CurHealth / MaxHealth;
        public float ManaRatio => CurMana / MaxMana;
        public int Level;
        
    }

    public struct PackStatUpdate : IComponentData
    {
    };
    
    public struct AdjustPackCurHP : IComponentData
    {
        public float Amount;
    }
    public partial class AIStatLinkSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (aiStat, parent) in SystemAPI.Query<RefRW<AIStat>, Parent>())
            {
                var baseStat = EntityManager.GetComponentData<BaseCharacterComponent>(parent.Value);

                aiStat.ValueRW.CurHealth = baseStat.CurHealth;
                aiStat.ValueRW.MaxHealth = baseStat.MaxHealth;
                aiStat.ValueRW.CurMana = baseStat.CurMana;
                aiStat.ValueRW.MaxMana = baseStat.MaxMana;
                aiStat.ValueRW.Level = baseStat.Level;
            }

            foreach (var (aiStat, baseStat) in SystemAPI.Query<RefRW<AIStat>, BaseCharacterComponent>())
            {


                aiStat.ValueRW.CurHealth = baseStat.CurHealth;
                aiStat.ValueRW.MaxHealth = baseStat.MaxHealth;
                aiStat.ValueRW.CurMana = baseStat.CurMana;
                aiStat.ValueRW.MaxMana = baseStat.MaxMana;
                aiStat.ValueRW.Level = baseStat.Level;
            }

        }
    }
}
