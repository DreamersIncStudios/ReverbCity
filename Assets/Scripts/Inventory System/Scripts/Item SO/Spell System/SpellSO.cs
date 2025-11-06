using System;
using System.Collections.Generic;
using Dreamers.InventorySystem.AbilitySystem;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.CombatSystem;
using DreamersInc.DamageSystem;
using DreamersInc.MagicSystem;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;
using IEffects = Dreamers.InventorySystem.AbilitySystem.IEffects;
using Random = UnityEngine.Random;

namespace Dreamers.InventorySystem
{
    public  class SpellSO : ItemBaseSO
    {

        public int Level;

        public bool Layered;
        [ShowIf(nameof(Layered))]
        public  int MaxLayerCount;

        [SerializeField]private Spell[] spellSlots = new Spell[1];
        public int ManaCost => -manaCost;
        [SerializeField] private int manaCost =10 ;

        [SerializeReference]public List<IEffects> Effects;

        [Header("Spell Book Conjured Weapons")]
        public int ComboID => comboID;
        [SerializeField] private int comboID;
        private void OnValidate()
        {
            MaxLayerCount = Layered? 1: spellSlots.Length+1;
            if (!Layered && spellSlots.Length >1)
                Array.Resize(ref spellSlots, 1);
            if(Effects.IsNullOrEmpty()) return;
            foreach (var effect in Effects)
            {
                effect.ManaCost = effect switch
                {
                    ChargedProjectile => Mathf.RoundToInt(ManaCost * 1.5f),
                    Buff or Conjure or Heal or ImpactExplosion or Projectile => ManaCost,
                    _ => effect.ManaCost
                };
            }

        }

        private EventHandler<WeaponEventTrigger.CastingArg> onFireProjectile ;
        private EventHandler<WeaponEventTrigger.CastingArg> onHitEnemy;
        private EventHandler<WeaponEventTrigger.CastingArg> onHitByEnemy;
        private EventHandler<CastingInputArgs> caster;
        public void AddSpell(BaseCharacterComponent player)
        {
            onFireProjectile = null;
            var trigger = player.GORepresentative.GetComponent<WeaponEventTrigger>();
            
            foreach (IEffects effects in Effects)
            {
                switch (effects)
                {
             
                    case Projectile :
                        onFireProjectile +=
                            ((_, args) => effects.Trigger(args.CharacterStats));
                        break;     
                }
            
           caster += ((_, args) => effects.Trigger(args.CharacterStats, args.InputCode));
            }
            trigger.OnFireProjectile += onFireProjectile;
            trigger.OnCastingInput += caster;
    
        }
        

        public void RemoveSpell(BaseCharacterComponent player)
        {
            Debug.Log("Remove Spell");
            var trigger = player.GORepresentative.GetComponent<WeaponEventTrigger>();
            trigger.OnFireProjectile -= onFireProjectile;
            trigger.OnCastingInput += caster;

        }

 

    }
    
}