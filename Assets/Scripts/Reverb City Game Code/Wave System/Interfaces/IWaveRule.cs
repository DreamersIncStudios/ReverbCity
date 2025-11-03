using UnityEngine;

namespace DreamersInc.WaveSystem.interfaces
{


    public interface IWaveRule
    {
        public void Init();
        public void Execute();
        public void Reset();
        public void PassedTrial();
        public void FailTrial();
        public bool Pass { get; }
        public uint DifficultyRate { get; } // Scale of 1 to 50;
    }


}