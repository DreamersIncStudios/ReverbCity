using System.Collections.Generic;
using System.Linq;
using Dreamers.InventorySystem;
using Unity.Properties;
using UnityEngine;
using Slot = Dreamers.InventorySystem.Interfaces.WeaponSlot;

namespace DreamersInc.MoonShot.GameCode.UIElements.CustomVisualElements
{
    public class WeaponViewModel
    {
        private readonly CharacterInventory inventory;
        public Slot WeaponBeingEnchanted;
  
        public WeaponViewModel( ref CharacterInventory inventory)
        {
            this.inventory = inventory;
        }
        [CreateProperty]
        public int NumOfProjectiles
        {
            get
            {
                inventory.Equipment.EquippedWeapons.TryGetValue(
                    Slot.Projectile, out var so);
                return so.AllSpells.Count;
            }
        }

        //Todo for beta release look into composite textures at runtime. 
        [CreateProperty]
        public Texture2D[] ProjectileTextures
        {
            get
            {
                var spells = new Texture2D[NumOfProjectiles];
                inventory.Equipment.EquippedWeapons.TryGetValue(
                    Slot.Projectile, out var so);
                for (var i = 0; i < so.AllSpells.Count; i++)
                {
                    spells[i] = so.AllSpells[i].Icon.texture;
                }

                return spells;
            }
        }

        [CreateProperty]
        public Texture2D[] PrimaryWeaponTextures
        {
            get
            {
                var spells = new Texture2D[NumOfProjectiles];
                inventory.Equipment.EquippedWeapons.TryGetValue(
                    Slot.Projectile, out var so);
                for (var i = 0; i < so.AllSpells.Count; i++)
                {
                    spells[i] = so.AllSpells[i].Icon.texture;
                }

                return spells;
            }
        }

        [CreateProperty]
        public Texture2D[] SecondaryWeaponTextures
        {
            get
            {
                var spells = new Texture2D[NumOfProjectiles];
                inventory.Equipment.EquippedWeapons.TryGetValue(
                    Slot.Projectile, out var so);
                for (var i = 0; i < so.AllSpells.Count; i++)
                {
                    spells[i] = so.AllSpells[i].Icon.texture;
                }

                return spells;
            }
        }

        public WeaponSO Projectile => inventory.Equipment.EquippedWeapons.TryGetValue(
            Slot.Projectile, out var so)
            ? so
            : null;

        [CreateProperty]
        public WeaponSO CurrentWeapon =>
            inventory.Equipment.EquippedWeapons.GetValueOrDefault(WeaponBeingEnchanted);    
  
       
        [CreateProperty]
        public Texture2D CurrentIcon =>
            CurrentWeapon.Icon.texture;

        [CreateProperty]
        public string CurrentWeaponName =>
            CurrentWeapon.ItemName;
        [CreateProperty]
        public string CurrentWeaponDescription =>
            CurrentWeapon.Description;

        [CreateProperty]
        public List<Texture2D> WeaponSkillIcons
        {
            get => CurrentWeapon?.WeaponInheritedSpells?
                .Select(spell => spell.Icon.texture)
                .ToList() ?? new List<Texture2D>();
        }
        [CreateProperty]
        public List<Texture2D> WeaponSpellIcons
        {
            get => CurrentWeapon?.EquippableSpells?
                .Select(spell => spell.Icon.texture)
                .ToList() ?? new List<Texture2D>();
        }

        public int  NumberOfSkills => CurrentWeapon.WeaponInheritedSpells.Count; 
        public int  NumberOfSpells => CurrentWeapon.EquippableSpells.Count; 
        
    }
}