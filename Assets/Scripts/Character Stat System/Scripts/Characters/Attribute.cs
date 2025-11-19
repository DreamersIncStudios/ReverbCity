using UnityEngine.Serialization;

namespace Stats
{
    [System.Serializable]
    public class Attributes : BaseStat
    {
        public Attributes()
        {
            BaseValue = 0;
            ExpToLevel = 50;
            LevelModifier = 1.05f;
        }
    }

    public enum AttributeName
    {
        Level,
        Strength, // All values derived from Strength will get a 10x modifier
        Vitality,
        Skill,
        WillPower, //copy of magic 
        Concentration, // Copy of spirit
        Luck
    }
    [System.Serializable]
    public struct AttributeModifier
    {
        [FormerlySerializedAs("Stat")] public AttributeName Attribute;
        public int BuffValue;

    }
}