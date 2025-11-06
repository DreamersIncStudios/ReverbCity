using UnityEngine.UIElements;

namespace DreamersInc.MoonShot.GameCode.UI{
    public class InventoryScreen : VisualElement
    {
       public EquipmentView EquipmentView;
       public  InventoryView InventoryView;
        
        public InventoryScreen(int capacity = 50)
        {
            var tab = this.CreateChild("hide");
            tab.style.flexDirection = FlexDirection.Row;
   
            EquipmentView = new EquipmentView();
            InventoryView  = new InventoryView(capacity); 
            EquipmentView.AddTo(tab);
            InventoryView.AddTo(tab);
        }


    }
}