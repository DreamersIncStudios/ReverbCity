using DreamersInc.DamageSystem.Interfaces;
using Sirenix.OdinInspector;
using Stats.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public class MeleeWeaponSO : WeaponSO, IMeleeWeapon
    {
        [SerializeField] private bool breakable;

        [ShowIf("Breakable")] [SerializeField] private float maxDurable;

        private TypeOfDamage typeOfDamage => TypeOfDamage.Melee;
        public bool Breakable => breakable;
        public float MaxDurability => maxDurable;
        public float CurrentDurability { get; set; }

        public override bool Equip(ref BaseCharacterComponent player)
        {
            if (!base.Equip(ref player)) return false;
            WeaponModel.GetComponent<IDamageDealer>().SetStatData(player, typeOfDamage);

            return true;
        }

        public override bool Unequip(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            if (!base.Unequip(characterInventory, player)) return false;


            return true;
        }
    }
}