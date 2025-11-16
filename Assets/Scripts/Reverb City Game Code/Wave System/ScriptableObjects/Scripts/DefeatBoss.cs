using System;
using DreamersInc.WaveSystem.interfaces;

namespace DreamersInc.WaveSystem
{
    public class DefeatBoss: WaveRule
    {
  
        public override void ResetWave()
        {
            throw new NotImplementedException();
            
        }

        public override void Tick()
        {
            throw new NotImplementedException();
        }

        public override void IncrementDefeat(int value = 1)
        {
            throw new NotImplementedException();
        }
        public override void FailCheck()
        {
            throw new NotImplementedException();
        }

        public override bool IsFinished { get; }
        public override void PassedTrial()
        {
            throw new NotImplementedException();
        }

        public override void FailTrial()
        {
            throw new NotImplementedException();
        }
    }
}
