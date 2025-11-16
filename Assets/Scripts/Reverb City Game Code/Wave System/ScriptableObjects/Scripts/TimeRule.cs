using DreamersInc.ReverbCity;
using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;
using Stats.Entities;
using Unity.Entities;
using Unity.Properties;
using UnityEngine;
using Utilities;
using static Bestiary.BestiaryManager;

namespace DreamersInc.WaveSystem
{
    [CreateAssetMenu(menuName = "Wave Rules/Create TimeRule", fileName = "TimeRule", order = 0)]
    public class TimeRule : WaveRule
    {
      
        [SerializeField] float WaveDuration;
        private CountdownTimer timer;
        [SerializeField] float SpawnInterval;
        private float interval;
        [SerializeField] int spawnCount;
        [CreateProperty] public new string WaveLevelProperty => timer.IsFinished ? "Wave Completed" : $"Wave  Time Remaining {timer.CurrentTime}";
        private Vector3 spawnPosition = new Vector3();
        
        EntityManager entityManager;
        public override void StartWave(uint waveLevel)
        {
            base.StartWave(waveLevel);
            WaveLevel = waveLevel; 
            timer = new CountdownTimer(WaveDuration*60);
            timer.Start();

           GlobalFunctions.RandomPoint(Vector3.zero, 750, out Vector3 testing);
           spawnPosition = testing; 
           entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        public override void Stop()
        {
            base.Stop();
            spawnPosition = Vector3.zero;
            spawned = 0;
            interval = 0;
        }
        public override void IncrementDefeat(int value = 1)
        {
            throw new System.NotImplementedException();
        }
        public override void ResetWave()
        {
            timer.Reset();
            timer.Start();// Do we want a reset timer or countdown timer
            
        }
        private int spawned;
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

            if (IsRunning && interval <= 0 && spawned < spawnCount)
            {
                for (int i = 0; i < 4 * WaveLevel; i++)
                {
                    SpawnNPC(new SerializableGuid(), spawnPosition, WaveLevel);
                    spawned++;
                }

                interval = SpawnInterval * 60 / WaveLevel;
            }
        }

        public override void FailCheck()
        {
            var stats = entityManager.GetComponentData<BaseCharacterComponent>(PlayerEntity);
            if (stats.CurHealth == 0)
            {
                Stop();
                FailTrial(); 
                return;
            }
            
            if (timer.IsFinished)
            {
                PassedTrial();
            }
        }

 

        public override bool IsFinished => timer.IsFinished;
        public override void PassedTrial()
        {
        CompleteWave();
        WaveManager.DeregisterWave(this);
        Debug.Log($"Wave Completed. Reward player with {Credits}gold and {Exp}exp");
        Debug.Log("Start Wave Cool down timer");
        }

        public override void FailTrial()
        {
            CompleteWave();
            WaveManager.Fails++;
            Debug.Log($"Wave Failed. Player died");
            Debug.Log("Start Wave Cool down timer");
        }
    }
}