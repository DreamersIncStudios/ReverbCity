using System;
using System.Collections.Generic;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public class EquipmentModel
    {
        public ObservableArray<ItemDetails> Equipment { get; set; }

        public event Action<ItemDetails[]> OnWeaponModelChanged
        {
            add => Equipment.AnyValueChanged += value;
            remove =>Equipment.AnyValueChanged -= value;
        }
        
        public EquipmentModel(IEnumerable<ItemBaseSO> weapons,IEnumerable<ItemBaseSO> armors, int capacity = 7)
        {
            Equipment = new ObservableArray<ItemDetails>(capacity);
            foreach (var item in weapons)
            {
                Equipment.TryAdd(item.Create(1)); // consider adding create method??????
            }
            foreach (var item in armors)
            {
                Equipment.TryAdd(item.Create(1)); // consider adding create method??????
            }
        }
        public ItemDetails Get(int index) => Equipment[index];
       

        public void Clear() => Equipment.Clear();
        public bool Add(ItemDetails item) => Equipment.TryAdd(item);
        public bool Remove(int index) => Equipment.TryRemoveAt(index);
        public bool Remove(ItemDetails index) => Equipment.TryRemove(index);
       
        public void Swap(int source, int target)=> Equipment.Swap(source, target);

    }
}