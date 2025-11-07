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
        private static readonly List<WaveRule> waves= new();
        private static readonly List<WaveRule> sweepWave= new();

        public static uint PlayerLevel;
        public static void RegisterWave(WaveRule wave) => waves.Add(wave);
        public static void DeregisterWave(WaveRule wave) => waves.Remove(wave);

        public static void UpdateWaves()
        {
            if (waves.Count == 0) return;
            sweepWave.RefreshWith(waves);
            foreach (var wave in sweepWave) {
                wave.Tick();
            }
        }

        public static void Clear()
        {
            sweepWave.RefreshWith(waves);
            foreach (var timer in sweepWave) {
                timer.Dispose();
            }
            
            waves.Clear();
            sweepWave.Clear();
        }
    }
    internal static class WaveBootstrapper
    {
        private static PlayerLoopSystem waveSystem;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize() {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            if (!InsertWaveManager<Update>(ref currentPlayerLoop, 0))
            {
                Debug.LogWarning("Improved Timers not initialized, unable to register TimerManager into the Update loop.");
                return;
            }
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;
            static void OnPlayModeState(PlayModeStateChange state) {
                if (state == PlayModeStateChange.ExitingPlayMode) {
                    PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    RemoveWaveManager<Update>(ref currentPlayerLoop);
                    PlayerLoop.SetPlayerLoop(currentPlayerLoop);
                    
                    TimerManager.Clear();
                }
            }
#endif
        }
        
        static void RemoveWaveManager<T>(ref PlayerLoopSystem loop) {
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