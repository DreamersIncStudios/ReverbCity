using System.Collections.Generic;
using System.Linq;
using Stats.Entities;
using Dreamers.InventorySystem.Interfaces;
using Sirenix.Serialization;
using UnityEngine;
using Stats;
using Dreamers.InventorySystem.AbilitySystem;
using Unity.Entities;
using Unity.Collections;

namespace Dreamers.InventorySystem.Base {
    [System.Serializable]
    public class EquipmentBase
    {
        public Dictionary<ArmorType, ArmorSO> EquippedArmor = new Dictionary<ArmorType, ArmorSO>();
        public Dictionary<WeaponSlot, WeaponSO> EquippedWeapons = new Dictionary<WeaponSlot, WeaponSO>();

        public int CurrentActivationPoints;
        public int MaxActivationPoints;
        public List<ItemBaseSO> QuickAccessItems;
        public int NumOfQuickAccessSlots;
        public bool OpenSlots { get { return QuickAccessItems.Count < NumOfQuickAccessSlots; } }

        public void Init() { 
            QuickAccessItems= new List<ItemBaseSO>();
            NumOfQuickAccessSlots= 2;
        }
        public void Init(EquipmentSave save, ref BaseCharacterComponent player, Entity entity, int size =2) {
            EquippedArmor = new Dictionary<ArmorType, ArmorSO>();
            EquippedWeapons = new Dictionary<WeaponSlot, WeaponSO>();
            QuickAccessItems = new List<ItemBaseSO>();
            NumOfQuickAccessSlots=  size;
           LoadEquipment(ref player, entity, save);
            

        }
        public EquipmentSave Save;
        public void Init(EquipmentSave save, int size = 2)
        {
            EquippedArmor = new Dictionary<ArmorType, ArmorSO>();
            EquippedWeapons = new Dictionary<WeaponSlot, WeaponSO>();
            QuickAccessItems = new List<ItemBaseSO>();
            NumOfQuickAccessSlots = size;
         Save = save;

        }
        void reloadEquipment(ref BaseCharacterComponent player) {
            foreach (ArmorSO so in EquippedArmor.Values) {
                so.Equip(ref player);
            }
            foreach (WeaponSO so in EquippedWeapons.Values)
            {
                so.Equip(ref player);
            }
        }

        void LoadEquipment(ref BaseCharacterComponent PC, Entity entity, EquipmentSave Save)
        {
            if (Save.EquippedArmors.Count != 0)
            {
                foreach (var copy in from SO in Save.EquippedArmors where SO != null select Object.Instantiate(SO))
                {
                    copy.Equip(ref PC);
                    EquippedArmor[copy.ArmorType] = copy;
                }
            }

            if (Save.EquippedWeapons.Count == 0) return;
            {
                foreach (var copy in from SO in Save.EquippedWeapons where SO != null select Object.Instantiate(SO))
                {
                    copy.Equip(ref PC);
                    EquippedWeapons[copy.Slot] = copy;
                }
            }

            /*if (Save.EquippedAbilites.Count == 0) return;
            foreach (var ability in Save.EquippedAbilites) {
                ability.EquipAbility(entity);
            }*/
        }

    }
    [System.Serializable]
    public class EquipmentSave
    {
        public List<WeaponSO> EquippedWeapons;
        public List<ArmorSO> EquippedArmors;
     
    }

}
