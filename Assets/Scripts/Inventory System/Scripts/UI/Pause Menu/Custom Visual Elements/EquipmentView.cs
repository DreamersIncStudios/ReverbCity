using Dreamers.InventorySystem;
using Stats.Entities;
using UnityEngine.UIElements;

namespace DreamersInc.MoonShot.GameCode.UI{
   

   public class EquipmentView: StorageView
    {
        public Slot[] WeaponSlots;
        public Slot[] ArmorSlots;

        public EquipmentView(int size = 7)
        {
            Slots = new Slot[size];
            WeaponSlots = new Slot[3];
            ArmorSlots = new Slot[4];
            Container = this.CreateChild("equipmentContainer");
            // var weapons = Container.CreateChild("weapons");
            // var armors = Container.CreateChild("armors");
            var weaponSlotContainer = Container.CreateChild("slotContainer2");
            var armorSlotContainer = Container.CreateChild("slotContainer2");
            
            for (var i = 0; i < 3; i++)
            {
                var slot = weaponSlotContainer.CreateChild<Slot>("slot");
                WeaponSlots[i]=Slots[i] = slot;
            }
            
            for (var i = 3; i < size; i++)
            {
                var slot = armorSlotContainer.CreateChild<Slot>("slot");
                ArmorSlots[i-3]=Slots[i] = slot;
            }
   
        }
    }
}