namespace DreamersInc.Trackers
{
    public class CountdownCounter : Counter
    {
        public CountdownCounter(int initialValue) : base(initialValue)
        {
        }

        public override void Increment()
        {
            if (IsCounting && CurrentValue>0)
                CurrentValue--;
            if (IsCounting && CurrentValue <= 0)
                Stop();
        }

        public override bool IsFinished => CurrentValue == 0;
    }
}