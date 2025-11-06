using DreamersInc.MoonShot.GameCode.UIElements.CustomVisualElements;
using UnityEngine.UIElements;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public interface ITooltips
    {
        void SetParent(VisualElement Parent);
    }

    public class StatsTooltips : VisualElement, ITooltips
    {
        public void SetParent(VisualElement parent)
        {
            parent.RegisterCallback<BlurEvent>(cvt=> AddToClassList("hide"));
            parent.RegisterCallback<FocusEvent>(cvt=> RemoveFromClassList("hide"));
        }
    }
    public class EquipTooltips : VisualElement, ITooltips
    {
        public void SetParent(VisualElement parent)
        {
            parent.RegisterCallback<BlurEvent>(cvt=> AddToClassList("hide"));
            parent.RegisterCallback<FocusEvent>(cvt=> RemoveFromClassList("hide"));
        }
    }
    public class ItemTooltips : VisualElement, ITooltips
    {
        public void SetParent(VisualElement parent)
        {
            parent.RegisterCallback<BlurEvent>(cvt=> AddToClassList("hide"));
            parent.RegisterCallback<FocusEvent>(cvt=> RemoveFromClassList("hide"));
        }

        public void Bind(WeaponViewModel model)
        {
        }
    }
}