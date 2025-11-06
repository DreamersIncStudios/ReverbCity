using System;
using System.Collections.Generic;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.CombatSystem;
using DreamersInc.MagicSystem;
using Sirenix.OdinInspector;
using Stats;
using Stats.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dreamers.InventorySystem.AbilitySystem{
    
    public interface IEffects
    {
        GameObject VFXPrefab { get; }
        public int ManaCost { get; set; }
        string InputCode { get; }
        SpellAnimationInfo SpellAnimationInfo { get; }
        public SpellType SpellType { get; }

        void Trigger(BaseCharacterComponent player , string inputCode = "");
        
    }
    [Serializable]
    public struct SpellAnimationInfo {
        public uint AnimIndex;
        public float TransitionDuration, TransitionOffset, EndofCurrentAnim;

    }
    public interface IElemental
    {
        Elemental ElementalType { get; }
    }

    public interface IProjectile
    {
        float Speed { get; }
        GameObject VFXPrefab { get; }
    }

    public interface IOnHit
    {
        int Chance { get; }
    }

    public interface IHealing
    {
        
    }
    public interface IConjuring
    {
        
    }
    public interface IBuffing
    {
        
    }

    public class Projectile : IEffects,IProjectile
    {
        public SpellType SpellType => spellType;
        [SerializeField] private SpellType spellType;
        public string InputCode => inputCode;
        [ShowIf(nameof(ShowInputCode))][SerializeField] private string inputCode;
        private bool ShowInputCode => spellType != SpellType.Passive;
        public float Speed =>speed;
        public GameObject VFXPrefab=> vfxPrefab;
        [SerializeField] GameObject vfxPrefab;
        
        [SerializeField] private float speed;
        public int ManaCost{ get; set; }
        public  SpellAnimationInfo SpellAnimationInfo { get; }

        public virtual void Trigger(  BaseCharacterComponent player, string input = "")
        {
            
            if(input ==""  && input != InputCode) return;
            if (player.CurMana <ManaCost) return; //Todo create failed spell?
            player.AdjustMana(ManaCost);
           var spawnAt =player.GORepresentative.GetComponentInChildren<VFXSpawnPoint>().transform;
          new SpellBuilder("Spell")
              .WithVFX(VFXPrefab, spawnAt, 3)
              .WithDamage(player)
              .WithTrajectory(speed)
              .Build();
        }
    }  
    
    //Todo Figure out how to increase Damage for charged shot
    
    public class ChargedProjectile : Projectile
    {
        [SerializeField] private float speed;
        public override void Trigger(  BaseCharacterComponent player, string inputCode = "")
        {
            if (player.CurMana <ManaCost) return; //Todo create failed spell?
            player.AdjustMana(ManaCost);
            var spawnAt =player.GORepresentative.GetComponentInChildren<VFXSpawnPoint>().transform;
          new SpellBuilder("Spell")
              .WithVFX(VFXPrefab, spawnAt, 3)
              .WithDamage(player)
              .WithTrajectory(speed)
              .Build();
        }
    }

    public class ImpactExplosion : IEffects, IOnHit
    {
        public SpellType SpellType { get; }

        public GameObject VFXPrefab { get; set; }
        
        public int Chance { get; }
        public int ManaCost{ get; set; }
       public  string InputCode { get; }
       public  SpellAnimationInfo SpellAnimationInfo { get; }

        public void Trigger(BaseCharacterComponent player,string inputCode = "")
        {
            if(Random.Range(0,100) > Chance) return;
            if(player.CurMana<ManaCost) return;
            var spawnAt =player.GORepresentative.GetComponentInChildren<VFXSpawnPoint>().transform;
            new SpellBuilder("Spell")
                .WithVFX(VFXPrefab, spawnAt, 3)
                .WithDamage(player)
                .Build();

        }

    }

    public class ElementalProjectile : Projectile, IElemental,IOnHit
    {

       // public GameObject HitVFXPrefab;
        public int Range;
        public int Chance => 101;
     
        [SerializeField] private float speed;
        public Elemental ElementalType => elementalType; 
        [SerializeField]private Elemental elementalType;

        public override void Trigger(BaseCharacterComponent player, string input = "")
        {
            if(input !=""  && input != InputCode) return;
        
            if (player.CurMana <ManaCost) return; //Todo create failed spell?
            player.AdjustMana(ManaCost);
            var spawnAt =player.GORepresentative.GetComponentInChildren<VFXSpawnPoint>().transform;
            new SpellBuilder("Spell")
                .WithVFX(VFXPrefab, spawnAt, 3)
                .WithDamage(player)
                .WithTrajectory(speed)
               // .WithVFXOnHitOrDistance(HitVFXPrefab,player.GORepresentative.transform.position, Range/speed )
                .Build();
        }



    }

    public class Heal : IEffects
    {
     
        public SpellType SpellType => spellType;
        [SerializeField] private SpellType spellType;
        public string InputCode => inputCode;
        [ShowIf(nameof(ShowInputCode))][SerializeField] private string inputCode;
        private bool ShowInputCode => spellType != SpellType.Passive;
        public int ManaCost{ get; set; }
        public GameObject VFXPrefab { get; set; }
        

        public void Trigger(BaseCharacterComponent player, string inputCode)
        {
            if(inputCode !=""  && inputCode != InputCode) return;
            if (player.CurMana <ManaCost) return; //Todo create failed spell?
            player.AdjustMana(ManaCost);
            player.GORepresentative.GetComponent<WeaponEventTrigger>().AnimatorArgsToPass =
                new WeaponEventTrigger.AnimatorArgs(spellAnimationInfo.AnimIndex,
                    spellAnimationInfo.TransitionDuration, 
                    spellAnimationInfo.TransitionOffset,
                    spellAnimationInfo.EndofCurrentAnim);
            Debug.Log("Heal");
        }

        public SpellAnimationInfo SpellAnimationInfo => spellAnimationInfo;
        [SerializeField] private SpellAnimationInfo spellAnimationInfo;

    }
    public class Conjure : IEffects
    {
        public SpellType SpellType { get; }
        public GameObject VFXPrefab { get; set; }
        public int ManaCost{ get; set; }
        public  string InputCode { get; }
        public  SpellAnimationInfo SpellAnimationInfo { get; }
        [SerializeField] int startingHealth;
        public void Trigger(BaseCharacterComponent player, string inputCode = "")
        {
            if(inputCode !=""  && inputCode != InputCode) return;
            if (player.CurMana <ManaCost) return; //Todo create failed spell?
            player.AdjustMana(ManaCost);
            var healValue =startingHealth + player.GetStat((int)StatName.HealthRecover).AdjustBaseValue*(1+player.GetPrimaryAttribute((int)AttributeName.Skill).AdjustBaseValue/100);
            player.AdjustHealth(healValue);
            Debug.Log("Heal");
        }
    }
    public class Buff : IEffects
    {
        public SpellType SpellType { get; }
    

        public GameObject VFXPrefab { get; set; }
        public int ManaCost{ get; set; }
        public  string InputCode { get; }
        public  SpellAnimationInfo SpellAnimationInfo { get; }

        public void Trigger( BaseCharacterComponent player, string inputCode = "")
        {
            throw new System.NotImplementedException();
        }
    }


}