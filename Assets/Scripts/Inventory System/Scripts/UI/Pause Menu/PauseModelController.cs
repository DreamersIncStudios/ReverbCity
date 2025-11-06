using System;
using System.Collections;
using System.Collections.Generic;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using Stats.Entities;
using UnityEngine;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public class PauseModel
    {
        public EquipmentModel equipment;
        public InventoryModel inventory;


        public PauseModel(EquipmentModel equipment, InventoryModel inventoryModel, StatsViewModel stats)
        {
            Debug.Assert(equipment != null, "equipment != null");
            Debug.Assert(stats != null, "stats is null");
            Debug.Assert(inventoryModel != null, "inventoryModel != null");
            this.equipment = equipment;
            this.inventory = inventoryModel;
        }
    }

    public class PauseController
    {
        readonly EquipmentModel equipment;
        private readonly InventoryModel inventory;
        readonly int inventoryCapacity;
        private readonly PauseModel model;
        readonly PauseView pauseView;
        BaseCharacterComponent stats;

        private PauseController(PauseView pauseView, PauseModel model, BaseCharacterComponent stats,
            EquipmentModel equipmentModel, InventoryModel inventoryModel)
        {
            Debug.Assert(pauseView, "view is null");

            this.equipment = equipmentModel;
            this.pauseView = pauseView;
            this.model = model;
            this.inventory = inventoryModel;
            this.stats = stats;
            pauseView.StartCoroutine(Initialize());
        }

        IEnumerator Initialize()
        {
            yield return new WaitForSeconds(2);
            yield return pauseView.Generate();
            var status = new StatsController.Builder(pauseView)
                .WithEquipmentModel(equipment)
                .WithCharacter(ref stats)
                .Build();
            var invent = new InventoryController.Builder(pauseView)
                .WithCapacity(50)
                .WithEquipmentModel(equipment)
                .WithStartingItems(inventory)
                .Build();
            pauseView.AddTab("Stats", status.View.parent);
            ;

            pauseView.AddTab("inventory", invent.View.parent);
            ;
            // StatsView.BindCharacter(model.stats);

            RefreshView();
        }

        private void HandleModelChange(IList<ItemDetails> items) => RefreshView();

        void RefreshView()
        {
            RefreshInventorySlotView();
        }

        private void RefreshInventorySlotView()
        {
            // for (var i = 0; i < inventoryCapacity; i++)
            // {
            //     var item = model.inventory.Get(i);
            //     if (item == null || item.Id.Equals(SerializableGuid.Empty))
            //     {
            //         InventoryView.Slots[i].Set(SerializableGuid.Empty, null);
            //     }
            //     else
            //     {
            //         InventoryView.Slots[i].Set(item.Id, item.details.Icon, item.quantity);;
            //         InventoryView.Slots[i].OnClickDown += (pos, slot) => OnPointerDown(pos, slot);
            //     
            //     }
            // }
        }


        void OnPointerDown(Vector2 pos, Slot slot)
        {
        }

        #region Builder

        public class Builder
        {
            private IEnumerable<ItemBaseSO> armor;
            private int capacity;
            private IEnumerable<ItemBaseSO> inventory;
            private PauseView pauseView;
            private BaseCharacterComponent stats;
            private IEnumerable<ItemBaseSO> weapon;

            public Builder(PauseView pauseView, ref BaseCharacterComponent stats)
            {
                this.pauseView = pauseView;
                this.stats = stats;
            }

            public Builder WithStartingInventory(IEnumerable<ItemBaseSO> inventory, int capacity = 50)
            {
                this.inventory = inventory;
                this.capacity = capacity;
                return this;
            }

            public Builder WithStartingWeapon(IEnumerable<ItemBaseSO> equipment)
            {
                this.weapon = equipment;
                return this;
            }

            public Builder WithStartingArmor(IEnumerable<ItemBaseSO> equipment)
            {
                this.armor = equipment;
                return this;
            }

            public PauseController Build()
            {
                var statModel = new StatsViewModel(ref stats);
                weapon ??= Array.Empty<ItemBaseSO>();
                armor ??= Array.Empty<ItemBaseSO>();
                var equipmentModel =
                    new EquipmentModel(weapon, armor);
                InventoryModel inventoryModel = inventory != null
                    ? new InventoryModel(inventory, capacity)
                    : new InventoryModel(Array.Empty<ItemBaseSO>(), capacity);
                var model = new PauseModel(equipmentModel, inventoryModel, statModel);
                return new PauseController(pauseView, model, stats, equipmentModel, inventoryModel);
            }

            public Builder WithPlayer(ref BaseCharacterComponent player)
            {
                stats = player;
                return this;
            }
        }

        #endregion
    }
}