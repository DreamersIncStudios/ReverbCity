using System.Threading.Tasks;
using DreamersInc.SceneManagement;
using DreamersInc.ServiceLocatorSystem;
using UnityEngine;
namespace DreamersInc.ReverbCity.GameCode
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

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
            await ServiceLocator.Global.Get<SceneLoader>().LoadSceneGroup(1);
        }
    }
}