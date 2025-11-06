using System.Collections.Generic;
using System.Linq;
using Dreamers.InventorySystem.Interfaces;
using Dreamers.InventorySystem.AbilitySystem;
using DreamersInc.ServiceLocatorSystem;
using Sirenix.Utilities;
using UnityEngine;

namespace Dreamers.InventorySystem.Base {
    [System.Serializable]
    public class InventoryBase
    {
        [SerializeField] List<ItemBaseSO> ItemsInInventory;
        public uint MaxInventorySize;
   
        public bool OverBurdened => ItemsInInventory.Count >= MaxInventorySize;
        public InventoryBase() 
        {
            ItemsInInventory = new List<ItemBaseSO>();
        }

        public void Init(uint size) {
            MaxInventorySize = size;
        }
        public void Init(InventorySave save) { 
            MaxInventorySize= save.MaxInventorySize;
           
            if (save.ItemsInInventory.IsNullOrEmpty()) return;
         ItemsInInventory = new List<ItemBaseSO>(save.ItemsInInventory);

        }
        

        public List<ItemBaseSO> GetItemsByType(ItemType Type) {
            var ItemByType = new List<ItemBaseSO>();
            foreach(var Slot in ItemsInInventory)
            {
                if(Type == ItemType.None)
                    ItemByType.Add(Slot);
                else if (Slot.Type == Type)
                {
                    ItemByType.Add(Slot);
                }
            }
            return ItemByType;
        }

        public InventorySave GetInventorySave() {
            var save = new InventorySave(ItemsInInventory,MaxInventorySize );
            save.MaxInventorySize = MaxInventorySize;
            save.ItemsInInventory = ItemsInInventory;
            return save;
        }

        public void LoadInventory(InventorySave inventorySave) {
            MaxInventorySize = inventorySave.MaxInventorySize;
            ItemsInInventory = inventorySave.ItemsInInventory;
        }

     
        public bool AddToInventory(ItemBaseSO item )
        {
            ItemsInInventory.Add(item);
      
            return false;
        }
        private void AddNew(ItemBaseSO item) {
            ItemsInInventory.Add( item);
        
        }

        //public bool AddToInventory(int itemID) {
        //   return AddToInventory( ItemDatabase.GetItem(itemID));
        //}

        //public bool RemoveFromInventory(int index) {
        //    return RemoveFromInventory(ItemDatabase.GetItem(index));
        //}
        public bool RemoveFromInventory(ItemBaseSO item)
        {
            if (item.Type == ItemType.Quest) return false;
            foreach (var slot in ItemsInInventory.Where(slot => slot.ItemID.Equals( item.ItemID)))
            {
                ItemsInInventory.Remove(slot);
                return true;
            }

            return false;
        }
        

        public bool OpenSlot { get { return ItemsInInventory.Count < MaxInventorySize; } }
   
    }


    [System.Serializable]
    public class InventorySave {

        //Todo Add funtionality to load
        public List<ItemBaseSO> ItemsInInventory;
        public uint MaxInventorySize;

        public InventorySave(List<ItemBaseSO> itemsInInventory, uint maxInventorySize)
        {
            this.MaxInventorySize = maxInventorySize;
            this.ItemsInInventory = itemsInInventory;
        }
    }
}