using System.Threading.Tasks;
using DreamersInc.ServiceLocatorSystem;
using UnityEngine;
namespace DreamersInc.ReverbCity.GameCode
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField]
        private Transform cameraPosition;
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
            var cam = Camera.main;
            cam.transform. position = cameraPosition.position;
            cam.transform.rotation = cameraPosition.rotation;
            ServiceLocator.Global.Get<LevelChanger>().FadeIn();
        }
    }
}