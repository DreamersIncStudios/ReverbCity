using Dreamers.InventorySystem;
using Stats.Entities;
using UnityEngine.UIElements;

 namespace DreamersInc.MoonShot.GameCode.UI
 {
     public class InventoryView : StorageView
     {
         
 
         public InventoryView(int size = 40)
         {
                       
             Slots = new Slot[size];
             Container =this.CreateChild("container");
             var inventoryUI = Container.CreateChild("inventory");
             inventoryUI.CreateChild("inventoryFrame");
             inventoryUI.CreateChild("inventoryHeader").Add(new Label(PanelName));
             var slotContainer = inventoryUI.CreateChild("slotContainer");
             for (var i = 0; i < size; i++)
             {
                 var slot = slotContainer.CreateChild<Slot>("slot");
                 Slots[i] = slot;
             }
  
         }

        
         
     }
 }
 
 