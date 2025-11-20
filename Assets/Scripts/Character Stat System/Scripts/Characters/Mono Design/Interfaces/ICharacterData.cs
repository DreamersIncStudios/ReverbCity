namespace Stats
{
    public interface ICharacterData
    {
        public int Level { get; }
        public int Strength { get; }
        public int Vitality { get; }
        public int Skill { get; }
        public int Concentration { get; }
        public int WillPower { get; }
        public int Luck { get; }
        public int BaseHealth { get; }
        public int BaseMana { get; }
        //todo review if setters are needed
        public float LevelMod { get; set; }
    }
}