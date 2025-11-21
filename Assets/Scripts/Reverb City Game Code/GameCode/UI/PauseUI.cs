using System.Collections;
using DreamersInc.InputSystems;
using DreamersInc.ReverbCity.GameCode.UI;
using DreamersInc.UIToolkitHelpers;
using ImprovedTimers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static DreamersInc.ReverbCity.GameCode.UI.UIExtensionMethods;

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
            _playerControls.PauseMenu.PauseGame.performed += WaveManager.TogglePause;
            _playerControls.PlayerController.PauseGame.performed += WaveManager.TogglePause;
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
            Root.AddClass("PauseMenu");
            var panel = Create("PausePanel").AddTo(Root);
           var label = Create<Label>("PauseLabel").AddTo(panel);
           label.text = "Paused";
           var options = Create<Button>("PauseButton").AddTo(panel);
           options.text = "Options";
           var  restart = Create<Button>("PauseButton").AddTo(panel);
           restart.text = "Restart Run";
           var  save = Create<Button>("PauseButton").AddTo(panel);
           save.text = "Save Run";
           var exit  = Create<Button>("PauseButton").AddTo(panel);
           exit.clicked += () =>
           {
               Debug.Log("Exiting Game");
               Application.Quit();
           };
           exit.text = "Exit Game";
           
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