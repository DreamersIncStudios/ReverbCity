using DreamersInc.DamageSystem.Interfaces;
using Sirenix.Utilities;
using Stats.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public class ProjectileWeaponSO : WeaponSO, IProjectileWeapon
    {
        [SerializeField] uint maxNumOfSpells = 3;
        private TypeOfDamage typeOfDamage => TypeOfDamage.Projectile;


        public void EquipSpell(SpellSO spell)
        {
            if (EquippableSpells.Count < maxNumOfSpells)
                EquippableSpells.Add(spell);
        }

        public override bool Equip(ref BaseCharacterComponent player)
        {
            if (!base.Equip(ref player)) return false;
            if (WeaponInheritedSpells.IsNullOrEmpty())
            {
                Debug.LogError("NO Baked Spells");
                return false;
            }


            WeaponModel.GetComponent<IDamageDealer>().SetStatData(player, typeOfDamage);
            return true;
        }

        public override bool Unequip(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            if (!base.Unequip(characterInventory, player)) return false;

            ActiveSpell.RemoveSpell(player);
            return true;
        }


        public override void DrawWeapon(Animator anim)
        {
            base.DrawWeapon(anim);
            anim.SetBool("Projectile Drawn", true);
        }

        public override void StoreWeapon(Animator anim)
        {
            base.StoreWeapon(anim);
            anim.SetBool("Projectile Drawn", false);
        }
    }
}