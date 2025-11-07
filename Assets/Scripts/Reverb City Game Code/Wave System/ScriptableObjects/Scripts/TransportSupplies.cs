using System;
using DreamersInc.ReverbCity;
using DreamersInc.WaveSystem.interfaces;
using UnityEngine;
namespace DreamersInc.WaveSystem
{
    [CreateAssetMenu(menuName = "Wave Rules/Create transport", fileName = "TransportSupplies", order = 0)]
    
    public class TransportSupplies: WaveRule
    {

        public override void ResetWave()
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
