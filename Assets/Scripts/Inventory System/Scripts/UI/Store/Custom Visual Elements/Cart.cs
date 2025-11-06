using UnityEngine;
using UnityEngine.UIElements;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public class Cart : StorageView
    {
        public Cart(int size = 18)
        {
            Slots = new Slot[size];
            Container =this.CreateChild("container");
            var inventory = Container.CreateChild("inventory");
            inventory.CreateChild("inventoryFrame");
            inventory.CreateChild("inventoryHeader").Add(new Label(PanelName));
            var slotContainer = inventory.CreateChild("slotContainer");
            for (var i = 0; i < size; i++)
            {
                var slot = slotContainer.CreateChild<Slot>("slot");
                Slots[i] = slot;
            }
         
        }
    }
}