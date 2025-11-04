using System;
using DreamersInc.ReverbCity;
using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;
using UnityEngine;

namespace DreamersInc.WaveSystem
{
    [CreateAssetMenu(menuName = "Wave Rules/Create TimeRule", fileName = "TimeRule", order = 0)]
    public class TimeRule : WaveRule
    {
      
        public override void Reset()
        {
            throw new NotImplementedException();
            
        }

        public override void Tick()
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