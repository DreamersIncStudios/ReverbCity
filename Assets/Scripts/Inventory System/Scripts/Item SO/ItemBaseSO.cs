using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;
using Stats;
using Stats.Entities;

namespace Dreamers.InventorySystem.Interfaces
{
    [System.Serializable]
    public abstract class ItemBaseSO : ScriptableObject, IItemBase, IPurchasable
    {
        [HorizontalGroup("ItemSplit", 0.75f), VerticalGroup("ItemSplit/Left")]
        [SerializeField] private string _itemName;
                    
        [HorizontalGroup("ItemSplit", 0.75f), VerticalGroup("ItemSplit/Left")]
       public SerializableGuid _itemID =SerializableGuid.NewGuid();
        [TextArea(3, 6)] [SerializeField] private string _desc;
        
        [HorizontalGroup("ItemSplit", 0.25f), VerticalGroup("ItemSplit/Right"), HideLabel, PreviewField(100)]
        [SerializeField] private Sprite _icon;
        [SerializeField] private uint _value;
        [SerializeField] private ItemType _type;
        [SerializeField] bool _questItem;
        [HorizontalGroup("ItemSplit", 0.75f), VerticalGroup("ItemSplit/Left")]
        [SerializeField] private uint maxStackCount=1;
        public int Quantity { get; set; } 

        [Header("Loot Info")]
        
        [SerializeField] private float dropProbability;
        
        [HorizontalGroup("ItemSplit", 0.75f), VerticalGroup("ItemSplit/Left"), Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void setItemID()
        {
            _itemID= SerializableGuid.NewGuid();
        }
        public SerializableGuid ItemID
        {
            get { return _itemID; }
        } // To be implemented with Database system/CSV Editor creator 

      

        public string ItemName
        {
            get { return _itemName; }
        }


        public string Description
        {
            get { return _desc; }
        }


        public Sprite Icon
        {
            get { return _icon; }
        }


        public uint Value
        {
            get { return _value; }
        }


        public ItemType Type
        {
            get { return _type; }
        }


        //[SerializeField] bool _disposible;
        public bool Disposible
        {
            get { return !QuestItem; }
        }


        public bool QuestItem
        {
            get { return _questItem; }
        }

        public uint MaxStackCount
        {
            get { return maxStackCount; }
        }
        public float DropProbability => dropProbability;


        public virtual void Use( CharacterInventory characterInventory,BaseCharacterComponent player)
        {
            Debug.Log(characterInventory.Inventory.RemoveFromInventory(this));
        }

        public ItemDetails Create(int quantity) {
            return new ItemDetails(this, quantity);
        }

        public virtual string Serialize()
        {
            var serializeData = new SerializedItemSO(itemID: ItemID, itemName: ItemName, description: Description,
                value: Value, type: Type, questItem: QuestItem);
            string output = JsonConvert.SerializeObject(serializeData);


            return output;
        }

        public virtual void Deserialize()
        {
        }
    }

    public class SerializedItemSO
        {
            public SerializableGuid ItemID;
            public string ItemName;
            public string Description;
            public uint Value;
            public ItemType Type;
            public bool QuestItem;

            public SerializedItemSO()
            {
            }

            public SerializedItemSO(SerializableGuid itemID, string itemName, string description, uint value, ItemType type,  bool questItem)
            {
                ItemID = itemID;
                ItemName = itemName;
                Description = description;
                Value = value;
                Type = type;
                QuestItem = questItem;
            }
        }
    
}