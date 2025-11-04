using DreamersInc.WaveSystem.interfaces;
using UnityEngine;
namespace DreamersInc.WaveSystem
{
    [CreateAssetMenu(menuName = "Wave Rules/Create TimeRule", fileName = "ChargeTheTower", order = 0)]
    
    public class ChargeTheTower: ScriptableObject, IWaveRule
    {
        public void Init()
        {
            throw new System.NotImplementedException();
        }
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
        public void Reset()
        {
            throw new System.NotImplementedException();
        }
        public void PassedTrial()
        {
            throw new System.NotImplementedException();
        }
        public void FailTrial()
        {
            throw new System.NotImplementedException();
        }
        public bool Pass
        {
            get;
        }
        public uint DifficultyRate
        {
            get;
        }
    }
}
