using System;
using System.Collections;
using DreamersInc.ReverbCity.GameCode.UI;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.UIToolkitHelpers;
using UnityEngine;
using UnityEngine.UIElements;
using static DreamersInc.ReverbCity.GameCode.UI.UIExtensionMethods;
namespace DreamersInc.ReverbCity.UI
{
    public class UISceneManager : UIManager
    {
        private VisualElement rootElement;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ServiceLocator.Global.Register(GetType(), this);
            StartCoroutine(Generate());
        }

        public override IEnumerator Generate()
        {
            StartCoroutine(base.Generate());
            rootElement = Create("Background");
            
            Root.Add(rootElement);
            yield return null;
        }

        private void OnDisable()
        {
            ServiceLocator.Global.Unregister(GetType(), this);
        }
    }

    public class PopUpPanel : VisualElement
    {
        public PopUpPanel()
        {
            AddToClassList("PopUpPanel");
        }
    }
}