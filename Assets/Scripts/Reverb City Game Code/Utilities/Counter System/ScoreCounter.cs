namespace DreamersInc.Trackers
{
    public class ScoreCounter : Counter
    {
        public ScoreCounter(int initialValue) : base(initialValue)
        {
        }

        public override void Increment(int value = 1)
        {
            if (IsCounting)       
                CurrentValue += value;
        }

        public override bool IsFinished => false;
    }
}
