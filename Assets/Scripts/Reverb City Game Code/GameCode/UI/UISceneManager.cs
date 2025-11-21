using System;
using System.Collections;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.Trackers;
using DreamersInc.UIToolkitHelpers;
using UnityEngine;
using UnityEngine.UIElements;

using DreamersInc.ReverbCity.GameCode.UI;
using Unity.Properties;


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
        private ScoreCounter scoreCounter;
        [CreateProperty] protected string ScoreProperty=>$"Score: {scoreCounter.CurrentValue}";
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            UIManager.RegisterUI(UIType.HUD, document);
            scoreCounter = new ScoreCounter(0);
            StartCoroutine(Generate());
            
        }

        public override IEnumerator Generate()
        {
            rootElement = document.rootVisualElement;
            var label = UIExtensionMethods.Create<Label>("ScoreLabel");
            rootElement.Add(label);
            label.dataSource = this;
            label.SetBinding(nameof(Label.text), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(ScoreProperty)),
                bindingMode = BindingMode.ToTarget
            });
            StartCoroutine(base.Generate());
            scoreCounter.Start();
      
            yield return null;
        }

        private void OnDisable()
        {
            ServiceLocator.Global.Unregister(GetType(), this);
        }
    }

}