using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Entities;
using System.Threading.Tasks;
using DreamersInc.DamageSystem.Interfaces;
using CharacterClass = Stats.Entities;

namespace Stats.Entities
{
    public partial class BaseCharacterComponent : IComponentData
    {

        public void SetupDataEntity(ICharacterData BaseStats, string name, uint Exp=0,SerializableGuid spawnID = new SerializableGuid())
        {
            Init();
            Name = name;
            this.Level = BaseStats.Level;
            float ModValue = BaseStats.LevelMod;
            this.SpawnID = spawnID;
            this.BaseExp = Exp;
            this.GetPrimaryAttribute((int)AttributeName.Strength).BaseValue = (int)(BaseStats.Strength * ModValue);
            this.GetPrimaryAttribute((int)AttributeName.WillPower).BaseValue = (int)(BaseStats.WillPower * ModValue);
            this.GetPrimaryAttribute((int)AttributeName.Vitality).BaseValue = (int)(BaseStats.Vitality * ModValue);
            this.GetPrimaryAttribute((int)AttributeName.Skill).BaseValue = (int)(BaseStats.Skill * ModValue);
            this.GetPrimaryAttribute((int)AttributeName.Luck).BaseValue = (int)(BaseStats.Luck * ModValue);
            this.GetPrimaryAttribute((int)AttributeName.Concentration).BaseValue = (int)(BaseStats.Concentration * ModValue);
            this.GetVital((int)VitalName.Health).StartValue = BaseStats.BaseHealth;
            this.GetVital((int)VitalName.Mana).StartValue = BaseStats.BaseMana;
            StatUpdate();
        }


        private void SetupVitalBase()
        {
            //health
            GetVital((int)VitalName.Health).AddDefiningAttribute(
                new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Vitality), 3f)
            );
            GetVital((int)VitalName.Health).AddDefiningAttribute(
                new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Level), 10.0f)
                );
            GetVital((int)VitalName.Health).AddDefiningAttribute(
               new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Luck), .5f)
               );


            //energy
            GetVital((int)VitalName.Energy).AddDefiningAttribute(
                new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), 1)
            );

            //mana
            GetVital((int)VitalName.Mana).AddDefiningAttribute(
                new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), 2.5f)
            );
            GetVital((int)VitalName.Mana).AddDefiningAttribute(
                new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), 1.75f)
                );
        }


        private void SetupVitalModifiers()
        {
            //health
            GetVital((int)VitalName.Health).AddModifier(
                new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Vitality), 3f)
            );
            GetVital((int)VitalName.Health).AddModifier(
                new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Level), 10.0f)
                );
            GetVital((int)VitalName.Health).AddModifier(
               new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Luck), .5f)
               );


            //energy
            GetVital((int)VitalName.Energy).AddModifier(
                new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), 1)
            );

            //mana
            GetVital((int)VitalName.Mana).AddModifier(
                new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), 2.5f)
            );
            GetVital((int)VitalName.Mana).AddModifier(
                new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), 1.75f)
                );
        }

        public void SetupStatsBase()
        {
            //Need to Update with Calculation based on FFXV and FFXIII
            GetStat((int)StatName.MeleeOffence).AddDefiningAttribute(
                new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Strength), 15f));
            GetStat((int)StatName.MeleeOffence).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Skill), 1.250f));
            GetStat((int)StatName.MeleeOffence).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Level), 3.0f));
            GetStat((int)StatName.MeleeDefense).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Vitality), 1));


            GetStat((int)StatName.MagicOffence).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .5f));
            GetStat((int)StatName.MagicOffence).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), .5f));

            GetStat((int)StatName.MagicDefense).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Strength), 2f));

            GetStat((int)StatName.RangedOffence).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .33f));
            // Recovery Rates for Mana;

            GetStat((int)StatName.ManaRecover).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), .25f));
            GetStat((int)StatName.ManaRecover).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .25f));
        }

        public void SetupStatsModifiers()
        {
            //Need to Update with Calculation based on FFXV and FFXIII
            GetStat((int)StatName.MeleeOffence).AddModifier(
                new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Strength), 15f));
            GetStat((int)StatName.MeleeOffence).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Skill), 1.250f));
            GetStat((int)StatName.MeleeOffence).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Level), 3.0f));
            GetStat((int)StatName.MeleeDefense).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Vitality), 1));


            GetStat((int)StatName.MagicOffence).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .5f));
            GetStat((int)StatName.MagicOffence).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), .5f));

            GetStat((int)StatName.MagicDefense).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Strength), 2f));

            GetStat((int)StatName.RangedOffence).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .33f));

            GetStat((int)StatName.ManaRecover).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), .25f));
            GetStat((int)StatName.ManaRecover).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .25f));
        }

        public void SetupAbilitesBase()
        {
            GetAbility((int)AbilityName.Detection).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Skill), 2.75f));
            GetAbility((int)AbilityName.Detection).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), 2.75f));
            GetAbility((int)AbilityName.Detection).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Luck), 2.15f));

        }


        public void SetupAbilitesModifiers()
        {
            GetAbility((int)AbilityName.Detection).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Skill), 2.75f));
            GetAbility((int)AbilityName.Detection).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), 2.75f));
            GetAbility((int)AbilityName.Detection).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Luck), 2.15f));



        }

        public async void SetAttributeBaseValue(int level, int BaseHealth, int BaseMana, int Str, int vit, int Awr, int Spd, int Skl, int Res, int Con, int Will, int Chars, int Lck)
        {
            _level = GetPrimaryAttribute((int)AttributeName.Level).BaseValue = level;
            GetPrimaryAttribute((int)AttributeName.Strength).BaseValue = Str;
            GetPrimaryAttribute((int)AttributeName.Vitality).BaseValue = vit;
            GetPrimaryAttribute((int)AttributeName.Skill).BaseValue = Skl;
            GetPrimaryAttribute((int)AttributeName.Concentration).BaseValue = Con;
            GetPrimaryAttribute((int)AttributeName.WillPower).BaseValue = Will;
            GetPrimaryAttribute((int)AttributeName.Luck).BaseValue = Lck;
            GetVital((int)VitalName.Health).BuffValue = BaseHealth;
            GetVital((int)VitalName.Mana).BuffValue = BaseMana;
            await Task.Delay(TimeSpan.FromSeconds(2));
            StatUpdate();
        }



    }
}
