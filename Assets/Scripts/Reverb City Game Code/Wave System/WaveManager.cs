using System.Collections.Generic;
using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;

namespace DreamersInc.ReverbCity
{
    public static class WaveManager
    {
        private static readonly List<WaveRule> waves;
        private static readonly List<WaveRule> sweepWave;
        
        public static void RegisterWave(WaveRule wave) => waves.Add(wave);
        public static void DeregisterWave(WaveRule wave) => waves.Remove(wave);

        public static void UpdateWaves()
        {
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

}