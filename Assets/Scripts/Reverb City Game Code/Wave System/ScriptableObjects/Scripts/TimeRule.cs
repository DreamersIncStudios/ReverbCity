using System;
using Bestiary;
using DreamersInc.ReverbCity;
using DreamersInc.SceneManagement;
using DreamersInc.UIToolkitHelpers;
using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;
using Stats.Entities;
using Unity.Entities;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;
using static DreamersInc.ReverbCity.GameCode.UI.UIExtensionMethods;
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
        [CreateProperty] public string WaveDurationProperty => timer.IsFinished ? "Wave Completed" : $"Wave  Time Remaining {timer.CurrentTime}";
        private Vector3 spawnPosition = new Vector3();
        
        EntityManager entityManager;
        public override void StartWave(uint waveLevel)
        {
            base.StartWave(waveLevel);
            WaveLevel = waveLevel; 
            timer = new CountdownTimer(WaveDuration*60);
            timer.Start();
            var hudUI = UIManager.GetUI(UIType.HUD);
            var panel = hudUI.rootVisualElement.Q<WaveUIPanel>();
           var label = Create<Label>("WaveInfo");
           label.dataSource = this;
           label.SetBinding(nameof(Label.text), new DataBinding()
           {
               dataSourcePath = new PropertyPath(nameof(WaveDurationProperty)),
               bindingMode = BindingMode.ToTarget
           });
           panel.Add(label);

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
        public override void ResetWave()
        {
            timer.Reset();
            
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
                    SpawnNPC(new SerializableGuid(), spawnPosition, 2);
                    spawned++;
                }

                interval = SpawnInterval * 60 / WaveLevel;
            }
        }

        public override void FailCheck()
        {
            var stats = entityManager.GetComponentObject<BaseCharacterComponent>(GameMaster.PlayerEntity);
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
            throw new NotImplementedException();
        }

        public override void FailTrial()
        {
            throw new NotImplementedException();
        }
    }
}