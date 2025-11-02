using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DreamersInc.ReverbCity
{


    public struct WaveManager : IComponentData
    {
        public uint WaveCount;
        public uint Failures;
        public Difficulty Difficulty;
        public bool IsGameOver => Failures >= (uint)( Difficulty);
        public uint WaveNumber;
        public WaveRules WaveRules;

    }
    
    public enum Difficulty
    {
        Easy = 7,
        Normal = 5,
        Hard = 2
    }

    public struct WaveRules
    {
        
        public int SpawnRate;
        public int SpawnQty;
    }
}