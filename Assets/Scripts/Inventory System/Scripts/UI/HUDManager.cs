using System.Collections;
using Dreamers.InventorySystem;
using DreamersInc.MoonShot.GameCode.UIElements.CustomVisualElements;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.UIToolkitHelpers;
using Newtonsoft.Json.Bson;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static DreamersInc.MoonShot.GameCode.UI.UIExtensionMethods;

namespace DreamersInc.MoonShot.GameCode.UI
{
    public class HUDManager : UIManager
    {
        private VisualElement rootElement;
       

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //todo create a completed load check
            if (!ServiceLocator.Global.TryGet<HUDManager>(out _))
            {
                ServiceLocator.Global.Register(GetType(), this);
            }
            
            PlayerControls = ServiceLocator.Global.Get<ControllerLocator>().controller;
            PlayerControls.PlayerController.PauseGame.performed += TogglePause;
            PlayerControls.PauseMenu.PauseGame.performed += TogglePause;
        }

        public void InitializeHUD()
        {
            StartCoroutine(Generate());
            CreateBinding();
        }

        private void OnDestroy()
        {
            ServiceLocator.Global.Unregister(GetType(), this);
        }
        
        // Update is called once per frame
        private WeaponInterface weapons;
        public override IEnumerator Generate()
        {
            StartCoroutine(base.Generate());
            rootElement = Create("Background");
            rootElement.AddTo(Root);
            weapons = rootElement.CreateChild<WeaponInterface>();
 
            yield return null;
        }

        void CreateBinding()
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerEntity = em.CreateEntityQuery(typeof(Player_Control)).GetSingletonEntity();
            var inventory = em.GetComponentData<CharacterInventory>(playerEntity);
            weapons.Bind(new WeaponViewModel(ref inventory), PlayerControls);
        }

        protected override void TogglePause(InputAction.CallbackContext obj)
        {
            base.TogglePause(obj);
            if(paused)
                rootElement.AddToClassList("hide");
            else
                rootElement.RemoveFromClassList("hide");
        }

        public void toggle()
        {
            Debug.Log("Remove in final build");
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            paused = !paused;
         
            if (paused)
            {
                em.RemoveComponent<RunningTag>(runningEntitySingleton);
            }
            else
            {
                em.AddComponent<RunningTag>(runningEntitySingleton);
            }
            if(paused)
                rootElement.AddToClassList("hide");
            else
                rootElement.RemoveFromClassList("hide");
        }
    }
}