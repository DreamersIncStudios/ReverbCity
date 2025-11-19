using System;
using Bestiary;
using DreamersInc.ReverbCity;
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
        [CreateProperty] public new string WaveLevelProperty => $"Enemies to Defeat {defeated}/{requiredEnemiesToDefeat}"; 
        private Vector3 spawnPosition = new Vector3();
        [SerializeField] float SpawnInterval;
        private float interval;
        private uint spawnCount;
        private uint defeated;
        public override void StartWave(uint waveLevel)
        {
            base.StartWave(waveLevel);
            WaveLevel = waveLevel;
            
        }
        public override void Tick()
        {
                 
            if (spawnPosition == Vector3.zero)
            {
                GlobalFunctions.RandomPoint(Vector3.zero, 50, out Vector3 testing);
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
                    SpawnNPC(new SerializableGuid(), spawnPosition, WaveLevel, base.WavePack);
                    spawnCount++;
                }

                interval = SpawnInterval * 60 / WaveLevel;
            }
        }
        public override void IncrementDefeat(int value = 1)
        {
            defeated += (uint)value;
            spawnCount-= (uint)value;
        }
        public override void FailCheck()
        {
            if (IsFinished)
            {
                CompleteWave();
                PassedTrial();
            }
        }
        public override void ResetWave()
        {
            defeated = 0;
            KillWaveNpcs(WaveLevel);
     
        }
        public override bool IsFinished => defeated >= requiredEnemiesToDefeat;
        public override void PassedTrial()
        {
            CompleteWave();
            WaveManager.DeregisterWave(this);
            Debug.Log($"Wave Completed. Reward player with {Credits}gold and {Exp}exp");
            Debug.Log("Start Wave Cool down timer");
        }

        public override void FailTrial()
        {
            throw new NotImplementedException();
        }
    }
}
