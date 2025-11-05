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
            ServiceLocator.Global.Register(GetType(), this);
            StartCoroutine(Generate());
        }

        public override IEnumerator Generate()
        {
            StartCoroutine(base.Generate());
            var popUpPanel = Create<PopUpPanel>();
            popUpPanel.SetText(headerText, bodyText);
            popUpPanel.SetButton(buttonText, buttonAction);
            rootElement.Add(popUpPanel);
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
            AddToClassList("Background");
            var header = Create<Label>("Header");
            var body = Create<Label>("Body");
            var startButton = Create<Button>("StartButton");
            startButton.Focus();
            Add(header);
            Add(body);
            Add(startButton);
        }
        public void SetText(string header, string body)
        {
            var headerLabel = this.Q<Label>("Header");
            var bodyLabel = this.Q<Label>("Body");
            headerLabel.text = header;
            bodyLabel.text = body;
        }
        
        public void SetButton(string text, Action action)
        {
            var button = this.Q<Button>("StartButton");
            button.text = text;
            button.clicked += action;
        }
    }
}