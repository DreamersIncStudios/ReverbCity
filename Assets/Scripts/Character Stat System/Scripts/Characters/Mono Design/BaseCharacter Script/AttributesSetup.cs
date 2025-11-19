using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Entities;
using System.Threading.Tasks;
using DreamersInc.DamageSystem.Interfaces;
namespace Stats
{
    public abstract partial class BaseCharacter : MonoBehaviour
    {

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
                new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Strength), 1.5f));
            GetStat((int)StatName.MeleeOffence).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Skill), 1.250f));
            GetStat((int)StatName.MeleeOffence).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Level), 3.0f));
            GetStat((int)StatName.MeleeDefense).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Vitality), 1));


            GetStat((int)StatName.MagicOffence).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .5f));
            GetStat((int)StatName.MagicOffence).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), .5f));

            GetStat((int)StatName.MagicDefense).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Strength), .2f));

            GetStat((int)StatName.RangedOffence)
                .AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Concentration),
                    .33f));
            // Recovery Rates for Mana;

            GetStat((int)StatName.ManaRecover).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), .25f));
            GetStat((int)StatName.ManaRecover).AddDefiningAttribute(new BaseDefiningAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .25f));
        }

        public void SetupStatsModifiers()
        {
            //Need to Update with Calculation based on FFXV and FFXIII
            GetStat((int)StatName.MeleeOffence).AddModifier(
                new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Strength), 1.5f));
            GetStat((int)StatName.MeleeOffence).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Skill), 1.250f));
            GetStat((int)StatName.MeleeOffence).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Level), 3.0f));
            GetStat((int)StatName.MeleeDefense).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Vitality), 1));


            GetStat((int)StatName.MagicOffence).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Concentration), .5f));
            GetStat((int)StatName.MagicOffence).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.WillPower), .5f));

            GetStat((int)StatName.MagicDefense).AddModifier(new ModifyingAttribute(GetPrimaryAttribute((int)AttributeName.Strength), .2f));
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

        public async void SetAttributeBaseValue(int level, int BaseHealth, int BaseMana, int Str, int vit, 
            int Skl,  int Con, int Will, int Lck)
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
