using System;
using System.Collections;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.UIToolkitHelpers;
using UnityEngine;
using UnityEngine.UIElements;
namespace DreamersInc.ReverbCity.UI
{
    public class UISceneManager : UIController
    {
        private VisualElement rootElement;
        [SerializeField]
        private string headerText;
        [SerializeField]
        private string bodyText;
        [SerializeField]
        private string buttonText;
        [SerializeField]
        private Action buttonAction;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            UIManager.RegisterUI(UIType.HUD, document);
            ServiceLocator.Global.Register(GetType(), this);
            StartCoroutine(Generate());
        }

        public override IEnumerator Generate()
        {
            StartCoroutine(base.Generate());
     
      
            yield return null;
        }

        private void OnDisable()
        {
            ServiceLocator.Global.Unregister(GetType(), this);
        }
    }

}