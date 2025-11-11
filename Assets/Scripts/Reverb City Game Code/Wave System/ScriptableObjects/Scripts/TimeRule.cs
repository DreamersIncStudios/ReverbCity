using System;
using Bestiary;
using DreamersInc.ReverbCity;
using DreamersInc.UIToolkitHelpers;
using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;
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

           GlobalFunctions.RandomPoint(Vector3.zero, 750, out spawnPosition);
        }

        public override void ResetWave()
        {
            timer.Reset();
            
        }

        public override void Tick()
        {
            if (IsRunning && interval > 0)
            {
                interval -= Time.deltaTime;
            }

            if (!IsRunning || !(interval <= 0))
                return;
            for (int i = 0; i < spawnCount* WaveLevel; i++)
            {
                SpawnNPC(new SerializableGuid(), spawnPosition, 2);

            }
            interval = SpawnInterval*60/WaveLevel;
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