using System;
using DreamersInc.WaveSystem.interfaces;
using Unity.Properties;
using UnityEngine;
using Utilities;
using static Bestiary.BestiaryManager;
namespace DreamersInc.WaveSystem
{
    
    [CreateAssetMenu(menuName = "Wave Rules/Create Defeat Enemies", fileName = "Defeat X Enemies", order = 1)]
    public class DefeatEnemies : WaveRule
    {
        [SerializeField] uint MaxSpawnCount;
        [SerializeField] uint requiredEnemiesToDefeat;
        [CreateProperty] public new string WaveLevelProperty => $"Enemies to Defeat {requiredEnemiesToDefeat}/{spawnCount}"; 
        private Vector3 spawnPosition = new Vector3();
        [SerializeField] float SpawnInterval;
        private float interval;
        private uint spawnCount;
        public override void StartWave(uint waveLevel)
        {
            base.StartWave(waveLevel);
            WaveLevel = waveLevel;
            
        }
        public override void Tick()
        {
                 
            if (spawnPosition == Vector3.zero)
            {
                GlobalFunctions.RandomPoint(Vector3.zero, 750, out Vector3 testing);
                spawnPosition = testing;    
                return;
            }
            if (IsRunning && interval > 0)
            {
                interval -= Time.deltaTime;
            }

            if (IsRunning && interval <= 0 && spawnCount<MaxSpawnCount)
            {
                for (int i = 0; i < 4 * WaveLevel; i++)
                {
                    SpawnNPC(new SerializableGuid(), spawnPosition, WaveLevel);
                    spawnCount++;
                }

                interval = SpawnInterval * 60 / WaveLevel;
            }
        }
        public override void FailCheck()
        {
  
        }
        public override void ResetWave()
        {
        
            
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
