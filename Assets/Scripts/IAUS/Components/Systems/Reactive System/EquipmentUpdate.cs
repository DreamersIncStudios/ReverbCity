using System;
using Unity.Entities;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using Sirenix.Utilities;
using Unity.Transforms;

// ReSharper disable Unity.BurstLoadingManagedType

namespace IAUS.ECS.Component
{
    public partial class EquipmentUpdate : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, ref AttackCapable attackState,
                ref CheckAttackStatus tag, in Parent root) =>
            {
                if (!EntityManager.HasComponent<CharacterInventory>(root.Value)) return;
                attackState.CapableOfMelee = false;
                attackState.CapableOfMagic = false;
                attackState.CapableOfProjectile = false;
                var test = EntityManager.GetComponentData<CharacterInventory>(root.Value);

                foreach (var item in test.Equipment.EquippedWeapons)
                {
                    switch (item.Value.WeaponType)
                    {
                        case WeaponType.Axe:
                        case WeaponType.Sword:
                        case WeaponType.H2BoardSword:
                        case WeaponType.Katana:
                        case WeaponType.Bo_Staff:
                        case WeaponType.Club:
                        case WeaponType.Gloves:
                        case WeaponType.Claws:
                            attackState.CapableOfMelee = true;
                            break;
                        case WeaponType.Mage_Staff:
                            attackState.CapableOfMagic = true;
                            break;
                        case WeaponType.Bow:

                        case WeaponType.Pistol:
                            attackState.CapableOfProjectile = true;
                            break;
                        case WeaponType.SpellBlade:
                        case WeaponType.SpellBook:
                            attackState.CapableOfMagic = true;
                            attackState.CapableOfMelee = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (!item.Value.AllSpells.IsNullOrEmpty())
                        attackState.CapableOfMagic = true;
                }

                EntityManager.RemoveComponent<CheckAttackStatus>(entity);
            }).Run();
        }
    }
}