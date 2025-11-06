using System.Linq;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.ServiceLocatorSystem;
using Stats;
using Stats.Entities;
using Unity.Properties;
using UnityEngine;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public class StatsViewModel
    {
        private readonly BaseCharacterComponent characterStats;

        public StatsViewModel(ref BaseCharacterComponent characterStats )
        {
            this.characterStats = characterStats;

        }
        [CreateProperty]
        public string PlayerName => $"{characterStats.Name} \tLevel: {characterStats.Level}" ;
        
        [CreateProperty]
        public string PlayerStats
        {
            get
            {
                var statsString = $"Exp:\n" +
                                  $"Health:\n" +
                                  $"Mana:\n" +
                                  $"Strength:\n" +
                                  $"Vitality:\n" +
                                  $"Resistance:\n" +
                                  $"Attack:\n" +
                                  $"Magic Power:\n";

                return statsString;
            }
        }      
        [CreateProperty]
        public string PlayerStatsValues
        {
            get
            {
                var statsString = $"{characterStats.FreeExp}/{characterStats.ExpTilNextLevel}\n" +
                                  $"{characterStats.CurHealth}/{characterStats.MaxHealth}\n" +
                                  $"{characterStats.CurMana}/{characterStats.MaxMana}\n" +
                                  $"{characterStats.GetPrimaryAttribute((int)AttributeName.Strength).AdjustBaseValue}\n" +
                                  $"{characterStats.GetPrimaryAttribute((int)AttributeName.Vitality).AdjustBaseValue}\n" +
                                  $"{characterStats.GetPrimaryAttribute((int)AttributeName.Resistance).AdjustBaseValue}\n" +
                                  $"{characterStats.GetStat((int)StatName.MeleeOffence).AdjustBaseValue}\n" +
                                  $"{characterStats.GetStat((int)StatName.MagicOffence).AdjustBaseValue}\n";

                return statsString;
            }
        }
        [CreateProperty]
        public Texture ActiveCharacterImage => ServiceLocator.Global.Get<UITextureControl>().ProfileTexture;

    }
}