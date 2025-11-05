using System.Collections.Generic;
using UnityEngine.UIElements;

namespace DreamersInc.UIToolkitHelpers
{
    public static  class UIManager
    {
        private static readonly Dictionary<UIType, UIDocument> uiDocuments = new Dictionary<UIType, UIDocument>();
        
        public static void RegisterUI(UIType type, UIDocument document) => uiDocuments.Add(type, document);
        public static void DeregisterUI(UIType type) => uiDocuments.Remove(type);
        public static UIDocument GetUI(UIType type) => uiDocuments[type];
        
    }

    public enum UIType
    {
        MainMenu,
        GameMenu,
        HUD,
        PauseMenu,
    }
}