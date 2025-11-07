using System;
using DreamersInc.ReverbCity;
using UnityEngine;

namespace DreamersInc.WaveSystem.interfaces
{


    public abstract class WaveRule: ScriptableObject, IDisposable
    {
        uint Credits { get; }
        uint Exp{ get; }
        public Action OnWaveStart = delegate { };
        public Action OnWaveEnd = delegate { };
        public bool IsRunning { get; private set; }

        public virtual void Start()
        {
            if(IsRunning) return;
            IsRunning = true;
            WaveManager.RegisterWave(this);
            OnWaveStart.Invoke();
        }

        public void Stop()
        {
            if(!IsRunning) return;
            IsRunning = false;
            WaveManager.DeregisterWave(this);
            OnWaveEnd.Invoke();
        }
        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public abstract void Reset();
        
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