using System.Collections.Generic;
using DreamersInc.MoonShot.GameCode.UI;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using Slot = Dreamers.InventorySystem.Interfaces.WeaponSlot;

namespace DreamersInc.MoonShot.GameCode.UIElements.CustomVisualElements
{
    public class WeaponEnchantment : VisualElement
    {
        private readonly Image weaponIcon;
        private Label weaponName, weaponDescription;
        private List<Button> skills;
        private List<Button> spells;
        
        public WeaponEnchantment()
        {
            skills = new List<Button>();
            spells = new List<Button>();
            var column = UIExtensionMethods.Create("SpellView");
            var weaponStatusContainer = UIExtensionMethods.Create("WeaponContainer");
            var weaponTextContainer = UIExtensionMethods.Create("WeaponTextContainer");
            weaponName = UIExtensionMethods.Create<Label>("weaponTextHeader");
            weaponDescription = UIExtensionMethods.Create<Label>("weaponTextBody");
            weaponIcon = UIExtensionMethods.Create<Image>("icon");
            
            var close = UIExtensionMethods.Create<Button>();
            close.text = "Close";
            close.RegisterCallback<ClickEvent>(cvt=> AddToClassList("hide"));
            //Inherited Spells
            var inherit = UIExtensionMethods.Create("SpellContainer");
            var equip = UIExtensionMethods.Create("SpellContainer");
            var label1 = UIExtensionMethods.Create<Label>("SpellHeader");
            label1.text = "Weapon Skills";
            var label2 = UIExtensionMethods.Create<Label>("SpellHeader");
            label2.text = "Spells";
            for (int i = 0; i < 4; i++)
            {
                var temp = CreateSpellSlot();
                skills.Add(temp);
                inherit.Add(temp);
            }
            //Equippable
            for (int i = 0; i < 8; i++)
            {
                var temp = CreateSpellSlot();
                spells.Add(temp);
                equip.Add(temp);
            }
            Add(weaponStatusContainer);
            
            weaponStatusContainer.Add(weaponIcon);
            
            weaponTextContainer.Add(weaponName);
            weaponTextContainer.Add(weaponDescription);
            weaponStatusContainer.Add(weaponTextContainer);
            column.Add(label1);
            column.Add(inherit);
            column.Add(label2);
            column. Add(equip);
            Add(column);
            column.Add(close);
        }
        Button CreateSpellSlot()
        {
            var button = UIExtensionMethods.Create<Button>("SpellSlot");

            return button;
        }
        public Slot ActiveSlot { get=> activeSlot;
            set
            {
                if (activeSlot == value) return;
                activeSlot = value;
                OnActiveSlotChange();
            }
        }

        private void OnActiveSlotChange()
        {
            weaponModel.WeaponBeingEnchanted = activeSlot;
            foreach (var spell in spells)
            {
                //spell.Unbind();
                spell.style.backgroundImage = null;
            }

            foreach (var spell in skills)
            {
               //spell.Unbind();
                spell.style.backgroundImage = null;
            }
            Debug.Log(weaponModel.NumberOfSkills);
            for (int i = 0; i < weaponModel.NumberOfSkills; i++)
                BindWeaponSkillButton(skills[i],i);  
            for (int i = 0; i < weaponModel.NumberOfSpells; i++)
                BindSpellButton(spells[i],i);
        }

        private Slot activeSlot;
        private WeaponViewModel weaponModel;
        
        public void Bind(WeaponViewModel model)
        {
            this.weaponModel = model;
            weaponName.dataSource=weaponDescription.dataSource=weaponIcon.dataSource = model;
            weaponIcon.SetBinding(nameof(Image.image), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(WeaponViewModel.CurrentIcon)),
                bindingMode = BindingMode.ToTarget
            });
            
            weaponName.SetBinding(nameof(Label.text), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(WeaponViewModel.CurrentWeaponName)),
                bindingMode = BindingMode.ToTarget
            });
            
            weaponDescription.SetBinding(nameof(Label.text), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(WeaponViewModel.CurrentWeaponDescription)),
                bindingMode = BindingMode.ToTarget
            });
            for (int i = 0; i < model.NumberOfSkills; i++)
                BindWeaponSkillButton(skills[i],i);  
            for (int i = 0; i < model.NumberOfSpells; i++)
                BindSpellButton(spells[i],i);
        }

        private void BindSpellButton(Button slot,int index)
        {
            slot.dataSource = weaponModel;
            slot.SetBinding("style.backgroundImage", new DataBinding()
            {
                dataSourcePath = new PropertyPath( $"{nameof(WeaponViewModel.WeaponSpellIcons)}[{index}]") ,
                bindingMode = BindingMode.ToTarget
            });
        }

        private void BindWeaponSkillButton(Button slot,int index)
        {
            slot.dataSource = weaponModel;
            slot.SetBinding("style.backgroundImage", new DataBinding()
            {
                dataSourcePath = new PropertyPath( $"{nameof(WeaponViewModel.WeaponSkillIcons)}[{index}]") ,
                bindingMode = BindingMode.ToTarget
            });
        }

    }

}