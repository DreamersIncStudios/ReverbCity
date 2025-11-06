using System.Collections.Generic;
using UnityEngine;
using Stats;
using Dreamers.InventorySystem.Base;
using Dreamers.InventorySystem.Interfaces;
using System.Linq;
using DreamersInc.CombatSystem;
using DreamersInc.DamageSystem;
using Stats.Entities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine.Serialization;

namespace Dreamers.InventorySystem
{
    public class WeaponSO : ItemBaseSO, IEquipable, IWeapon
    {
        #region Variables

        [Header("Weapon Data")]
        [SerializeField] private WeaponType weaponType;
        [SerializeField] private WeaponSlot slot;
        [SerializeField] private bool upgradeable;


        [SerializeField] Quality quality;
        [SerializeField] GameObject model;

        [Header("Equipment Data")]
        [SerializeField] private uint levelRqd;
        [SerializeField] private bool equipToHuman;    
        [SerializeField] bool alwaysDrawn;
        [SerializeField] private Vector3 sheathedPos;
        [SerializeField] private Vector3 heldPos;
        [SerializeField] private Vector3 sheathedRot;
        [SerializeField] private Vector3 heldRot;

        [SerializeField] private Vector3 styleHeldPost;
        [SerializeField] Vector3 styleHeldRot;

        
        [SerializeField] [ShowIf(nameof(equipToHuman))] private HumanBodyBones heldBone;
        [SerializeField]  [ShowIf(nameof(equipToHuman))] private HumanBodyBones equipBone;
        
        [Header("Stat Data")]
        [SerializeField] private List<AttributeModifier> modifiers;

        [Header("Spell Data")]
        [SerializeField] private uint spellSlotLevel; // Same as Weapon level??
        [FormerlySerializedAs("weaponInheritedSpells")] public List<SpellSO> WeaponInheritedSpells;

        public List<SpellSO> EquippableSpells;
        public List<SpellSO> AllSpells {
            get
            {
                var spells = new List<SpellSO>();
                spells.AddRange(WeaponInheritedSpells);
                spells.AddRange(EquippableSpells);
                return spells;
            }
        }

        
        protected new ItemType Type => ItemType.Weapon;
        public Quality Quality => quality;

        public bool EquipToHuman => equipToHuman;
        public HumanBodyBones HeldBone => heldBone;
        public bool Equipped { get; private set; }

        public HumanBodyBones EquipBone => equipBone;
        public List<AttributeModifier> Modifiers => modifiers;
      
        public uint LevelRqd => levelRqd;
        public WeaponType WeaponType => weaponType;
        public WeaponSlot Slot => slot;
        public bool Upgradeable => upgradeable;

        public bool AlwaysDrawn => alwaysDrawn;
        public Vector3 SheathedPos => sheathedPos;
        public Vector3 HeldPos => heldPos;
        public Vector3 SheathedRot => sheathedRot;
        public Vector3 HeldRot => heldRot;
        public Vector3 StyleHeldPost => styleHeldPost;
        public Vector3 StyleHeldRot => styleHeldRot;
        
        public SpellSO ActiveSpell { get; private protected set; }

        #endregion

        public GameObject WeaponModel { get; set; }
        protected WeaponDamage weaponDamage;
        
        public BaseCharacterComponent CharacterEquipped { get; private set; }
        public virtual bool Equip(ref BaseCharacterComponent player)
        {

            
            var anim = player.GORepresentative.GetComponent<Animator>();
            CharacterEquipped = player;
            player.GORepresentative.GetComponent<WeaponEventTrigger>().Set(ref player);
            
            if (player.Level >= LevelRqd)
            {
                if (model)
                {
                    WeaponModel = Instantiate(model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        var bone = anim.GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            WeaponModel.transform.SetParent(bone);
                        }
                        if (weaponType == WeaponType.Gloves)
                        {
                            var leftGlove = Instantiate(model, anim.GetBoneTransform(HumanBodyBones.LeftHand), true);
                            leftGlove.transform.localPosition = SheathedPos;
                            leftGlove.transform.localRotation = Quaternion.Euler(SheathedRot);
                        }
                    }
                    else
                    {
                        WeaponModel.transform.SetParent(anim.transform);
                    }
                    WeaponModel.transform.localPosition = SheathedPos;
                    WeaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);
                }
                else
                {
                    WeaponModel = new GameObject();
                 WeaponModel.AddComponent<WeaponDamage>();
                    WeaponModel.transform.SetParent(anim.transform);
                }
                weaponDamage=  WeaponModel.GetComponent<WeaponDamage>();

                player.ModCharacterAttributes(Modifiers);
            
