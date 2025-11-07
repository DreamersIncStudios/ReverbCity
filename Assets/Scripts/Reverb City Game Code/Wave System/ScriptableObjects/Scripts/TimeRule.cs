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
      
        [SerializeField] float WaveDuration;
        private CountdownTimer timer;
        public override void Start()
        {
            base.Start();
            timer = new CountdownTimer(WaveDuration);
            timer.Start();
        }

        public override void Reset()
        {
            timer.Reset();
            
        }

        public override void Tick()
        {
            
        }

        public override bool IsFinished => timer.IsFinished;
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