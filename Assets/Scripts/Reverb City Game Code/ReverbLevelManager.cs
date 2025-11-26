using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DreamersInc.ReverbCity.GameCode;
using DreamersInc.ReverbCity.UI;
using DreamersInc.SceneManagement;
using DreamersInc.ServiceLocatorSystem;
using DreamersInc.UIToolkitHelpers;
using DreamersInc.WaveSystem.interfaces;
using ImprovedTimers;
using Unity.Entities;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using static DreamersInc.ReverbCity.GameCode.UI.UIExtensionMethods;
using static Bestiary.BestiaryManager;
using Timer = System.Timers.Timer;
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
    IntervalTimer timer;

        [Header("Spawn Settings")]
        [SerializeField] List<Transform> spawnPoints;
        [Header("Wave Settings")]
        public WaveRule TestRule;
        [Tooltip( "Time between waves in minutes" )]
        [Range( 1, 15 )]
        [SerializeField] float timeBetweenWaves;

        private EntityManager manager;
        private Entity runningEntity;
        [CreateProperty] string TimeLeftProperty =>$"Time Until Next Wave: {FormatTime(timer.CurrentTime- timer.nextInterval)}";
        private string FormatTime(float totalSeconds)
        {
            int minutes = Mathf.FloorToInt(totalSeconds / 60f);
            int seconds = Mathf.FloorToInt(totalSeconds % 60f);

            return $"{minutes:00}:{seconds:00}";
        }
        public async Task Init()
        {
            await SpawnPlayer(GameMaster.GetPlayerGuid(), spawnPoints[0].position);
            ServiceLocator.Global.Get<LevelChanger>().FadeIn();
            await Task.Delay(1000);
            var speakers = GameObject.FindObjectsByType<Speaker>(FindObjectsSortMode.None);
             manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            runningEntity = manager.CreateEntityQuery(typeof(RunningTag)).GetSingletonEntity();
        manager.RemoveComponent<RunningTag>(runningEntity);
            foreach (var speaker in speakers)
            {
                await speaker.Init();
            }
            timer = new IntervalTimer(360 * 60, timeBetweenWaves * 60);
            timer.OnInterval += () =>
            {
                TestRule.StartWave(1);
            };
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

        Task CreateUI()
        {
            var hudDoc = UIManager.GetUI(UIType.HUD);
            var popUpPanel = Create<PopUpPanel>();
            var panel = Create<WaveUIPanel>("hide");
            hudDoc.rootVisualElement.Add(popUpPanel);
            hudDoc.rootVisualElement.Add(panel);
            buttonAction += () =>
            {
                var delayTimer = new CountdownTimer(5);
                delayTimer.OnTimerStop += () =>
                {
                    TestRule.StartWave(2);
                    timer.Start();
                };
                delayTimer.Start();
                panel.RemoveFromClassList("hide");
                manager.AddComponent<RunningTag>(runningEntity);
            };
            popUpPanel.SetText(headerText, bodyText);
            popUpPanel.SetButton(buttonText, buttonAction);
            var label = Create<Label>("WaveInfo").AddTo(panel);
            label.dataSource = this;
            label.SetBinding(nameof(Label.text), new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(TimeLeftProperty)),
                bindingMode = BindingMode.ToTarget
            });
                
            return Task.CompletedTask;
        }
    }
}
