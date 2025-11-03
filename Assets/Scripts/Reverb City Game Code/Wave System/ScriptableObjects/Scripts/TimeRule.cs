using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;
using UnityEngine;

namespace DreamersInc.WaveSystem
{
    public class TimeRule : ScriptableObject, IWaveRule
    {
        [SerializeField] float Duration;
        private uint exp, credits;
        public bool Pass { get; private set;}
        public uint DifficultyRate => difficultyRate;
        [Range(1,50)]
        [SerializeField]
        private uint difficultyRate;

        private CountdownTimer countdown;
        public void Init()
        {
            
            //Cache Player's Score and Stats for reset
            countdown = new CountdownTimer(Duration);
            countdown.OnTimerStop += () => Pass = countdown.IsFinished;
            countdown.OnTimerStop += () =>
            {
                if (Pass)
                    PassedTrial();
                else
                    FailTrial();
            };
        }

        public void Execute()
        {
            countdown.Start();
        }
        public void Reset()
        {
            //Reset Player's Score and stats tp 
            countdown.Reset();
        }
        public void PassedTrial()
        {
            difficultyRate++;
            
        }
        public void FailTrial()
        {
            // get Manager Singleton Failures ++
        }

    }
}