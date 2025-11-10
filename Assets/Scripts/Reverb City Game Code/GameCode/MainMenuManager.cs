using System.Collections.Generic;
using System.Threading.Tasks;
using DreamersInc.SceneManagement;
using DreamersInc.ServiceLocatorSystem;
using UnityEngine;
using UnityEngine.Serialization;
namespace DreamersInc.ReverbCity.GameCode
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField]
        private List<string> playerGuids;

        public void Awake()
        {
            ServiceLocator.Global.Register(GetType(), this);
        }

        private void OnDisable()
        {
            ServiceLocator.Global.Unregister(GetType(), this);
        }
        public async Task Init()
        {

            ServiceLocator.Global.Get<LevelChanger>().FadeIn();
        }
        public async void StartGame()
        {
            GameMaster.RegisterPlayerGuid(playerGuids[0]); // todo make menu choice
            await ServiceLocator.Global.Get<SceneLoader>().LoadSceneGroup(1);
        }
    }
}