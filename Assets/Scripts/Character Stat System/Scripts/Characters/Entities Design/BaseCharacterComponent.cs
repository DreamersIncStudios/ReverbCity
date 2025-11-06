using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Properties;
using UnityEngine;


namespace Stats.Entities
{
    public partial class BaseCharacterComponent : IComponentData
    {
        private string _name;
        public int _level;
        public bool Clone;
        private Attributes[] _primaryAttribute;
        private Vital[] _vital;
        private Stat[] _stats;
        private Abilities[] _ability;
        private ElementDamageModStat[] elementalDamageMods;
        public bool InPlay;
        public bool InvincibleMode;
        public uint SpawnID;// { get; private set; }
        [HideInInspector] public GameObject GORepresentative;
        [HideInInspector] public uint CharacterID;
        [Range(0, 9999)]
        [SerializeField]
        private int curHealth;

        public bool HealthCantDropBelow;
        public uint HealthLimit;
        
        public event EventHandler<OnStatChangeEventArgs> OnStatChanged;
        public class OnStatChangeEventArgs : EventArgs
        {
            public BaseCharacterComponent Stats;

            public OnStatChangeEventArgs(BaseCharacterComponent stats)
            {
                Stats = stats;
            }
        }
        
        public int CurHealth
        {
            get => curHealth;
            set
            {

                if (value <= 0)
                    curHealth = 0;
                else if (value > MaxHealth && MaxHealth != 0)
                    curHealth = MaxHealth;
                else
                    curHealth = value;
            }
        }

        [Range(0, 9999)]
        [SerializeField] int maxHealth;

        public int MaxHealth { get { return MaxHealthMod + maxHealth; } set { maxHealth = value; } }
        public int MaxHealthMod { get; set; }
        [Range(0, 9999)]
        [SerializeField] int _curMana;

        public int CurMana
        {
            get { return _curMana; }
            set
            {

                if (value <= 0)
                    _curMana = 0;
                else if (value > MaxMana && MaxHealth != 0)
                    _curMana = MaxMana;
                else
                    _curMana = value;
            }
        }
        [Range(0, 9999)]
        [SerializeField] int maxMana;
        public int MaxMana { get { return maxMana + MaxHealthMod; } set { maxMana = value; } }
        public int MaxManaMod { get; set; }

        public float HealthRatio => (float)CurHealth / (float)MaxHealth;
        public float ManaRatio => (float)CurMana / (float)MaxMana;

        public bool Dead => !InvincibleMode && CurHealth <= 0;
        public uint BaseExp { get; protected set; }
        public uint ExpGiven(uint playerLevel)
        {
            var mod = Mathf.Pow((2 * Level + 10) / (Level + playerLevel + 10), 2.5f);
            return (uint)Mathf.CeilToInt((BaseExp * _level)*.2f *mod );
        }

        [CreateProperty]
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        [CreateProperty]

        public int Level
        {
            get => _level;
            private set => _level = value;
        }
        public uint FreeExp { get; private set; }

        public void AddExp(uint exp)
        {
            FreeExp += exp;
            CalculateLevel();
        }


        public int ExpToNextLevel => Mathf.FloorToInt(1.2f* Mathf.Pow(Level, 3) - 15 * Mathf.Pow(Level, 2) + 100 * Level - 140);

        public int ExpTilNextLevel => ExpToNextLevel - (int)FreeExp;

        private void CalculateLevel()
        {
            if (ExpTilNextLevel>0) return;
            FreeExp = 0;
            Level++;
            StatUpdate();
        }

        private void Init()
        {
            _name = string.Empty;
            _level = 0;
            FreeExp = 0;
            _primaryAttribute = new Attributes[Enum.GetValues(typeof(AttributeName)).Length];
            _vital = new Vital[Enum.GetValues(typeof(VitalName)).Length];
            _stats = new Stat[Enum.GetValues(typeof(StatName)).Length];
            _ability = new Abilities[Enum.GetValues(typeof(AbilityName)).Length];
            elementalDamageMods = new ElementDamageModStat[Enum.GetValues(typeof(ElementName)).Length];
            
            SetupPrimaryAttributes();
            SetupVitals();
            SetupStats();
            SetupAbilities();

            CurHealth = MaxHealth;
            CurMana = MaxMana;
#if !UNITY_EDITOR
            InvincibleMode = false;
#endif

#if UNITY_EDITOR
            if (InvincibleMode)
                Debug.LogWarning($"This Character {Name} is in Invincible Mode and will not take Damage");
#endif

            // SetupElementalMods();
        }

        private void SetupPrimaryAttributes()
        {
            for (int cnt = 0; cnt < _primaryAttribute.Length; cnt++)
            {
                _primaryAttribute[cnt] = new Attributes();
            }
        }
        public Attributes GetPrimaryAttribute(int index)
        {
            return _primaryAttribute[index];
        }
        private void SetupVitals()
        {
            for (int cnt = 0; cnt < _vital.Length; cnt++)
                _vital[cnt] = new Vital();

            SetupVitalBase();
            SetupVitalModifiers();
        }
        public Vital GetVital(int index)
        {
            return _vital[index];
        }
        public Stat GetStat(int index)
        {
            return _stats[index];
        }
        public Abilities GetAbility(int index)
        {
            return _ability[index];
        }

        public ElementDamageModStat GetElementMod(int index)
        {
            return elementalDamageMods[index];
        }

        public ElementDamageModStat GetElementMod( ElementName elementName)
        {
            
            return elementalDamageMods[(int)elementName];
        }
        private void SetupStats()
        {
            for (int cnt = 0; cnt < _stats.Length; cnt++)
                _stats[cnt] = new Stat();
            SetupStatsBase();
            SetupStatsModifiers();
        }
        private void SetupAbilities()
        {
            for (int cnt = 0; cnt < _ability.Length; cnt++)
                _ability[cnt] = new Abilities();
            SetupAbilitesBase();
            SetupAbilitesModifiers();
        }

        public void StatUpdate()
        {
            foreach (var vital in _vital)
                vital.Update();

            foreach (var stat in _stats)
                stat.Update();

            foreach (var ability in _ability)
                ability.Update();

            CurHealth = MaxHealth = GetVital((int)VitalName.Health).AdjustBaseValue;
            CurMana = MaxMana = GetVital((int)VitalName.Mana).AdjustBaseValue;
            OnStatChanged?.Invoke(this,new OnStatChangeEventArgs(this));
        }

        public void AdjustHealth(int adj)
        {
            if (InvincibleMode) return;
            if (HealthCantDropBelow && CurHealth < HealthLimit) 
                return;
            CurHealth += adj;
            if (CurHealth < 0)
            {
                CurHealth = 0;
                Debug.Log("dead");
            }
            if (HealthCantDropBelow && CurHealth < HealthLimit) 
                curHealth = (int)HealthLimit;
            if (CurHealth > MaxHealth) { CurHealth = MaxHealth; }

        }
        public void AdjustMana(int adj)
        {
            CurMana += adj;
            if (CurMana < 0) { CurMana = 0; }
            if (CurMana > MaxMana) { CurMana = MaxMana; }

        }
  
    }
}