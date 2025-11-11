using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{
    [CreateAssetMenu(fileName = "Spawn Special", menuName = "GAIA/Spawn Special", order = 1)]
    public class GAIASpawnSO : ScriptableObject, ISpawnSpecial
    {
        [SerializeField] private uint biomeID;
        [SerializeField] private List<SpawnData> spawnData;
        [SerializeField] private List<PackInfo> packsToSpawn;
        public uint BiomeID => biomeID;
        public List<SpawnData> SpawnData => spawnData;
        public List<PackInfo> PacksToSpawn => packsToSpawn;


        public void LoadSpawnData(List<SerializableGuid> locationMapGuids)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = em.CreateEntityQuery(typeof(GaiaSpawnBiome));
            var biomeEntities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in biomeEntities)
            {
                var biome = em.GetComponentData<GaiaSpawnBiome>(entity);
                if (biome.BiomeID != BiomeID) continue;
                foreach (var spawn in SpawnData)
                    biome.SpawnData.Add(spawn);
                foreach (var pack in PacksToSpawn)
                {
                    var temp = pack;
                    foreach (var guid in locationMapGuids)
                        temp.LocationMapGuids.Add(guid);

                    biome.PacksToSpawn.Add(temp);
                }


                em.SetComponentData(entity, biome);
            }

            biomeEntities.Dispose();
        }
    }
}