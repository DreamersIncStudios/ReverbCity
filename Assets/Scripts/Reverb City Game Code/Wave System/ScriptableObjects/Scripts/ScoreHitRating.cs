using System;
using DreamersInc.ReverbCity;
using DreamersInc.Trackers;
using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;
using UnityEngine;

namespace DreamersInc.WaveSystem
{
    public class ScoreHitRating:WaveRule
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

    public enum StyleScoring
    {
        Dismal, Cruel, Brutal, AreYouOK, Stylish, Savage, Sensational
    }
}