using System;
using System.Threading.Tasks;
using DreamersInc.CombatSystem.Animation;
using DreamersInc.DamageSystem.Interfaces;
using DreamersInc.EntityUtilities;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Stats
{
    [RequireComponent(typeof(Collider))]
    public sealed class Damageable : MonoBehaviour, IDamageable
    {

        public Entity SelfEntityRef { get; private set; }
        private Stat meleeDefense;
        private Stat magicDefense;
        public Collider GetCollider => GetComponent<Collider>();
        private float MagicDef => 1.0f / (1.0f + (magicDefense.AdjustBaseValue / 100.0f));
        private float MeleeDef => 1.0f / (1.0f + (meleeDefense.AdjustBaseValue / 100.0f));

        /// <summary>
        /// Reacts to a hit received by the damageable object.
        /// </summary>
        /// <param name="impact">The impact force of the hit.</param>
        /// <param name="hitPosition">The test vector used for hit contact point.</param>
        /// <param name="forward">The forward vector of the damageable object.</param>
        /// <param name="typeOf">The type of damage inflicted (default: TypeOfDamage.Melee).</param>
        /// <param name="elementName">The element of the damage inflicted (default: Element.None).</param>
        public void ReactToHit(float impact, Vector3 hitPosition, Vector3 forward, TypeOfDamage typeOf = TypeOfDamage.Melee, ElementName elementName = ElementName.None)
        {

            ReactToContact reactTo = new()
            {
                Duration = .75f,
                ForwardVector = forward,
                positionVector = this.transform.position,
                RightVector = transform.right,
                HitIntensity =3,//Todo balance the mathe Mathf.FloorToInt(impact / (defense * 10.0f) * Random.Range(.92f, 1.08f)),
                HitContactPoint = hitPosition
            };
            if (!World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<ReactToContact>(SelfEntityRef))
                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(SelfEntityRef, reactTo);
        }

        /// <summary>
        /// Takes a specific amount of damage based on the type of damage and element.
        /// </summary>
        /// <param name="amount">The amount of damage to be taken.</param>
        /// <param name="typeOf">The type of damage.</param>
        /// <param name="elementName">The element of damage.</param>
        /// <param name="damageDealerEntity">entity ref to who dealt the damage</param>
        public void TakeDamage(int amount, TypeOfDamage typeOf, ElementName elementName, Entity damageDealerEntity, uint level)
        {
            var manager =
                World.DefaultGameObjectInjectionWorld.EntityManager;
            //Todo Figure out element resistances, conditional mods, and possible affinity 
            float defense = typeOf switch
            {
                TypeOfDamage.MagicAoE => MagicDef,
                TypeOfDamage.Magic=> MagicDef,
                _ => MeleeDef,
            };

            var damageElementMod = 1.0f;
            if (elementName != ElementName.None)
            {
                var stats = manager.GetComponentData<BaseCharacterComponent>(SelfEntityRef);

                damageElementMod = elementName switch
                {
                    ElementName.Fire => stats.GetElementMod(ElementName.Fire).AdjustBaseValue,
                    ElementName.Water => stats.GetElementMod(ElementName.Water).AdjustBaseValue,
                    ElementName.Earth => stats.GetElementMod(ElementName.Earth).AdjustBaseValue,
                    ElementName.Wind => stats.GetElementMod(ElementName.Wind).AdjustBaseValue,
                    ElementName.Ice => stats.GetElementMod(ElementName.Ice).AdjustBaseValue,
                    ElementName.Holy => stats.GetElementMod(ElementName.Holy).AdjustBaseValue,
                    ElementName.Dark => stats.GetElementMod(ElementName.Dark).AdjustBaseValue,
                    _ => 1.0f
                };
            }
            var damageToProcess = -Mathf.FloorToInt(amount * defense * Random.Range(.92f, 1.08f)* damageElementMod);
            Debug.Log(damageToProcess + " HP of damage to target ");
            manager.AddComponentData(SelfEntityRef, new AdjustHealth(damageToProcess, damageDealerEntity, level));
        }
        public void SetData(Entity entity, BaseCharacterComponent character) {
            SelfEntityRef = entity;
            magicDefense = character.GetStat((int)StatName.MagicDefense);
            meleeDefense = character.GetStat((int)StatName.MeleeDefense);

        }

        private async void OnDestroy()
        {
            try
            {
                if (!Application.isPlaying) return;
                var ecbSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
                ecbSystem.CreateCommandBuffer().DestroyEntity(SelfEntityRef);
                await Task.Delay(2000);
                if(World.DefaultGameObjectInjectionWorld.EntityManager.Exists(SelfEntityRef))
                    EntityExtensions.RemoveAllComponents(World.DefaultGameObjectInjectionWorld.EntityManager, SelfEntityRef);
            }
            catch (Exception e)
            {
               
            }
        }

    }
}
