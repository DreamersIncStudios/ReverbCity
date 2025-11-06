using System;
using DreamersInc.ReverbCity.GameCode.UI;
using UnityEngine.UIElements;
namespace DreamersInc.ReverbCity.UI
{
    public class PopUpPanel : VisualElement
    {
        Label header;
        Label body;
        Button startButton;
        public PopUpPanel()
        {
            AddToClassList("PopUpPanel");
            AddToClassList("Background"); 
            header = UIExtensionMethods.Create<Label>("Header");
            body = UIExtensionMethods.Create<Label>("Body");
            startButton = UIExtensionMethods.Create<Button>("StartButton");
            startButton.Focus();
            Add(header);
            Add(body);
            Add(startButton);
        }
        public void SetText(string header, string body)
        {
            this.header.text = header;
            this.body.text = body;
        }
        
        public void SetButton(string text, Action action)
        {
            this.startButton.text = text;
            this.startButton.clicked += action;
            this.startButton.clicked += () => this.AddClass("hide");
            
        }
    }
}