                if(alwaysDrawn )
                {
                    DrawWeapon(anim);
                }
                if(AllSpells.IsNullOrEmpty()) return Equipped = true; 
                 ActiveSpell = AllSpells[0];
                 ActiveSpell.AddSpell(player);
                return Equipped = true; 
            }
            else
            {
                Debug.LogWarning("Level required to Equip is " + LevelRqd + ". Character is currently level " + player.Level);
                return Equipped = false;
            }
        }



        //TODO Should this be a bool instead of Void

        /// <summary>
        /// Equip Item in Inventory to Another Character
        /// </summary>
        /// <param name="characterInventory"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool EquipItem(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            EquipmentBase equipment = characterInventory.Equipment;
            var anim = player.GORepresentative.GetComponent<Animator>();

            if (player.Level >= LevelRqd)
            {
                if (equipment.EquippedWeapons.TryGetValue(this.Slot, out _))
                {
                    equipment.EquippedWeapons[this.Slot].Unequip(characterInventory, player);
                }
                equipment.EquippedWeapons[this.Slot] = this;


                if (model != null)
                {
                    WeaponModel = Instantiate(model);
                    // Consider adding and enum as all character maybe not be human 
                    if (EquipToHuman)
                    {
                        Transform bone = anim.GetBoneTransform(EquipBone);
                        if (bone)
                        {
                            WeaponModel.transform.SetParent(bone);
                        }
                    }
                    else
                    {
                        WeaponModel.transform.SetParent(anim.transform);

                    }
                    WeaponModel.transform.localPosition = SheathedPos;
                    WeaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);
                    
                }  
                else
                {
                    WeaponModel = new GameObject();
                    WeaponModel.AddComponent<WeaponDamage>();
                    WeaponModel.transform.SetParent(anim.transform);
                }


                player.ModCharacterAttributes(Modifiers);
         
                
                characterInventory.Inventory.RemoveFromInventory(this);
                if (alwaysDrawn)
                {
                    anim.SendMessage("EquipWeaponAnim");
                    DrawWeapon(anim);
             
                }

                player.StatUpdate();
                return Equipped = true; 
            }
            else
            {
                Debug.LogWarning("Level required to Equip is " + LevelRqd + ". Character is currently level " + player.Level);
                return Equipped = false;
            }

        }

        /// <summary>
        /// Unequip item from character and return to target inventory
        /// </summary>
        /// <param name="characterInventory"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool Unequip(CharacterInventory characterInventory, BaseCharacterComponent player)
        {
            EquipmentBase equipment = characterInventory.Equipment;
            characterInventory.Inventory.AddToInventory(this);
            Destroy(WeaponModel);

            player.ModCharacterAttributes(Modifiers, false);
            
            equipment.EquippedWeapons.Remove(this.Slot);
            
            Equipped = false;
            return true; 
        }


        public virtual void DrawWeapon(Animator anim)
        {
            if (!equipToHuman) return;
            WeaponModel.transform.SetParent(anim.GetBoneTransform(HeldBone));
            WeaponModel.transform.localPosition = HeldPos;
            WeaponModel.transform.localRotation = Quaternion.Euler(HeldRot);

        }
        public virtual void StoreWeapon(Animator anim)
        {
            WeaponModel.transform.parent = anim.GetBoneTransform(EquipBone);
            WeaponModel.transform.localPosition = SheathedPos;
            WeaponModel.transform.localRotation = Quaternion.Euler(SheathedRot);
        }

        public virtual void StyleChange(bool check)
        {
            if (check)
            {
                WeaponModel.transform.localPosition = styleHeldPost;
                WeaponModel.transform.localRotation = Quaternion.Euler(styleHeldRot);
            }
            else
            {
                WeaponModel.transform.localPosition = HeldPos;
                WeaponModel.transform.localRotation = Quaternion.Euler(HeldRot);
            }
        }

        public void SwapSpell(int index)
        {
            ActiveSpell.RemoveSpell(CharacterEquipped);
            ActiveSpell = AllSpells[index];
            ActiveSpell.AddSpell(CharacterEquipped);
            Debug.Log(ActiveSpell.name + " is now active");
        }

        public bool Equals(ItemBaseSO obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj.Type != Type)
                return false;

            // TODO: write your implementation of Equals() here

            WeaponSO weapon = (WeaponSO)obj;

            return ItemID == weapon.ItemID && ItemName == weapon.ItemName && Value == weapon.Value && Modifiers.SequenceEqual(weapon.Modifiers) &&
                LevelRqd == weapon.LevelRqd;
        }

   
    }


}