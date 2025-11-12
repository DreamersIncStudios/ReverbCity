using System.Collections.Generic;
using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace DreamersInc.ReverbCity
{
    public static class WaveManager
    {
        private static readonly List<WaveRule> Waves= new List<WaveRule>();
        private static readonly List<WaveRule> SweepWave= new List<WaveRule>();

        public static uint PlayerLevel;
        public static void RegisterWave(WaveRule wave) => Waves.Add(wave);
        public static void DeregisterWave(WaveRule wave) => Waves.Remove(wave);

        public static void UpdateWaves()
        {
            if (Waves.Count == 0) return;
            SweepWave.RefreshWith(Waves);
            foreach (var wave in SweepWave) {
                wave.Tick();
                wave.FailCheck();
            }
        }
        public static void StopWaves()
        {
            if (Waves.Count == 0) return;
            SweepWave.RefreshWith(Waves);
            foreach (var wave in SweepWave) {
                wave.Stop();
            }
        }

        public static void Clear()
        {
            SweepWave.RefreshWith(Waves);
            foreach (var timer in SweepWave) {
                timer.Dispose();
            }
            
            Waves.Clear();
            SweepWave.Clear();
        }
    }
    internal static class WaveBootstrapper
    {
        private static PlayerLoopSystem waveSystem;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            if (!InsertWaveManager<Update>(ref currentPlayerLoop, 0))
            {
                Debug.LogWarning("Wave System not initialized, unable to register Wave Manager into the Update loop.");
                return;
            }

            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;
            static void OnPlayModeState(PlayModeStateChange state)
            {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    WaveManager.StopWaves();
                    RemoveWaveManager<Update>(ref currentPlayerLoop);
                    PlayerLoop.SetPlayerLoop(currentPlayerLoop);

                    TimerManager.Clear();
                }
            }
#endif
        }

        static void RemoveWaveManager<T>(ref PlayerLoopSystem loop)
        {
            PlayerLoopUtils.RemoveSystem<T>(ref loop, in waveSystem);
        }

        static bool InsertWaveManager<T>(ref PlayerLoopSystem loop, int index)
        {
            waveSystem = new PlayerLoopSystem()
            {
                type = typeof(WaveManager),
                updateDelegate = WaveManager.UpdateWaves,
                subSystemList = null
            };
            return PlayerLoopUtils.InsertSystem<T>(ref loop, in waveSystem, index);
        }
    }
}