using UnityEngine;


namespace DreamersInc.Bootstrapping
{
    public static class EntryPoint
    {
  
        const string QualityManagerPrefab = "QualityMangerUI";
        const string ControllerLocalPrefab = "Controller Location";
        const string ControllerLocatorPrefab = "Control assign";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            InitController();
            InitQualityManager();
        }

        private static void InitQualityManager()
        {
        
            
        }

        private static void InitController()
        {
         
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void CreateOrLoadQualitySettings()
        {
    
        }

    }
}