using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

namespace Stats
{
    public class CharacterSetup : MonoBehaviour
    {
        public ICharacterData CharClass;

        private void Awake()
        {
            if (CharClass.LevelMod == 0)
                CharClass.LevelMod = 1;

            if (CharClass.DifficultyMod == 0)
                CharClass.DifficultyMod = 1;

        }

        public void StatsUpdate(BaseCharacter CharacterStats)
        {
            CharacterStats.Level = CharClass.Level;
            float ModValue = CharClass.LevelMod;
            CharacterStats.GetPrimaryAttribute((int)AttributeName.Strength).BaseValue = (int)(CharClass.Strength * ModValue);
            CharacterStats.GetPrimaryAttribute((int)AttributeName.WillPower).BaseValue = (int)(CharClass.WillPower * ModValue);
            CharacterStats.GetPrimaryAttribute((int)AttributeName.Vitality).BaseValue = (int)(CharClass.Vitality * ModValue);
            CharacterStats.GetPrimaryAttribute((int)AttributeName.Skill).BaseValue = (int)(CharClass.Skill * ModValue);
            CharacterStats.GetPrimaryAttribute((int)AttributeName.Luck).BaseValue = (int)(CharClass.Luck * ModValue);
            CharacterStats.GetPrimaryAttribute((int)AttributeName.Concentration).BaseValue = (int)(CharClass.Concentration * ModValue);
            CharacterStats.GetVital((int)VitalName.Health).StartValue = 500;
            CharacterStats.GetVital((int)VitalName.Mana).StartValue = 250;


            CharacterStats.StatUpdate();

        }
    }


}
