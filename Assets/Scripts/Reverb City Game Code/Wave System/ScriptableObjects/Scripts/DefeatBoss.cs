using DreamersInc.WaveSystem.interfaces;
using UnityEngine;
namespace DreamersInc.WaveSystem
{
    [CreateAssetMenu(menuName = "Wave Rules/Create TimeRule", fileName = "Defeat Boss", order = 0)]
    
    public class DefeatBoss: ScriptableObject, IWaveRule
    {
        [Header("Reward")] 
        [SerializeField] private uint credits, exp;
        public uint Credits => credits;
        public uint Exp => exp;
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
