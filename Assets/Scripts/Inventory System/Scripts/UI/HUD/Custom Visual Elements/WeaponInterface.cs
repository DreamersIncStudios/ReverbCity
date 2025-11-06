using System;
using System.Collections.Generic;
using DreamersInc.MoonShot.GameCode.UI;
using Unity.Properties;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Slot = Dreamers.InventorySystem.Interfaces.WeaponSlot;

namespace DreamersInc.MoonShot.GameCode.UIElements.CustomVisualElements
{
    public class WeaponInterface : VisualElement
    {
        private WeaponSlot projectile;
        public WeaponInterface() {
            AddToClassList("WeaponInterface");
            var primary = UIExtensionMethods.Create<WeaponSlot>( "Primary");
            var secondary = UIExtensionMethods.Create<WeaponSlot>("Secondary");
             projectile = UIExtensionMethods.Create<WeaponSlot>("Projectile");
            Add(primary);
            Add(secondary);
            Add(projectile);
        }
        public void Bind(WeaponViewModel viewModel, PlayerControls controls)
        {
            projectile.Bind(viewModel, Slot.Projectile);
            controls.PlayerController.SpellChange.performed += projectile.SwapSpells;
        }

    }
    public class WeaponSlot : VisualElement
    {
        VisualElement currentBlock;
        private List<VisualElement> blocks;
        public WeaponSlot()
        {blocks = new List<VisualElement>();
            var block = UIExtensionMethods.Create("WeaponSlot");
            Add(block);
            blocks.Add(block);
            currentBlock = block;
        }

        private WeaponViewModel model;
        private int numOfItems;
        public void Bind(WeaponViewModel viewModel, Slot slot)
        {
            model = viewModel;
            currentBlock.dataSource = viewModel;
            var propertyPath = slot switch
            {
                Slot.Primary => new PropertyPath($"{nameof(WeaponViewModel.PrimaryWeaponTextures)}[0]"),
                Slot.Secondary => new PropertyPath($"{nameof(WeaponViewModel.SecondaryWeaponTextures)}[0]"),
                Slot.Projectile => new PropertyPath($"{nameof(WeaponViewModel.ProjectileTextures)}[0]"),
                _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
            };
            
            numOfItems  = slot switch
            {
                Slot.Projectile => viewModel.NumOfProjectiles,
                _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
            };
          
            currentBlock.SetBinding("style.backgroundImage", new DataBinding()
            {
                dataSourcePath = propertyPath,
                bindingMode = BindingMode.ToTarget
            });
            CreateAdditionalSlots(viewModel, slot);

        }
        
        private void CreateAdditionalSlots(WeaponViewModel viewModel, Slot slot)
        {
            for (var i = 1; i < numOfItems; i++)
            {
                var optionalBlock = UIExtensionMethods.Create<VisualElement>("WeaponSlot", "hide");
                Add(optionalBlock);
                optionalBlock.dataSource = viewModel;
                var propertyPath = slot switch
                {
                    Slot.Primary => new PropertyPath($"{nameof(WeaponViewModel.PrimaryWeaponTextures)}[{i}]"),
                    Slot.Secondary => new PropertyPath($"{nameof(WeaponViewModel.SecondaryWeaponTextures)}[{i}]"),
                    Slot.Projectile => new PropertyPath($"{nameof(WeaponViewModel.ProjectileTextures)}[{i}]"),
                    _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
                };
                blocks.Add(optionalBlock);
                optionalBlock.SetBinding("style.backgroundImage", new DataBinding()
                {
                    dataSourcePath = propertyPath,
                    bindingMode = BindingMode.ToTarget
                });
            }
        }

        public void SwapSpells(InputAction.CallbackContext obj)
        {
            currentBlock.AddToClassList("hide");
            var index = blocks.IndexOf(currentBlock)== blocks.Count - 1 ? 0 : blocks.IndexOf(currentBlock) + 1;
            blocks[index].RemoveFromClassList("hide");
            currentBlock = blocks[index];
            model.Projectile.SwapSpell(index);   
        }

    }
}