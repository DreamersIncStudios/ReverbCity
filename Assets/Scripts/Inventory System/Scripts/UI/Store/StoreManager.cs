using System;
using System.Collections;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.UIToolkitHelpers;
using UnityEngine;
using UnityEngine.UIElements;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public class StoreManager : UIManager
    {
 

        private void OnDestroy()
        {
            ServiceLocator.Global.Unregister(GetType(), this);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (!ServiceLocator.Global.TryGet<StoreManager>(out _))
            {
                ServiceLocator.Global.Register(GetType(), this);
            }
            else
                Destroy(gameObject);
            StartCoroutine(Generate());
        }

        public override IEnumerator Generate()
        {
            StartCoroutine(base.Generate());
            Root.AddClass(HideClass);
            var background= Root.CreateChild("Background");
            background.CreateChild("storeHeader").Add(new Label("Store"));
            var container = background.CreateChild("containerG");
            var options = container.CreateChild("optionPanel", "panel");
            var buy = options.CreateChild<Button>();
            buy.text = "Buy";
            var sell = options.CreateChild<Button>();
            sell.text = "Sell";
            var close = options.CreateChild<Button>();
            close.text = "Exit";

            var inventory = new StoreInventory();
            inventory.AddTo(container).AddClass("inventory", "panel");
            var cart = new Cart();
            cart.AddTo(container).AddClass("cartPanel", "panel");
            var footer = background.CreateChild("footer");
            yield return null;
        }

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                ToggleStore();
            }

        }
        bool visible = false;
        void ToggleStore()
        {
            if (visible)
            {
                Root.AddClass(HideClass);
                visible = false;
            }
            else
            {
                Root.RemoveFromClassList(HideClass);
                visible = true;
            }
            ServiceLocator.Global.Get<HUDManager>().toggle();
        }
    }
}