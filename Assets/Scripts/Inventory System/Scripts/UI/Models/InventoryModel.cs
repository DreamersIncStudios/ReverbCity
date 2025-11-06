using System;
using System.Collections;
using System.Collections.Generic;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public class InventoryModel
    {
       public ObservableArray<ItemDetails> Items { get; set; }
       public event Action<ItemDetails[]> OnModelChanged
       {
           add => Items.AnyValueChanged += value;
           remove => Items.AnyValueChanged -= value;
       }

       public InventoryModel(IEnumerable<ItemBaseSO> items, int capacity)
       {
           Items = new ObservableArray<ItemDetails>(capacity);
           foreach (var item in items)
           {
               Items.TryAdd(item.Create(1)); // consider adding create method??????
           }
       }
       
       public ItemDetails Get(int index) => Items[index];
       

       public void Clear() => Items.Clear();
       public bool Add(ItemDetails item) => Items.TryAdd(item);
       public bool Remove(int index) => Items.TryRemoveAt(index);
       public bool Remove(ItemDetails index) => Items.TryRemove(index);
       
       public void Swap(int source, int target)=> Items.Swap(source, target);

       public int Combine(int source, int target)
       {
           var total = Items[source].quantity + Items[target].quantity;
           Items[target].quantity = total;
           Remove(Items[source]);
           return total;
       }
    }

    public class InventoryController
    {
        private readonly InventoryModel model;
        private readonly EquipmentModel equipment;
        public readonly InventoryView View;
        public readonly EquipmentView EquipmentView;
        private readonly int capacity;
        private readonly PauseView pauseView;
        protected static ItemDisplayPanel InfoPanel;
        private InventoryController(InventoryScreen view, InventoryModel model, EquipmentModel equipmentModel,
            PauseView pauseView, int capacity)
        {
            Debug.Assert(pauseView, "View is null");
            Debug.Assert(model != null, "Model is null");
            Debug.Assert(capacity >0, $"{capacity}Capacity is less than 1");
            this.View = view.InventoryView;
            this.EquipmentView = view.EquipmentView;
            this.model = model;
            this.equipment = equipmentModel;
            this.capacity = capacity;
            this.pauseView = pauseView;
            pauseView.StartCoroutine(Initialize());
        }


        IEnumerator Initialize()
        {
          
          View.RegisterCallback<PointerDownEvent>(OnClickOutsideOfInfoPanel);
          EquipmentView.RegisterCallback<PointerDownEvent>(OnClickOutsideOfInfoPanel);
          
          InfoPanel = new ItemDisplayPanel();
          InfoPanel.AddTo(View.parent);
          InfoPanel.BringToFront();
          
          model.OnModelChanged += HandleModelChange;
          equipment.OnWeaponModelChanged += HandleModelChange;
          
          View.parent.Q<ItemDisplayPanel>().BindModel(model);
        
          RefreshView();
          RefreshWeaponSlotDisplay();
          yield return null;
        }



        private void RefreshView()
        {
            for (var i = 0; i < capacity; i++)
            {
                var item = model.Get(i);
                if (item == null || item.Id.Equals(SerializableGuid.Empty))
                {
                    View.Slots[i].Set(SerializableGuid.Empty, null);
                }
                else
                {
                    View.Slots[i].Set(item.Id, item.details.Icon, item.quantity);;
                    View.Slots[i].OnClickDown += (pos, slot) => OnPointerDown(pos, slot);
                
                }
            }
        }
        
        private void RefreshWeaponSlotDisplay()
        {
            for (var i = 0; i < 3; i++)
            {
                var item = equipment.Get(i);
                if (item == null || item.Id.Equals(SerializableGuid.Empty))
                {
                    EquipmentView. WeaponSlots[i] .Set(SerializableGuid.Empty, null);
                }
                else
                {
                    EquipmentView.WeaponSlots[i].Set(item.Id, item.details.Icon, item.quantity);;
                    EquipmentView.WeaponSlots[i].OnClickDown += (pos, slot) => OnEquipmentPointerDown(pos, slot);  

                }
            }
            for (var i = 0; i < 4; i++)
            {
                var item = equipment.Get(i+3);
                if (item == null || item.Id.Equals(SerializableGuid.Empty))
                {
                    EquipmentView. ArmorSlots[i] .Set(SerializableGuid.Empty, null);
                }
                else
                {
                    EquipmentView.ArmorSlots[i].Set(item.Id, item.details.Icon, item.quantity);;
                    EquipmentView.ArmorSlots[i].OnClickDown += (pos, slot) => OnEquipmentPointerDown(pos, slot);  

                }
            }
        }
        
        private static bool isDisplayed;

        void OnPointerDown(Vector2 pos, Slot slot)
        {
            isDisplayed = true;
     
  
            SetInfoPanelPosition(pos);
            var item = model.Get(slot.Index);
            InfoPanel.DisplayItemInfo(item);
            
            // InfoPanel.style.backgroundImage = originSlot.BaseSprite.texture;
            // originSlot.Icon.image = null;
            // originSlot.StackLabel.visible = false;
            //  
          
                   
            InfoPanel.style.visibility = Visibility.Visible;
             
        }        
        void OnEquipmentPointerDown(Vector2 pos, Slot slot)
        {
            isDisplayed = true;
     
  
            SetInfoPanelPosition(pos);
            var item = equipment.Get(slot.Index);
            InfoPanel.DisplayItemInfo(item);
            
            // InfoPanel.style.backgroundImage = originSlot.BaseSprite.texture;
            // originSlot.Icon.image = null;
            // originSlot.StackLabel.visible = false;
            //  
          
                   
            InfoPanel.style.visibility = Visibility.Visible;
             
        }
        void OnClickOutsideOfInfoPanel(PointerDownEvent evt)
        {
            if(!isDisplayed) return;
            if(InfoPanel.worldBound.Contains(evt.position)) return;
            isDisplayed = false;
            InfoPanel.style.visibility = Visibility.Hidden;
            InfoPanel.Clear();
        }

        private static void SetInfoPanelPosition(Vector2 position)
        {
            InfoPanel.style.top = position.y - InfoPanel.layout.height ;
            InfoPanel.style.left = position.x - InfoPanel.layout.width/4 ;
        }
        private void HandleModelChange(IList<ItemDetails> items)
        {
            RefreshView();
            RefreshWeaponSlotDisplay();
        }

        #region Builder

        public class Builder
        {
            PauseView pauseView;
            private InventoryScreen view;
            private EquipmentModel equipment;
            private IEnumerable<ItemBaseSO> Items;
            private InventoryModel model;
            private int capacity = 50;

            public Builder(PauseView pauseView)
            {
                this.pauseView = pauseView;
            }
            
            public Builder WithStartingItems( IEnumerable<ItemBaseSO> items)
            {
                this.Items = items;
                model = Items != null
                    ? new InventoryModel(Items, capacity)
                    : new InventoryModel(Array.Empty<ItemBaseSO>(), capacity);
                return this;
            }
            public Builder WithStartingItems( InventoryModel items)
            {
                this.model = items;
                return this;
            }
            public Builder WithCapacity(int Capacity)
            {
                this.capacity = Capacity;
                return this;
            }
            
            public Builder WithEquipmentModel(EquipmentModel equipment)
            {
                this.equipment = equipment;
                return this;
            }


            public InventoryController Build()
            {
                view = new InventoryScreen(capacity);

                return new InventoryController(view, model, equipment, pauseView, capacity);
            }
        }

        #endregion
        
    }
}