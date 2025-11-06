using System;
using System.Threading.Tasks;
using DreamersInc.ReverbCity.UI;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.UIToolkitHelpers;
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
        public async Task Init()
        {
            ServiceLocator.Global.Get<LevelChanger>().FadeIn();
            await Task.Delay(2000);
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
            hudDoc.rootVisualElement.Add(popUpPanel);
            
            popUpPanel.SetText(headerText, bodyText);
            popUpPanel.SetButton(buttonText, buttonAction);
        }
    }
}
