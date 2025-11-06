using System;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.ServiceLocatorSystem;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public abstract class StorageView : VisualElement
    {
        public Slot[] Slots;
        protected string PanelName = "Inventory";
        protected VisualElement Container;
   
        protected const string HideClass = "hide";
        
        private static bool isDisplayed;
        static Slot originSlot;

        public event Action<Slot, Slot> OnDrop;

        
    }
    public class ItemDisplayPanel : VisualElement
    {
        private Label itemName;
        Label description;
        Label statsEffects;
        private Button button1;
        private Button button2;
        private Button button3;
        BaseCharacterComponent player;
        CharacterInventory inventory;
        public ItemDisplayPanel()
        {
            this.AddClass("infoPanel");
            itemName = this.CreateChild<Label>().AddClass("infoLabel");
            var container1 = this.CreateChild("container");
            container1.style.flexDirection = FlexDirection.Row;
            description = container1.CreateChild<Label>().AddClass("infoLabel");
            statsEffects = container1.CreateChild<Label>().AddClass("infoLabel");
            var container2 = this.CreateChild("container");
            container2.style.flexDirection = FlexDirection.Row;
            button1 = container2.CreateChild<Button>();
            button2 = container2.CreateChild<Button>();
            button3 = container2.CreateChild<Button>();
            this.player = player;
            this.inventory = inventory;
            this.model = model;
        }

        public void BindModel(InventoryModel model)
        {
            this.model = model;
        }

        private InventoryModel model;
        public void DisplayItemInfo(ItemDetails itemDetails)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerEntity = em.CreateEntityQuery(typeof(Player_Control)).GetSingletonEntity();
            var characterInventory = em.GetComponentData<CharacterInventory>(playerEntity);
            var player = em.GetComponentData<BaseCharacterComponent>(playerEntity);
            
            var item = ItemDatabase.GetItem(itemDetails.detailsId);
            itemName.text = item.ItemName;
            description.text = item.Description;
            switch (item.Type)
            {
                case ItemType.None:
                    Debug.LogError($"Item {item.name} has no type");
                    break;
                case ItemType.General:
                    var recovery = (RecoveryItem)item;
                    foreach (IItemAction action in recovery.RecoveryItems)
                    {
                        statsEffects.text += action.Type switch
                        {
                            RecoveryType.Health => $"Health: {action.Amount}\n",
                            RecoveryType.Mana => $"Mana: {action.Amount}\n",
                            RecoveryType.Durability => $"Durability: {action.Amount}\n",
                            RecoveryType.Status => $"Status: {action.Amount}\n" //Todo remove status effects
                            ,
                            RecoveryType.Other => $"Other: {action.Amount}\n",
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }
                    button1.text = "Use";
                    button1.RegisterCallback<ClickEvent>((_)=>
                    {
                         recovery.Use( characterInventory, player);
                         model.Remove(itemDetails);
                    });
                    button2.text = "Drop";
                    button2.RegisterCallback<ClickEvent>((_)=>inventory.Inventory.RemoveFromInventory(item));
                    
                    button3.text = "Equip";
                    button3.RegisterCallback<ClickEvent>((_)=>Debug.Log("equip to be added"));

                    break;
                case ItemType.Weapon:
                    var weapon = (WeaponSO)item;
                    statsEffects.text = $"Required Level: {weapon.LevelRqd}\n Perks:";
                    foreach (var modifier in weapon.Modifiers)
                    {
                        statsEffects.text += $"{modifier.Attribute}:{modifier.BuffValue}\n";
                    }

                    button1.text = "Equip";
                    button1.RegisterCallback<ClickEvent>((_)=> weapon.Equip(ref player));
                    button2.text = "Drop";
                    button2.RegisterCallback<ClickEvent>((_)=>inventory.Inventory.RemoveFromInventory(item));
                    
                    button3.text = "Modify";
                    button3.RegisterCallback<ClickEvent>((_)=>Debug.Log("modify to be added"));
                    
                    break;
                case ItemType.Armor:
                    var armor = (ArmorSO)item;
                    statsEffects.text = $"Required Level: {armor.LevelRqd}\n Perks:";
                    foreach (var modifier in armor.Modifiers)
                    {
                        statsEffects.text += $"{modifier.Attribute}:{modifier.BuffValue}\n";
                    }
              
                    button1.text = "Equip";
                    button1.RegisterCallback<ClickEvent>((_)=> armor.Equip( ref player));
                    button2.text = "Drop";
                    button2.RegisterCallback<ClickEvent>((_)=>inventory.Inventory.RemoveFromInventory(item));
                    
                    button3.text = "Modify";
                    button3.RegisterCallback<ClickEvent>((_)=>Debug.Log("modify to be added"));
                    break;
                case ItemType.Crafting_Materials:
                    //Todo Remove for now???????
                    break;
                case ItemType.Blueprint_Recipes:
                    //Todo Remove for now???????
                    break;
                case ItemType.Quest:
                    statsEffects.text += "Key Item";
                    break;
                case ItemType.CompiledSpell:
                
                    button1.text = "Equip";
                    button1.RegisterCallback<ClickEvent>((_)=> item.Use( inventory,  player));
                    button2.text = "Drop";
                    button2.RegisterCallback<ClickEvent>((_) => inventory.Inventory.RemoveFromInventory(item));
                    
                    button3.text = "Modify";
                    button3.RegisterCallback<ClickEvent>((_)=>Debug.Log("modify to be added"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof(item), $"item {item.ItemName} does not have Type");
            }
        }
        
        public void Clear()
        {
            itemName.text = string.Empty;
            description.text = string.Empty;
            statsEffects.text = string.Empty;
            
        }


    }
}