using System;
using Bestiary;
using DreamersInc.ReverbCity;
using DreamersInc.UIToolkitHelpers;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using static DreamersInc.ReverbCity.GameCode.UI.UIExtensionMethods;

namespace DreamersInc.WaveSystem.interfaces
{


    public abstract class WaveRule: ScriptableObject, IDisposable
    {
        protected uint Credits { get; }
        protected uint Exp{ get; }
        public Action OnWaveStart = delegate { };
        public Action OnWaveEnd = delegate { };
        public bool IsRunning { get; private set; }
        [CreateProperty] protected string WaveLevelProperty=>"This need to be overriden";
        protected uint WaveLevel;

        public virtual void StartWave(uint waveLevel)
        {
            if(IsRunning) return;
            IsRunning = true;
            WaveLevel = waveLevel;
            WaveManager.RegisterWave(this);
            OnWaveStart.Invoke();
            SetUILabel();
        }
        protected void SetUILabel()
        {
            var hudUI = UIManager.GetUI(UIType.HUD);
            var panel = hudUI.rootVisualElement.Q<WaveUIPanel>();
            var label = Create<Label>("WaveInfo");
            label.dataSource = this;
            label.SetBinding(nameof(Label.text), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(WaveLevelProperty)),
                bindingMode = BindingMode.ToTarget
            });
            panel.Add(label);
        }

        public virtual void Stop()
        {
            if(!IsRunning) return;
            IsRunning = false;
            WaveManager.DeregisterWave(this);
            OnWaveEnd.Invoke();
        }
        protected void CompleteWave()
        {
            Stop();
            BestiaryManager.KillWaveNpcs(WaveLevel);
            
        }
        public abstract void FailCheck();

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public abstract void ResetWave();
        
        public abstract void Tick();
        public abstract bool IsFinished { get; }
        public abstract void PassedTrial();
        public abstract void FailTrial();
        public bool Pass { get; }
        
        bool disposed;
        
        ~WaveRule() {
            Dispose(false);
        }

        // Call Dispose to ensure deregistration of the timer from the TimerManager
        // when the consumer is done with the timer or being destroyed
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed) return;

            if (disposing) {
                WaveManager.DeregisterWave(this);
            }

            disposed = true;
        }
    }


}