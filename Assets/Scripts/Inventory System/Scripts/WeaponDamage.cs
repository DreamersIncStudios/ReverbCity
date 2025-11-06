using System.Collections;
using UnityEngine;
using DreamersInc.DamageSystem.Interfaces;
using Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using Sirenix.Utilities;
using Stats.Entities;
using Unity.Entities;

namespace DreamersInc.DamageSystem
{
    // [RequireComponent(typeof(MeshCollider))]
    public class WeaponDamage : MonoBehaviour, IDamageDealer
    {
        public event EventHandler<OnHitArgs> OnHitAction;
        public Action ChanceCheck { get; set; }
        public Action CriticalEventCheck { get; set; }
        public Stat Magic_Offense { get; private set; }
        public Stat Range_Offense { get; private set; }
        public Stat Melee_Offense { get; private set; }
        public Attributes Skill { get; private set; }
        public Attributes Speed { get; private set; }
        
        private Animator animator;

        private Entity ParentEntity => self.SelfEntityRef;

        public int BaseDamage
        {
            get
            {
              // Todo Add mod value for Magic infused/ Enchanted weapon
                var output = TypeOfDamage switch
                {
                    TypeOfDamage.MagicAoE => Magic_Offense.AdjustBaseValue,
                    TypeOfDamage.Projectile =>  Range_Offense.AdjustBaseValue,
                    TypeOfDamage.Melee => Melee_Offense.AdjustBaseValue,
                    _ => Melee_Offense.AdjustBaseValue,
                };
                return output;
            }
        }
        public float CriticalHitMod => CriticalHit ? Random.Range(1.5f, 2.15f) : 1;
        private float randomMod => Random.Range(.85f, 1.15f);
        
        public bool CriticalHit
        {
            get
            {
                var prob = Mathf.RoundToInt(Random.Range(0, 255));
                var threshold =  (Skill.AdjustBaseValue + Speed.AdjustBaseValue) / 2;
                return prob < threshold;
            }
        }
        
        public float MagicMod { get; private set; }
        public ElementName ElementName { get; private set; }

        public TypeOfDamage TypeOfDamage { get; private set; }
        public WeaponType Type => type;
        [SerializeField] WeaponType type;

        public bool DoDamage { get; private set; }

        public int DamageAmount()
        {
            return Mathf.RoundToInt(BaseDamage * randomMod );
        }
        private void Start()
        {
            animator = GetComponentInParent<Animator>();
            registered = true;
            //Todo rewrite this 
            if (GetComponent<Collider>())
            {
                TypeOfDamage = TypeOfDamage.Melee;
                GetComponent<Collider>().isTrigger = true;
                self = GetComponentInParent<IDamageable>();
            }
            else
            {
              //  throw new ArgumentNullException(nameof(gameObject), $"Collider has not been set up on equipped weapon. Please set up Collider in Editor; {gameObject.transform.parent.name}");
            }
        }
        
        public void SetDamageBool(bool value)
        {
            DoDamage = value;
        }

        public void SetDamageType()
        {
            throw new System.NotImplementedException();
        }

        public void SetElement(ElementName value)
        {
            ElementName = value;
            //TODO Balance 
            MagicMod =  ElementName != ElementName.None ? Magic_Offense.AdjustBaseValue / 10.0f : 1.0f;
        }

        
        private IDamageable self;

        // Use this for initialization
 

        private float criticalHitMod;
        private IDamageDealer damageDealerImplementation;

        public void CheckChance()
        {
            criticalHitMod = CriticalHitMod;
            if (criticalHitMod != 1) { 
                CriticalEventCheck();
            }
            ChanceCheck.Invoke();
        }
        int         hash = Animator.StringToHash("Light");

        public void OnTriggerEnter(Collider other)
        {

            var hit = other.GetComponent<IDamageable>();
            //Todo add Friend filter.
            if (!DoDamage || hit == null || hit == self) return;
            hit.TakeDamage(DamageAmount(), TypeOfDamage, ElementName, ParentEntity, level);
            var root = transform.root;
            hit.ReactToHit(.5f, root.position, root.forward);
            var attackType = animator.GetCurrentAnimatorStateInfo(0).tagHash switch
            {
                var state when state == Animator.StringToHash("Light") => 1,
                var state when state == Animator.StringToHash("Heavy") => 2,
                _ => 0
            };
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            OnHitAction?.Invoke(this, new OnHitArgs() { Entity = hit.SelfEntityRef, TypeOfDamage = this.TypeOfDamage, AttackType = attackType, StateInfo  = stateInfo});
        }
        

        private uint level;

        private bool registered;
        [SerializeField]private BaseCharacterComponent statsInfo;
        /// <summary>
        /// Sets the stat data for the weapon.
        /// </summary>
        /// <param name="stats">The BaseCharacterComponent containing the stats.</param>
        /// <param name="damageType">The type of damage.</param>
        ///
        ///
        public void SetStatData(BaseCharacterComponent stats, TypeOfDamage damageType)
        {
            statsInfo = stats;
            Magic_Offense = stats.GetStat((int)StatName.MagicOffence);
            Range_Offense = stats.GetStat((int)StatName.RangedOffence);
            Melee_Offense = stats.GetStat((int)StatName.MeleeOffence);
            Speed = stats.GetPrimaryAttribute((int)AttributeName.Speed);
            Skill = stats.GetPrimaryAttribute((int)AttributeName.Skill);
            TypeOfDamage = damageType;

            level = (uint)stats.Level;

            if (registered) return;
            stats.OnStatChanged += ((_, args) => SetStatData(args.Stats, TypeOfDamage));
            registered = true;
        }

        private void OnDestroy()
        {
            OnHitAction = null;
            
            
        }
    }
    public class OnHitArgs : EventArgs
    {
        public Entity Entity;
        public TypeOfDamage TypeOfDamage;
        public int AttackType;
        public AnimatorStateInfo StateInfo;
        public int EnemyID;
    }

}