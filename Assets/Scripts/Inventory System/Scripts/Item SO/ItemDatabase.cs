using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dreamers.InventorySystem.Interfaces;
using Unity.Mathematics;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using Dreamers.Global;
#endif

namespace Dreamers.InventorySystem
{
    public static class ItemDatabase 
    {
        private static List<ItemBaseSO> items;
        private static bool isLoaded { get; set; }

        private static void ValidateDatabase() {
            if (items == null||!isLoaded )
            {
                items = new List<ItemBaseSO>();
                isLoaded = false;
            }
            else { isLoaded = true; }
        }

        public static void LoadDatabase()
        {
            if (isLoaded)
                return;
            LoadDatabaseForce();
        }

        public static void LoadDatabaseForce()
        {
            items = new List<ItemBaseSO>();
            isLoaded = true;
            ItemBaseSO[] itemsToLoad = Resources.LoadAll<ItemBaseSO>("Item Database");
            foreach (var item in itemsToLoad)
            {
                if (!items.Contains(item))
                    items.Add(item);
            }
        }
        public static void ClearDatabase() {
            isLoaded = false;
            items.Clear();

        }
        public static ItemBaseSO GetItem(SerializableGuid SpawnID) {
            ValidateDatabase();
            LoadDatabase();
            foreach (ItemBaseSO item in items)
            {
                if (item.ItemID == SpawnID)
                    return ScriptableObject.Instantiate(item) as ItemBaseSO;
                // Consider add switch to return Item as it derived type ?????

            }
            return null;
        }
        
#if UNITY_EDITOR
        public static class Creator {
            private const string GeneralFolderPath = "Assets/Prefab Library/Resources/Item Database/General";

            [MenuItem("Assets/Create/RPG/Recovery Item")]
            public static void CreateRecoveryItem()
            {
                ScriptableObjectUtility.CreateAsset<RecoveryItem>(GeneralFolderPath,"Item", out RecoveryItem Item);
                ItemDatabase.LoadDatabaseForce();
                Debug.Log( Item.ItemID );
                AssetDatabase.SetLabels(Item, new [] {"Item","Curative"});
                // need to deal with duplicate itemID numbers 

            }
            private const string ArmorFolderPath = "Assets/Prefab Library/Resources/Item Database/Weapons";

            [MenuItem("Assets/Create/RPG/Armor Item")]
            public static void CreateArmorItem()
            {
                ScriptableObjectUtility.CreateAsset<ArmorSO>(ArmorFolderPath,"Armor", out ArmorSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID();
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Item","Equippable","Armor"});
                // need to deal with duplicate itemID numbers 

            }
            private const string WeaponFolderPath = "Assets/Prefab Library/Resources/Item Database/Weapons";

            [MenuItem("Assets/Create/RPG/Weapon/Melee Item")]
            public static void CreateWeaponItem()
            {
                ScriptableObjectUtility.CreateAsset<MeleeWeaponSO>(WeaponFolderPath,"Weapon", out MeleeWeaponSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID();
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Item","Equippable","Weapon"});
                // need to deal with duplicate itemID numbers 

            }

            [MenuItem("Assets/Create/RPG/Weapon/Spell Weapon Item")]
            public static void CreateWeaponSpellItem()
            {
                ScriptableObjectUtility.CreateAsset<SpawnedWeaponSpellSO>(WeaponFolderPath,"SpellWeapon", out SpawnedWeaponSpellSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID();
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new [] {"Item","Equippable","Weapon","Spawn Spell"});

                // need to deal with duplicate itemID numbers 

            }
            private const string SpellFolderPath = "Assets/Prefab Library/Resources/Item Database/Spells";
            [MenuItem("Assets/Create/RPG/Spell Item")]
            public static void CreateSpellItem()
            {
         

                ScriptableObjectUtility.CreateAsset<SpellSO>(SpellFolderPath,"Spell", out SpellSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID();
                Debug.Log(Item.ItemID);
                // need to deal with duplicate itemID numbers 
                AssetDatabase.SetLabels(Item, new [] {"Item","Spell","Weapon", });

            }
  

            [MenuItem("Assets/Create/RPG/Weapon/Projectile Weapon")]
            public static void CreateProjectileWeapon()
            {
                ScriptableObjectUtility.CreateAsset<ProjectileWeaponSO>(WeaponFolderPath,"Projectile Weapon", out ProjectileWeaponSO Item);
                ItemDatabase.LoadDatabaseForce();
                Item.setItemID();
                Debug.Log(Item.ItemID);
                AssetDatabase.SetLabels(Item, new[] { "Item", "Equippable", "Weapon" });
                // need to deal with duplicate itemID numbers 
            }

        }
#endif
    }
}