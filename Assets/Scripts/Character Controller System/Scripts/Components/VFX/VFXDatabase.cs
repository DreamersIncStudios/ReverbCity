using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DreamersInc.CharacterControllerSys.VFX
{
    public static class VFXDatabase
    {
        public static TextAsset VFXList;
        static List<VFXInfo> vfxInfos;
        static bool VFXLoaded;
        static bool PoolLoaded;

        private static void Init()
        {
        

            loadVFX();
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityArchetype baseEntityArch = manager.CreateArchetype(
                typeof(LocalTransform),
                typeof(LocalToWorld)
            );

        }

        private static GameObject pool;
        public static void loadVFX()
        {
            VFXLoaded = true;
            VFXList = Resources.Load<TextAsset>("VFX/Combo Damage");
            var lines = VFXList.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            vfxInfos = new List<VFXInfo>();
            pool = new GameObject("VFX Pool");
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');

                VFXInfo temp = new()
                {
                    ID = int.Parse(parts[0]),
                    PoolObject = Resources.Load<GameObject>(parts[1])
                };
                temp.CreatePool(pool);
                vfxInfos.Add(temp);
            }
        }

        static void DestoryVFXPool()
        {
            foreach (Transform item in pool.transform)
            {
                Object.Destroy(item.gameObject);
            }
        }

        public static void PlayVFX(int ID, Vector3 Pos, Vector3 Rot, float DelayStart = 0.0f, float lifeTime = 0.0f)
        {
            if (!VFXLoaded)
            {
                DestoryVFXPool();
                loadVFX();
            }

            GetVFX(ID).Play(Pos, Rot, DelayStart, lifeTime);
        }

        public static void PlayVFX(int ID, Vector3 Pos, float lifeTime = 0.0f)
        {
            if (!VFXLoaded)
            {
                DestoryVFXPool();
                loadVFX();
            }

            GetVFX(ID).Play(Pos, lifeTime);
        }

        public static VFXInfo GetVFX(int id)
        {
            VFXInfo temp = new();
            foreach (var item in vfxInfos)
            {
                if (item.ID == id)
                {
                    temp = item;
                    return temp;
                }
            }

            return null;

        }
    }
}