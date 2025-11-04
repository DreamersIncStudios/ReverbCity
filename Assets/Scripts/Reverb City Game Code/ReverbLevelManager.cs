using System.Threading.Tasks;
using DreamersInc.ServiceLocatorSystem;
using UnityEngine;
namespace DreamersInc.ReverbCity
{
    public class ReverbLevelManager : MonoBehaviour, ILevelManager
    {
        
        public async Task Init()
        {

            ServiceLocator.Global.Get<LevelChanger>().FadeIn();
        }
        
        public void Awake()
        {
            ServiceLocator.Global.Register(typeof(ILevelManager), this);
        }

        private void OnDisable()
        {
            ServiceLocator.Global.Unregister(typeof(ILevelManager), this);
        }
    }
}
