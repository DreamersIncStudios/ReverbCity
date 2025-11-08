using System;
using System.Threading.Tasks;
using DreamersInc.ReverbCity.GameCode;
using DreamersInc.ReverbCity.UI;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.UIToolkitHelpers;
using DreamersInc.WaveSystem.interfaces;
using UnityEngine;
using static DreamersInc.ReverbCity.GameCode.UI.UIExtensionMethods;
namespace DreamersInc.ReverbCity
{
    public class ReverbLevelManager : MonoBehaviour, ILevelManager
    {
        [Header("UI Settings")]
        [SerializeField]
        private string headerText;
        [SerializeField]
        [TextArea(3, 10)]
        private string bodyText;
        [SerializeField]
        private string buttonText;
        [SerializeField]
        private Action buttonAction;

        public WaveRule TestRule;
        public async Task Init()
        {
            ServiceLocator.Global.Get<LevelChanger>().FadeIn();
            await Task.Delay(2000);
            var speakers = GameObject.FindObjectsByType<Speaker>(FindObjectsSortMode.None);
            foreach (var speaker in speakers)
            {
                await speaker.Init();
            }
            await CreateUI();
        }
        
        public void Awake()
        {
            ServiceLocator.Global.Register(typeof(ILevelManager), this);
        }

        private void OnDisable()
        {
            ServiceLocator.Global.Unregister(typeof(ILevelManager), this);
        }

        async Task CreateUI()
        {
            var hudDoc = UIManager.GetUI(UIType.HUD);
            var popUpPanel = Create<PopUpPanel>();
            var panel = Create<WaveUIPanel>("hide");
            hudDoc.rootVisualElement.Add(popUpPanel);
            hudDoc.rootVisualElement.Add(panel);
            buttonAction += () =>
            {
                TestRule.StartWave(2);
                panel.RemoveFromClassList("hide");
            };
            popUpPanel.SetText(headerText, bodyText);
            popUpPanel.SetButton(buttonText, buttonAction);
        }
    }
}
