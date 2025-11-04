using ImprovedTimers;

namespace DreamersInc.Trackers
{
    public class RatingCounter : Counter
    {
        private CountdownTimer timer;
        public RatingCounter(int value) : base(value)
        {
            timer = new CountdownTimer(6.0f);
         
        }

        public override void Increment()
        {
            if (!timer.IsRunning)
            {
                this.Reset();
            }
            timer.Reset();
            timer.Start();
            CurrentValue++;
            
        }

        public override bool IsFinished => false;
    }
}