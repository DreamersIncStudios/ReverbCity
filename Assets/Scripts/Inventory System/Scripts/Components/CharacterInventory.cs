using Dreamers.InventorySystem.Base;
using Stats.Entities;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.MoonShot.GameCode.UI;
using DreamersInc.ServiceLocatorSystem;
using Unity.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public class CharacterInventory : IComponentData
    {
        public PauseController pauseController;
        public PauseView view;
        public uint Gold { get; private set; }

        public InventoryBase Inventory;
        public EquipmentBase Equipment;
        
        public void Setup()
        {
            Inventory = new();
            Inventory.Init(10);
            Equipment = new();
            Equipment.Init();

        }
        public void Setup(InventorySave inventory)
        {
            Inventory = new();
            Inventory.Init(inventory);
            Equipment = new();
            Equipment.Init();

        }
        public void Setup(Entity entity, EquipmentSave equipment, ref BaseCharacterComponent player)
        {
            Inventory = new();
            Inventory.Init(10);
            Equipment = new();
            Equipment.Init(equipment, ref player, entity);

        }
        public void Setup(Entity entity, InventorySave inventory, EquipmentSave equipment, ref BaseCharacterComponent player)
        {
            Inventory = new();
            Inventory.Init(inventory);
            Equipment = new();
            Equipment.Init(equipment, ref player, entity);
            view = ServiceLocator.Global.Get<PauseView>();
            pauseController = new PauseController.Builder(view, ref player)
                .WithStartingWeapon(equipment.EquippedWeapons)
               .WithStartingArmor(equipment.EquippedArmors)
                .WithStartingInventory(inventory.ItemsInInventory)
                .WithPlayer(ref player)
                .Build();

        }

        public void RemoveGold(uint amount)
        {

            if (amount <= Gold)
            {
                Gold = (uint)Mathf.Clamp(Gold - amount, 0, Mathf.Infinity);
            }
        }
        public void AddGold(uint amount)
        {
            Gold = (uint)Mathf.Clamp(Gold + amount, 0, Mathf.Infinity);
        }

        public bool BuyItem(ItemBaseSO item)
        {
            if (item.Value < Gold &&
                Inventory.AddToInventory(ScriptableObject.Instantiate(item)))
            {
                RemoveGold(item.Value);
                return true;
            }
            else
                return false;
        }

        public bool SellItem(ItemBaseSO item)
        {
            if (!item.QuestItem)
            {
                Inventory.RemoveFromInventory(item);
                AddGold(item.Value);
                return true;
            }
            else return false;
        }
    }
    public struct CheckAttackStatus : IComponentData { }
}