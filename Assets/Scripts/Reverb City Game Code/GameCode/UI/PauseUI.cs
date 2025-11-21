using System;
using System.Collections;
using DreamersInc.InputSystems;
using DreamersInc.ReverbCity.GameCode.UI;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.UIToolkitHelpers;
using Unity.Entities;
using UnityEngine.InputSystem;
namespace DreamersInc.ReverbCity.UI
{
    public class PauseUI : UIController
    {
        private EntityManager manager;
        private Entity runningEntity;
        private  PlayerControls _playerControls;
        void Start()
        {
            UIManager.RegisterUI(UIType.PauseMenu, document);
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            runningEntity = manager.CreateEntityQuery(typeof(RunningTag)).GetSingletonEntity();
            StartCoroutine(Generate());
            GetPlayerControls();
            _playerControls.PauseMenu.PauseGame.performed += TogglePause;
            _playerControls.PlayerController.PauseGame.performed += TogglePause;
        }
        private void OnDisable()
        {
            _playerControls.PauseMenu.PauseGame.performed -= TogglePause;
            _playerControls.PlayerController.PauseGame.performed -= TogglePause;
        }

        public override IEnumerator Generate()
        {
           
            StartCoroutine(base.Generate());
            Root.AddClass(HideClass);
            yield return null;
        }
        bool isPaused = false;

        void TogglePause(InputAction.CallbackContext obj)
        {
            isPaused = !isPaused;
            if (!isPaused)
            {
                Root.AddClass(HideClass);
                manager.AddComponent<RunningTag>(runningEntity);

            }
            else
            {
                Root.RemoveFromClassList(HideClass);
                manager.RemoveComponent<RunningTag>(runningEntity);

            }
        }
        private void GetPlayerControls()
        {
            EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var inputEntity = manager.CreateEntityQuery(typeof(InputSingleton)).GetSingletonEntity();
            _playerControls = manager.GetComponentData<InputSingleton>(inputEntity).ControllerInput;
        }

    }
}