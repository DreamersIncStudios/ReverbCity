using System.Collections;
using System.Collections.Generic;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.UIToolkitHelpers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static DreamersInc.MoonShot.GameCode.UI.UIExtensionMethods;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public class PauseView : UIManager
    {
        [SerializeField] private Texture lb, rb, lt, rt;

        private VisualElement buttonHolder;


        private int capacity = 50;

        private VisualElement contentArea;
        private int curHeaderIndex;
        private List<Button> headerButtons;
        private VisualElement rootElement;
        StatsViewModel statsViewModel;
        private List<VisualElement> tabs;

        private UITextureControl textureControl;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            PlayerControls = ServiceLocator.Global.Get<ControllerLocator>().controller;
            ServiceLocator.Global.Register(GetType(), this);
            PlayerControls.PlayerController.PauseGame.performed += TogglePause;
            PlayerControls.PauseMenu.SwapTabs.performed += SwitchTab;
            PlayerControls.PauseMenu.PauseGame.performed += TogglePause;
            PlayerControls.PauseMenu.Disable();
        }

        public override IEnumerator Generate()
        {
            StartCoroutine(base.Generate());
            rootElement = Root.CreateChild("Background", "hide");
            var header = rootElement.CreateChild("header-block");
            contentArea = rootElement.CreateChild("content-block");
            CreateTabView(header);
            // CreateHeader(header, content);

            yield return null;
        }

        private void CreateTabView(VisualElement header)
        {
            headerButtons = new List<Button>();
            tabs = new List<VisualElement>();
            header.CreateChild<Image>("shoulder").image = lb;
            buttonHolder = header.CreateChild();
            buttonHolder.style.flexDirection = FlexDirection.Row;
            header.CreateChild<Image>("shoulder").image = rb;
        }

        public void AddTab(string tabName, VisualElement Tab)
        {
            var button = CreateButtonWithTab(tabName, contentArea, Tab);
            buttonHolder.Add(button.Item1);
            headerButtons.Add(button.Item1);
            tabs.Add(button.Item2);
            Tab.AddTo(contentArea);
        }


        private (Button, VisualElement) CreateButtonWithTab(string buttonText, VisualElement content, VisualElement tab)
        {
            var button = Create<Button>("header-Button");
            button.text = buttonText;
            button.clicked += () =>
            {
                button.AddToClassList("header-Button-Active");
                headerButtons[curHeaderIndex].RemoveFromClassList("header-Button-Active");
                curHeaderIndex = headerButtons.IndexOf(button);
                SwitchTab(content.Children(), tab);
            };
            return (button, tab);
        }

        private void SwitchTab(IEnumerable<VisualElement> allTabs, VisualElement activeTab)
        {
            foreach (var tab in allTabs)
            {
                if (tab == activeTab)
                    tab.RemoveFromClassList(HideClass);
                else
                    tab.AddToClassList(HideClass);
            }
        }

        private void SwitchTab(InputAction.CallbackContext obj)
        {
            var dir = obj.ReadValue<float>() > 0 ? 1 : -1;
            curHeaderIndex += dir;
            if (curHeaderIndex < 0) curHeaderIndex = headerButtons.Count - 1;
            if (curHeaderIndex > headerButtons.Count - 1) curHeaderIndex = 0;
            headerButtons[curHeaderIndex].AddToClassList("header-Button-Active");
            headerButtons[curHeaderIndex].RemoveFromClassList("header-Button-Active");
            SwitchTab(contentArea.Children(), tabs[curHeaderIndex]);
        }

        protected override void TogglePause(InputAction.CallbackContext obj)
        {
            base.TogglePause(obj);
            textureControl = ServiceLocator.Global.Get<UITextureControl>();

            if (paused)
            {
                rootElement.RemoveFromClassList("hide");
                textureControl.gameObject.SetActive(true);
                PlayerControls.PlayerController.Disable();
                PlayerControls.PauseMenu.Enable();
            }
            else
            {
                rootElement.AddToClassList("hide");
                textureControl.gameObject.SetActive(false);
                PlayerControls.PlayerController.Enable();
                PlayerControls.PauseMenu.Disable();
            }
        }
    }
}