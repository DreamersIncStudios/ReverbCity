using System;
using Dreamers.InventorySystem.Interfaces;
using UnityEngine;

namespace Dreamers.InventorySystem.AbilitySystem
{
    
    
    [Serializable]
    public class SpellDisplay : ISpells
    {
        public uint Level => level;
        public School School => school;
        public float Duration => duration;
        public uint Range => range;
        public uint AreaOfEffect => areaOfEffect;
        public DamageType DamageType => damageType;

        public uint ManaCost
        {
            get
            {
                var schoolModifier = GetSchoolModifier(school);
                return CalculateManaCost(level, schoolModifier, CalculateAreaOfEffectModifier);
            }
        }

        private uint GetSchoolModifier(School school)
        {
            return school switch
            {
                School.Abjuration => 1,
                School.Conjuration => 1,
                School.Divination => 1,
                School.Enchantment => 1,
                School.Evocation => 1,
                School.Illusion => 1,
                School.Necromancy => 1,
                School.Transmutation => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(school), school, "Invalid school type")
            };
        }

        private const float AoeDivisionFactor = 2f;
        private uint CalculateAreaOfEffectModifier=>(uint)Mathf.RoundToInt(AreaOfEffect / AoeDivisionFactor);
        

        private uint CalculateManaCost(uint level, uint schoolModifier, uint areaOfEffectModifier)
        {
            return (level * 2 + 1 + schoolModifier + areaOfEffectModifier);
        }

        [SerializeField] private uint level;
        [SerializeField] private School school;
        [SerializeField] private float duration;
        [SerializeField] private uint range;
        [SerializeField] private uint areaOfEffect;
        [SerializeField] private DamageType damageType;
    }

}