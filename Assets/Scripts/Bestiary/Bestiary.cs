using System;
using System.Collections.Generic;
using System.Linq;
using DreamersInc.DamageSystem;
using Stats;
using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bestiary
{
    public static partial class BestiaryManager
    {
        static readonly List<PlayerInfo> PlayerInfos = new List<PlayerInfo>();
        static readonly List<CreatureInfo> CreatureInfos = new List<CreatureInfo>();
        static readonly List<StructureInfo> StructureInfos = new List<StructureInfo>();

        static readonly Dictionary<uint, WaveInfo> NPCSpawnForWaves = new Dictionary<uint, WaveInfo>();
        static readonly List<Entity> StructureEntities = new List<Entity>();

        public static void AddPlayerInfo(PlayerInfo info) => PlayerInfos.Add(info);
        public static void AddCreatureInfo(CreatureInfo info) => CreatureInfos.Add(info);
        public static void AddStructureInfo(StructureInfo info) => StructureInfos.Add(info);

        public static PlayerInfo GetPlayerInfo(SerializableGuid guid)
            => PlayerInfos.Find(x => x.Guid == guid);

        public static CreatureInfo GetCreatureInfo(SerializableGuid guid) => CreatureInfos.Find(x => x.Guid == guid);
        public static StructureInfo GetStructureInfo(SerializableGuid guid) => StructureInfos.Find(x => x.Guid == guid);

        static Entity playerEntity;


        static public bool IsLoaded { get; private set; }

        public static void LoadDatabase(bool ForceLoad = false)
        {

            if (IsLoaded && !ForceLoad)
                return;
            CreatureInfo[] creatureSO = Resources.LoadAll<CreatureInfo>("Bestiary/Creatures");
            foreach (var item in creatureSO)
            {
                if (!CreatureInfos.Contains(item))
                    AddCreatureInfo(item);
            }

            PlayerInfo[] playerSO = Resources.LoadAll<PlayerInfo>("Bestiary/Player Characters");
            foreach (var item in playerSO)
            {
                if (!PlayerInfos.Contains(item))
                    AddPlayerInfo(item);
            }

            StructureInfo[] testingSO = Resources.LoadAll<StructureInfo>(@"Bestiary/Testing");
            foreach (var item in testingSO)
            {
                if (!StructureInfos.Contains(item))
                    AddStructureInfo(item);
            }

            IsLoaded = true;
        }

        public static CreatureInfo GetCreature(SerializableGuid id)
        {
            LoadDatabase();
            return Object.Instantiate(CreatureInfos.FirstOrDefault(creature => creature.Guid == id));
        }
        public static CreatureInfo GetCreature()
        {
            LoadDatabase();
            return Object.Instantiate(CreatureInfos[0]);
        }

        public static bool TryGetCreature(SerializableGuid id, out CreatureInfo info)
        {
            info = GetCreature(id);
            return info != null;
        }

        public static PlayerInfo GetPlayer(SerializableGuid id)
        {
            LoadDatabase();
            foreach (var player in PlayerInfos)
            {
                if (player.Guid == id)
                    return Object.Instantiate(player);
            }
            return null;
        }


        public static void RegisterNPCEnemy(uint waveNumber, Entity entity)
        {
            if (NPCSpawnForWaves.TryGetValue(waveNumber, out var waveInfo))
            {
                waveInfo.Entities.Add(entity);
            }
            else
            {
                NPCSpawnForWaves.Add(waveNumber, new WaveInfo()
                {
                    Entities = new List<Entity>()
                    {
                        entity
                    }
                });
            }

        }

        public static void KillWaveNpcs(uint waveNumber)
        {
            try
            {
                
                NPCSpawnForWaves.TryGetValue(waveNumber, out var waveInfo);
                var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                foreach (var entity in waveInfo.Entities)
                {
                    manager.AddComponent<DeathTag>(entity);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void RegisterStructure(Entity entity) => StructureEntities.Add(entity);
        public static void RegisterPlayer(Entity entity) => playerEntity = entity;

        public static void DeregisterNPCEnemy(uint waveNumber, Entity entity)
        {
            if (NPCSpawnForWaves.TryGetValue(waveNumber, out var waveInfo))
            {
                waveInfo.Entities.Remove(entity);
            }
        }

        public class WaveInfo
        {
            public List<Entity> Entities;
        }


        public static bool SpawnStructure()
        {
            return false;
        }
        

    }

    public interface ICharacterInfo
    {
        SerializableGuid Guid { get; }
        public PlayerCharacterClass Stats { get; }

    }
    
    internal static class BestiaryBootstrapper
        {
           [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
           internal static void Initialize() {
               BestiaryManager.LoadDatabase();
           }
        }
    
}