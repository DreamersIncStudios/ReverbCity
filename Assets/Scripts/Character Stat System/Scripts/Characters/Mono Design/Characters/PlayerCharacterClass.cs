using Unity.Mathematics;
using UnityEngine;

namespace Stats
{
    [System.Serializable]
    public class PlayerCharacterClass : ICharacterData
    {
        [SerializeField] private int level;
        [SerializeField] private int strength;
        [SerializeField] private int vitality;
        [SerializeField] private int skill;
        [SerializeField] private int concentration;
        [SerializeField] private int willPower;
        [SerializeField] private int luck;
        [SerializeField] private int baseHealth;
        [SerializeField] private int baseMana;
        [SerializeField] private float difficultyMod;
        [SerializeField] private float levelMod;

  
        public int Level => level;
        public int Strength => strength;
        public int Vitality => vitality;

        public int Skill => skill;

        public int Concentration => concentration;
        public int WillPower => willPower;

        public int Luck => luck;
        public int BaseHealth => baseHealth;
        public int BaseMana =>baseMana;
        public float DifficultyMod
        {
            get => difficultyMod;
            set => difficultyMod = value;
        }

        public float LevelMod
        {
            get => levelMod;
            set => levelMod = value;
        }
    }
}