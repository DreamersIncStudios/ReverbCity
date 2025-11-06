using Dreamers.InventorySystem.Interfaces;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    [System.Serializable]
    public class ItemDetails
    {
        [field: SerializeField] public SerializableGuid Id;
        [field: SerializeField] public SerializableGuid detailsId;
        public ItemBaseSO details;
        public int quantity;

        public ItemDetails(ItemBaseSO details, int quantity = 1)
        {
            Id = SerializableGuid.NewGuid();
            this.detailsId = details.ItemID;
            this.details = details;
            this.quantity = quantity;
        }
    }
}