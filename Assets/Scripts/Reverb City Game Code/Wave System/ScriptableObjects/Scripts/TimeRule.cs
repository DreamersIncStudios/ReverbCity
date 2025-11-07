using System;
using DreamersInc.ReverbCity;
using DreamersInc.UIToolkitHelpers;
using DreamersInc.Utils;
using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using static DreamersInc.ReverbCity.GameCode.UI.UIExtensionMethods;

namespace DreamersInc.WaveSystem
{
    [CreateAssetMenu(menuName = "Wave Rules/Create TimeRule", fileName = "TimeRule", order = 0)]
    public class TimeRule : WaveRule
    {
      
        [SerializeField] float WaveDuration;
        private CountdownTimer timer;

        [CreateProperty] public string WaveDurationProperty => timer.IsFinished ? "Wave Completed" : $"Wave  Time Remaining {timer.CurrentTime}";
        public override void Start()
        {
            base.Start();
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
        }

        public override void ResetWave()
        {
            timer.Reset();
            
        }

        public override void Tick()
        {
            
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