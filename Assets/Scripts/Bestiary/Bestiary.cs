using System;
using System.Collections.Generic;
using System.Linq;
using Global.Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using BoxCollider = UnityEngine.BoxCollider;
using CapsuleCollider = UnityEngine.CapsuleCollider;
using MeshCollider = UnityEngine.MeshCollider;
using Object = UnityEngine.Object;
using SphereCollider = UnityEngine.SphereCollider;

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
            return Object.Instantiate( CreatureInfos.FirstOrDefault(creature => creature.Guid == id));
        }

        public static bool TryGetCreature(SerializableGuid id, out CreatureInfo info)
        {
            info = GetCreature(id);
            return  info != null;
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
    private uint canSeeLayerMask;
            
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

            public CharacterBuilder WithExistingModel(GameObject go, Vector3 position, quaternion rot, string tagging)
            {
                model = go;
                manager.SetComponentData(entity, new LocalTransform()
                {
                    Position = go.transform.position,
                    Rotation = go.transform.rotation,
                    Scale = 1
                });
                return this;
            }
            
                public CharacterBuilder WithEntityPhysics(PhysicsInfo physicsInfo, bool isPlayer = false)
    {
        if (entity == Entity.Null)
            return this;

        if (!model)
            return this;
        if (!model.TryGetComponent<UnityEngine.Collider>(out var col))
            return this;
        var spCollider = new BlobAssetReference<Unity.Physics.Collider>();
        canSeeLayerMask = physicsInfo.CollidesWith.Value;
        switch (col)
        {
            case CapsuleCollider capsule:
                spCollider = Unity.Physics.CapsuleCollider.Create(new CapsuleGeometry()
                    {
                        Radius = capsule.radius,
                        Vertex0 = new float3(0, 0, 0),
                        Vertex1 = capsule.height - 2f * capsule.radius
                    },
                    new CollisionFilter()
                    {
                        BelongsTo = physicsInfo.BelongsTo.Value,
                        CollidesWith = physicsInfo.CollidesWith.Value,
                        GroupIndex = 0
                    });
                break;
            case BoxCollider box:
                spCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry()
                {
                    Center = box.center,
                    Size = box.size,
                    Orientation = quaternion.identity,
                }, new CollisionFilter()
                {
                    BelongsTo = physicsInfo.BelongsTo.Value,
                    CollidesWith = physicsInfo.CollidesWith.Value,
                });
                break;
            case SphereCollider:
                break;
            case MeshCollider:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        manager.AddSharedComponent(entity, new PhysicsWorldIndex());
        manager.AddComponentData(entity, new PhysicsCollider()
        {
            Value = spCollider
        });
        manager.AddComponentData(entity, new PhysicsInfo
        {
            BelongsTo = physicsInfo.BelongsTo,
            CollidesWith = physicsInfo.CollidesWith
        });
        if (model.TryGetComponent<Rigidbody>(out var rb))
        {
            if (!isPlayer)
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            manager.AddComponentObject(entity, rb);
        }

    
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