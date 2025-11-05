using UnityEngine.UIElements;

namespace DreamersInc.ReverbCity.GameCode.UI
{
    public  static class UIExtensionMethods
    {
        public static VisualElement Create(params string[] classNames)
        {
            return Create<VisualElement>(classNames);
        }

        public static T Create<T>(params string[] classNames) where T : VisualElement, new()
        {
            var ele = new T();
            foreach (var className in classNames)
            {
                ele.AddToClassList(className);
            }

            return ele;
        }
        public static VisualElement CreateChild(this VisualElement parent, params string[] classes)
        {
            var child = Create(classes);
            child.AddTo(parent);
            return child;
        }
    
        public static T CreateChild<T>(this VisualElement parent, params string[] classes) where T : VisualElement, new()
        {
            var child = Create<T>(classes);
            child.AddTo(parent);
            return child;
        }

        public static T AddTo<T>(this T child, VisualElement parent) where T : VisualElement {
            parent.Add(child);
            return child;
        }

        public static T AddClass<T>(this T visualElement, params string[] classes) where T : VisualElement {
            foreach (string cls in classes) {
                if (!string.IsNullOrEmpty(cls)) {
                    visualElement.AddToClassList(cls);
                }
            }
            return visualElement;
        }
    }
}