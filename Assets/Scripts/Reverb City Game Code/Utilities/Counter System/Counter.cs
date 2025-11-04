using System;
using System.Collections.Generic;
using ImprovedTimers;

namespace DreamersInc.Trackers{
    public static class CounterManager
    {
        static readonly List<Counter> counters = new List<Counter>();
        static readonly List<Counter> sweep = new List<Counter>();
        public static void RegisterCounter(Counter counter) => counters.Add(counter);
        public static void DeregisterCounter(Counter counter) => counters.Remove(counter);

        public static void Clear()
        {
            sweep.RefreshWith(counters);
            foreach (var counter in sweep)
            {
                counter.Dispose();
            }
            counters.Clear();
            sweep.Clear();
        }
    }

    public abstract class Counter:IDisposable
    {
        public int CurrentValue { get; protected set; }
        public bool IsCounting { get; protected set; }
        
        protected int InitialValue;
        public Action OnCounterStart = delegate { };
        public Action OnCounterEnd = delegate { };

        protected Counter(int value)
        {
            InitialValue = value;
        }

        public void Start()
        {
            CurrentValue = InitialValue;
            if (!IsCounting)
            {
                IsCounting = true;
                CounterManager.RegisterCounter(this);
                OnCounterStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsCounting)
            {
                IsCounting= false;
                CounterManager.DeregisterCounter(this);
                OnCounterEnd.Invoke();
            }
        }


        public abstract void Increment();
        public abstract bool IsFinished { get; }
        
        public void Resumer() => IsCounting = true;
        public void Pause() => IsCounting = false;
        public void Reset() => CurrentValue = InitialValue;
        public void Reset(int value)
        {
            InitialValue = value;
            Reset();
        }
        bool disposed;
        ~Counter()
        {
            Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                CounterManager.DeregisterCounter(this);
            }
            disposed = true;
        }
    }
}