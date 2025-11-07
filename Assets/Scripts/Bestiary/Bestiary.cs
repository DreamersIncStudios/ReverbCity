using System.Collections.Generic;
using Stats;
using Stats.Entities;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Bestiary
{
    public static class Bestiary
    {
        static readonly List<PlayerInfo> PlayerInfos = new List<PlayerInfo>();
        static readonly List<CreatureInfo> CreatureInfos = new List<CreatureInfo>();
        static readonly List<StructureInfo> StructureInfos = new List<StructureInfo>();
        
        public static void AddPlayerInfo(PlayerInfo info) => PlayerInfos.Add(info);
        public static void AddCreatureInfo(CreatureInfo info) => CreatureInfos.Add(info);
        public static void AddStructureInfo(StructureInfo info) => StructureInfos.Add(info);
        
        public static PlayerInfo GetPlayerInfo(SerializableGuid guid) => PlayerInfos.Find(x => x.Guid == guid);
        public static CreatureInfo GetCreatureInfo(SerializableGuid guid) => CreatureInfos.Find(x => x.Guid == guid);
        public static StructureInfo GetStructureInfo(SerializableGuid guid) => StructureInfos.Find(x => x.Guid == guid);

        static readonly Dictionary<int, WaveInfo> NPCSpawnForWaves;
        static readonly List<Entity> StructureEntities;
        static Entity playerEntity;

        public static void RegisterNPCEnemy(int waveNumber, Entity entity)
        {
            if (NPCSpawnForWaves.TryGetValue(waveNumber, out var waveInfo))
            {
                waveInfo.Entity.Add(entity);
            }
            else
            {
                NPCSpawnForWaves.Add(waveNumber, new WaveInfo()
                {
                    Entity = new List<Entity>(){entity}
                });
            }

        }

        public static void RegisterStructure(Entity entity) => StructureEntities.Add(entity);
        public static void RegisterPlayer(Entity entity) => playerEntity = entity;

        public static void DeregisterNPCEnemy(int waveNumber, Entity entity)
        {
            if (NPCSpawnForWaves.TryGetValue(waveNumber, out var waveInfo))
            {
                waveInfo.Entity.Remove(entity);
            }
        }

        public class WaveInfo
        {
            public List<Entity> Entity;
        }

        public static bool SpawnNPC()
        {
            return false;
        }
        public static bool SpawnPlayer()
        {
            return false;
        }
        
        public static bool SpawnStructure()
        {
            return false;
        }

        #region Builder

        public class CharacterBuilder
        {
            
            private readonly Entity entity;
            private EntityManager manager;
            
            public CharacterBuilder(string entityName)
            {
                manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var baseEntityArch = manager.CreateArchetype(
                    typeof(LocalTransform),
                    typeof(LocalToWorld)
                );
                entity = manager.CreateEntity(baseEntityArch);
                manager.SetName(entity, entityName != string.Empty ? entityName : "NPC Data");
                manager.SetComponentData(entity, new LocalTransform()
                {
                    Scale = 1
                });
            }
            
            private GameObject model;
            private string tag;
            
            public CharacterBuilder WithModel(GameObject go, Vector3 position, quaternion rot, string tagging,
                out GameObject spawned)
            {
                spawned = model = Object.Instantiate(go);
                model.transform.position = position;
                model.transform.rotation = rot;

                manager.SetComponentData(entity, new LocalTransform()
                {
                    Position = position,
                    Rotation = model.transform.rotation,
                    Scale = 1
                });

                tag = tagging;
                tag = go.tag = tagging;
                return this;
            }
            
            
            public Entity Build()
            {
                return entity;
            }
        }

        #endregion
    }

    public interface ICharacterInfo
    {
        SerializableGuid Guid { get; }
        public uint Level { get; }

    }
}