
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DreamersInc.ComboSystem;
using ImprovedTimers;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DreamersInc.CharacterControllerSys.VFX
{
    public static class VFXDatabase
    {
        private static readonly Dictionary<SerializableGuid, PoolVFX> vfxDictionaryActive = new();
        private static readonly Dictionary<SerializableGuid, VFXSO> Library = new();
        
        private static void RegisterLibrary(SerializableGuid guid, VFXSO vfx) => Library.Add(guid, vfx);
        private static void RegisterVFX(SerializableGuid guid, GameObject vfx, bool keepAlive)
        {
            var poolVFX = new PoolVFX();
            poolVFX.PoolObject = vfx;
            poolVFX.Pool = new List<GameObject>();
            for (var i = 0; i < 6; i++)
            {
                var go = Object.Instantiate(vfx, pool.transform);
                    go.SetActive(false);
                poolVFX.Pool.Add(go);
            }
            if (!keepAlive)
            {
                poolVFX.Timer = new CountdownTimer(10 * 60);
                //add self destruct function
                poolVFX.Timer.Start();
            }
            vfxDictionaryActive.Add(guid, poolVFX);
        }
        
        private static void DeregisterVFX(SerializableGuid guid)
        {
            if (!vfxDictionaryActive.TryGetValue(guid, out var poolvfx))
                return;
            poolvfx.Timer.Dispose();
            poolvfx.Pool.ForEach(Object.Destroy);
            vfxDictionaryActive.Remove(guid);

        }
        

        static bool VFXLoaded;
        static bool PoolLoaded;

        public  static void Init()
        {
        

            LoadVFX();
       

        }

        private static GameObject pool;
        private static void LoadVFX()
        {
            
            VFXLoaded = true;
            var vfxSOs = Resources.LoadAll<VFXSO>("VFX");
             pool = new GameObject("VFX Pool");
             Object.DontDestroyOnLoad(pool);
            foreach (var vfx in vfxSOs)
            {
                if (vfx.CreatePoolOnLoad)
                {
                   RegisterVFX(vfx.ID,vfx.Prefab, vfx.KeepPoolAlive);
                }
                RegisterLibrary(vfx.ID, vfx);
            }
      
        }

 


        public static async void PlayVFX(SerializableGuid ID, Vector3 Pos, Vector3 Rot, float DelayStart = 0.0f, float lifeTime = 0.0f)
        {
            if (!vfxDictionaryActive.ContainsKey(ID))
            {
                RegisterVFX(ID, Library[ID].Prefab, Library[ID].KeepPoolAlive);
            }
            var poolVFX = vfxDictionaryActive[ID];
            if(!Library[ID].KeepPoolAlive)
                poolVFX.Timer.Reset(10*60);
            bool played = false;
            GameObject vfxToPlay = null;
            foreach (var instance in poolVFX.Pool)
            {
                if (instance.activeSelf)
                    return;
                vfxToPlay = instance;
                played = true;
                break;
            }
            if (!played)
            {
               vfxToPlay = GameObject.Instantiate(poolVFX.PoolObject, pool.transform);
                poolVFX.Pool.Add(vfxToPlay);
            }
            
            Play(vfxToPlay, Pos, Rot, DelayStart, lifeTime);

        }
        static async Task Play(GameObject instance,Vector3 Pos, Vector3 Rot, float DelayStart = 0.0f, float lifeTime = 0.0f)
        {
            instance.transform.SetPositionAndRotation(Pos, Quaternion.Euler(Rot));
            ParticleSystem PS = instance.GetComponent<ParticleSystem>();
            instance.SetActive(true);
            await Task.Delay(TimeSpan.FromMilliseconds(DelayStart));
            PS.Play(true);
            //  TriggerPlay;
            await Task.Delay(TimeSpan.FromSeconds(lifeTime));
            PS.Stop(true);
            instance.SetActive(false);
        }




        public class PoolVFX
        {
            public GameObject PoolObject;
            public List<GameObject> Pool;
            public CountdownTimer Timer;
        }
    }

    internal static class VFXBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            VFXDatabase.Init();
        }
    }
}