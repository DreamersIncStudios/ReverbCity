using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public class Slot : VisualElement
    {
        public Image Icon;
        public Label StackLabel;
        public int Index => parent.IndexOf(this);
        public SerializableGuid ItemID;
        public Sprite BaseSprite;
        
        
        public event Action<Vector2, Slot> OnClickDown = delegate { };        
        public Slot()
        {
            Icon = this.CreateChild<Image>("slotIcon");
            StackLabel = this.CreateChild("slotFrame").CreateChild<Label>("stackCount");
            RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if(evt.button!=0 || ItemID == SerializableGuid.Empty) return;
            OnClickDown.Invoke(evt.position,this);
            evt.StopPropagation();
        }

        public void Set(SerializableGuid id, Sprite icon, int qty = 0)
        {
            ItemID = id;
             
            BaseSprite = icon;
            Icon.image = BaseSprite? icon.texture: null;
             
            StackLabel.text = qty > 1 ? qty.ToString() : string.Empty;
            StackLabel.visible = qty > 1;
        }

        public void Clear()
        {
            ItemID = SerializableGuid.Empty;
            Icon.image = null;
        }
    }
}