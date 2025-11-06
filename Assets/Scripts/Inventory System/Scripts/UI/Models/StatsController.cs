using System.Collections;
using Stats.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace DreamersInc.MoonShot.GameCode.UI
{

public class StatsController
{
    private readonly EquipmentModel equipment;
    private readonly EquipmentView equipmentView;
    public readonly StatsView View;
    public StatsViewModel Stats;
    protected static ItemDisplayPanel InfoPanel;

    private StatsController(PauseView pauseView, StatsScreen view,  StatsViewModel stats, EquipmentModel equipment)
    {

        this.equipment = equipment;
        this.View = view.StatsView;
        this.equipmentView = view.EquipmentView;
        
        this.Stats = stats;
        pauseView.StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    { 
        View.RegisterCallback<PointerDownEvent>(OnClickOutsideOfInfoPanel);
            View.BindCharacter(Stats);
      
          
        InfoPanel = new ItemDisplayPanel();
        InfoPanel.AddTo(View.parent);   
        InfoPanel.BringToFront();
        RefreshWeaponSlotDisplay();
        yield return null;
    }
    
    
    private void RefreshWeaponSlotDisplay()
    {
        for (var i = 0; i < 3; i++)
        {
            var item = equipment.Get(i);
            if (item == null || item.Id.Equals(SerializableGuid.Empty))
            {
                equipmentView. WeaponSlots[i] .Set(SerializableGuid.Empty, null);
            }
            else
            {
                equipmentView.WeaponSlots[i].Set(item.Id, item.details.Icon, item.quantity);;
                equipmentView.WeaponSlots[i].OnClickDown += (pos, slot) => OnPointerDown(pos, slot);  

            }
        }
        for (var i = 0; i < 4; i++)
        {
            var item = equipment.Get(i+3);
            if (item == null || item.Id.Equals(SerializableGuid.Empty))
            {
                equipmentView. ArmorSlots[i] .Set(SerializableGuid.Empty, null);
            }
            else
            {
                equipmentView.ArmorSlots[i].Set(item.Id, item.details.Icon, item.quantity);;
                equipmentView.ArmorSlots[i].OnClickDown += OnPointerDown;  

            }
        }
    }
    
    void OnPointerDown(Vector2 pos, Slot slot)
    {
        isDisplayed = true;
     
  
        SetInfoPanelPosition(pos);
        var item = equipment.Get(slot.Index);
        InfoPanel.DisplayItemInfo(item);
            
        // InfoPanel.style.backgroundImage = originSlot.BaseSprite.texture;
        // originSlot.Icon.image = null;
        // originSlot.StackLabel.visible = false;
        //  
          
                   
        InfoPanel.style.visibility = Visibility.Visible;
             
    }
    private void SetInfoPanelPosition(Vector2 position)
    {
        InfoPanel.style.top = position.y - InfoPanel.layout.height ;
        InfoPanel.style.left = position.x- InfoPanel.layout.width / 2;
    }
    private static bool isDisplayed;
    void OnClickOutsideOfInfoPanel(PointerDownEvent evt)
    {
        if(!isDisplayed) return;
        if(InfoPanel.worldBound.Contains(evt.position)) return;
        isDisplayed = false;
        InfoPanel.style.visibility = Visibility.Hidden;
        InfoPanel.Clear();
    }
    #region builder

    public class Builder
    {
        PauseView pauseView;
        StatsScreen view;
        private BaseCharacterComponent player;
        private EquipmentModel equipment;
        
        public Builder(PauseView pauseView)
        {
            this.pauseView = pauseView;
            this.view = new StatsScreen();
        }

        public Builder WithCharacter(ref BaseCharacterComponent player)
        {
            this.player = player;
            return this;
        }
        public Builder WithEquipmentModel(EquipmentModel equipment)
        {
            this.equipment = equipment;
            return this;
        }

        public StatsController Build()
        {
            var model = new StatsViewModel(ref player);
            return new StatsController(pauseView,view, model, equipment);
        }
    }

    #endregion
}
}
