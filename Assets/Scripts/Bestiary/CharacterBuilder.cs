using System;
using System.Collections.Generic;
using AISenses;
using AISenses.VisionSystems;
using AISenses.VisionSystems.Combat;
using Components.MovementSystem;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Base;
using DreamersInc;
using DreamersInc.CombatSystem;
using DreamersInc.ComboSystem;
using DreamersInc.InfluenceMapSystem;
using DreamersInc.ReverbCity.GameCode.Entities;
using DreamersInc.ServiceLocatorSystem;
using DreamersIncStudio.FactionSystem;
using DreamersIncStudio.GAIACollective;
using DreamersStudio.CameraControlSystem;
using Global.Component;
using IAUS.ECS;
using IAUS.ECS.Component;
using IAUS.ECS.Component.Attacking;
using MotionSystem.Components;
using MotionSystem.Systems;
using ProjectDawn.Navigation;
using Stats;
using Stats.Entities;
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
    public static partial class BestiaryManager
    {
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

            public CharacterBuilder WithModel(GameObject go, Vector3 position, quaternion rot, string tagging)
            {
                model = Object.Instantiate(go);
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
            BaseCharacterComponent character;
            private ComboSO combo;
            private FactionNames factionID;
            private uint classLevel;
            public CharacterBuilder WithStats(ICharacterData stats, SerializableGuid spawnID, uint playerLevel, string name, uint exp = 0,
                bool InvincibleMode = false, bool limitHp = false,
                uint limit = 0, bool clone = false)
            {
                if (entity == Entity.Null) return this;
                if (!model) return this;
                BaseCharacterComponent data = new()
                {
                    GORepresentative = model,
                    InvincibleMode = InvincibleMode,
                    HealthLimit = limit,
                    HealthCantDropBelow = limitHp,
                    Clone = clone
                };
                data.SetupDataEntity(stats, name, exp, spawnID);
                manager.AddComponentObject(entity, data);

                if (!model.TryGetComponent<Damageable>(out var damageable))
                {
                    damageable = model.AddComponent<Damageable>();
                }
                damageable.SetData(entity, data); 
                
                this.character = data;

                var baseEntityArch = manager.CreateArchetype(
                    typeof(LocalTransform),
                    typeof(LocalToWorld),
                      typeof(MeleeAttackPosition)
                );
                var baseDataEntity = manager.CreateEntity(baseEntityArch);
                manager.SetName(baseDataEntity, "Attack Location Entity");
                manager.SetComponentData(baseDataEntity, new LocalTransform()
                {
                    Scale = 1
                });
                manager.AddComponentData(baseDataEntity, new Parent()
                {
                    Value = entity
                });

                manager.AddBuffer<ReserveLocationTag>(baseDataEntity);
                var meleeAttackPositions = manager.GetBuffer<MeleeAttackPosition>(baseDataEntity);
                meleeAttackPositions.Length = 4;
                for (var index = 0; index < meleeAttackPositions.Length; index++)
                {
                    var attackPosition = meleeAttackPositions[index];
                    attackPosition.State = OccupiedState.Vacant;
                    meleeAttackPositions[index] = attackPosition;
                }

                return this;
            }
            public CharacterBuilder WithMovement(MovementData move, CreatureType creatureType, bool ai = false)
            {
                if (entity == Entity.Null) return this;
                if (!model) return this;
                switch (creatureType)
                {
                    case CreatureType.biped:
                        var controllerData = new CharControllerE();
                        controllerData.Setup(move, model.GetComponent<CapsuleCollider>(), ai);
                        manager.AddComponentData(entity, controllerData);
                        break;
                    case CreatureType.quadruped:
                        var beastData = new BeastControllerComponent();
                        beastData.Setup(move, model.GetComponent<CapsuleCollider>(), ai);
                        manager.AddComponentData(entity, beastData);
                        break;
                    case CreatureType.mecha:
                        break;
                    case CreatureType.spirit:
                        break;
                    case CreatureType.stationary:
                        break;
                    case CreatureType.flying:
                        break;
                }

                if (move.CombatCapable)
                    manager.AddComponent<CombatCapable>(entity);
                return this;
            }
            public CharacterBuilder WithAnimation()
            {
                if (entity == Entity.Null) return this;
                if (!model) return this;
                var anim = model.GetComponent<Animator>();
                manager.AddComponentObject(entity, anim);
                manager.AddComponentObject(entity, model.transform);
                if (!model.TryGetComponent<AnimationSpeed>(out var add))
                {
                    add = model.AddComponent<AnimationSpeed>();
                }
                
                var link = new AnimationSpeedLink()
                {
                    Link = add
                };
               manager.AddComponentObject(entity, link);
               
                return this;
            }
      

            public CharacterBuilder WithPlayerControl()
            {
                if (entity == Entity.Null) return this;
                if (!model) return this;
                var tag = model.AddComponent<PlayerTag>();
                ServiceLocator.Global.Register(tag.GetType(), tag);
                manager.AddComponent<Player_Control>(entity);
                manager.AddComponent<AttackTarget>(entity);

                var command = new Command
                {
                    BareHands = true, // equip system need to adjust this value
                    InputTimeReset = 500.0f,
                    InputTimer = 500.0f
                };
                manager.AddComponentData(entity, command);
                var trigger = model.GetComponent<WeaponEventTrigger>();
                trigger.OnAnimationEvent += (sender, args) =>
                {
                    if (args.AnimID == 0) return;
                    command.InputQueue.Enqueue(new AnimationTrigger()
                    {
                        AttackType = AttackType.SpecialAttack,
                        triggerAnimIndex = args.AnimID,
                        TransitionDuration = args.Duration,
                        TransitionOffset = args.TransitionOffset,
                        EndOfCurrentAnim = args.EndofCurrentAnim
                    });
                };
                manager.AddComponentObject(entity, trigger);

                CameraControl.Instance.Follow.LookAt = model.GetComponentInChildren<LookHereTarget>().transform;
                CameraControl.Instance.Follow.Follow = model.transform;
                CameraControl.Instance.Target.Follow = model.transform;

                return this;
            }

            public CharacterBuilder WithInventorySystem(InventorySave infoInventory, EquipmentSave infoEquipment)
            {
                if (entity == Entity.Null || !model) return this;

                CharacterInventory inventory = new();
                inventory.AddGold(200);
                manager.AddComponentData(entity, inventory);
                manager.GetComponentData<CharacterInventory>(entity).Setup(entity, infoInventory, infoEquipment, ref character);

                return this;
            }
            public CharacterBuilder WithCombat(ComboSO infoCombo)
            {
                this.combo = infoCombo;
                if (entity == Entity.Null) return this;
                if (model == null) return this;
                manager.AddComponent<StorePrimaryWeapon>(entity);

                var comboInfo = Object.Instantiate(infoCombo);
                manager.AddComponentObject(entity, new PlayerComboComponent
                {
                    Combo = comboInfo
                });
                return this;
            }
            public CharacterBuilder WithVFX()
            {
                model.GetComponent<VFXControl>().Init(combo);
                return this;
            }
            public CharacterBuilder WithFactionInfluence(FactionNames factionID, int influenceValue, uint classLevel,
                float3 centerOffset = default,
                bool isPlayer = false)
            {
                if (entity == Entity.Null) return this;
                if (!model) return this;


                this.factionID = factionID;
                this.classLevel = classLevel;
            
                    manager.AddComponentData(entity,
                        new InfluenceComponent(this.factionID, influenceValue, 50));
                

                manager.AddComponentData(entity, new AITarget()
                {
                    FactionID = (FactionNames)factionID,
                    NumOfEntityTargetingMe = 3,
                    CanBeTargetByPlayer = isPlayer,
                    Type = TargetType.Character,
                    level = classLevel,
                    CenterOffset = centerOffset
                });

       

                return this;
            }

            private Entity aiEntity;
            public CharacterBuilder WithAI(Rank getRank, FactionNames FactionID, List<AIStates> aiStatesToAdd, bool capableOfMelee = false,
                bool capableOfMagic = false, bool capableOfRange = false, Role role = default)
            {
                if (!model || entity == Entity.Null) return this;

                var baseEntityArch = manager.CreateArchetype(
                    typeof(LocalTransform),
                    typeof(LocalToWorld)
                );
                aiEntity = manager.CreateEntity(baseEntityArch);
                manager.SetName(aiEntity, "AI Entity");
                manager.SetComponentData(aiEntity, new LocalTransform()
                {
                    Scale = 1
                });
                manager.AddComponentData(aiEntity, new Parent()
                {
                    Value = entity
                });

                manager.AddComponentData(aiEntity, new AIStat());
                manager.AddComponentData(entity, new AIStat());

                manager.AddComponentData(aiEntity, new IAUSBrain()
                {
                    rank = getRank,
                    FactionID = FactionID,
                    Difficulty = Difficulty.Normal,
                    Role = role
                });
                manager.AddComponentData(aiEntity, new VisionIAUSLink(visionEntity));
                model.layer = LayerMask.NameToLayer("NPC");
                bool attackStateAdd = false;
                var statesToCheck = manager.AddBuffer<StateData>(aiEntity);
                foreach (var state in aiStatesToAdd)
                {
                    statesToCheck.Add(new StateData(state));
                }

                if (aiStatesToAdd.Contains(AIStates.Attack)
                    || aiStatesToAdd.Contains(AIStates.Terrorize)
                    || aiStatesToAdd.Contains(AIStates.AttackGlobalTarget))
                {
                    var command = new Command
                    {
                        BareHands = true // equip system need to adjust this value
                    };
                    manager.AddComponentData(aiEntity, command);
                    manager.AddComponent<AttackTarget>(aiEntity);
                    
                    manager.AddComponent<CheckAttackStatus>(aiEntity);
                    manager.AddComponentData(aiEntity,
                        new AttackCapable(capableOfMelee, capableOfMagic, capableOfRange));
                }

                manager.AddComponent<SetupBrainTag>(aiEntity);

                return this;
            }
            
            Entity visionEntity;
            public CharacterBuilder WithCharacterDetection(FactionNames FactionID)
            {
                if (entity == Entity.Null) return this;
                if (!model) return this;
                var baseEntityArch = manager.CreateArchetype(
                    typeof(LocalTransform),
                    typeof(LocalToWorld)
                );
                visionEntity = manager.CreateEntity(baseEntityArch);
                manager.SetName(visionEntity, "Vision Entity");
                manager.SetComponentData(visionEntity, new LocalTransform()
                {
                    Scale = 1
                });
                manager.AddComponentData(visionEntity, new Parent()
                {
                    Value = entity
                });

                var vision = new Vision();
                vision.InitializeSense(character, (int)FactionID, canSeeLayerMask);
                //Todo Move to entity by self
                manager.AddBuffer<Enemies>(visionEntity);
                manager.AddBuffer<Allies>(visionEntity);
                manager.AddBuffer<AISenses.Resources>(visionEntity);
                manager.AddBuffer<PlacesOfInterest>(visionEntity);
                manager.AddComponentData(visionEntity, vision);

                return this;
            }
            public CharacterBuilder WithAIControl()
            {
                if (entity == Entity.Null || !model) return this;
                var maxSpeed = 0.0f;
                var collider = model.GetComponent<CapsuleCollider>();
                manager.AddComponentData(entity, new AgentShape()
                {
                    Radius = collider.radius,
                    Height = collider.height,
                    Type = ShapeType.Cylinder
                });
           
                        manager.AddComponentData(entity, Agent.Default);
                        manager.AddComponentData(entity, AgentBody.Default);


    
                manager.AddComponentData(entity, AgentLocomotion.Default);
             
         

                manager.AddComponentData(entity, NavMeshPath.Default);
                manager.AddBuffer<NavMeshNode>(entity);

                manager.AddComponentData(entity, AgentCollider.Default);
                manager.AddComponentData(entity, AgentSonarAvoid.Default);
                manager.AddComponentData(entity, AgentSeparation.Default);
                manager.AddComponentData(entity, AgentSmartStop.Default);

                manager.AddComponentData(aiEntity, new Movement()
                {
                    MaxMovementSpeed = 4
                });

                return this;
            }


            public CharacterBuilder WithParent(Entity parent )
            {
                if (parent == Entity.Null)
                    return this;
                manager.AddComponentData(entity, new Parent()
                {
                    Value = parent
                });
                return this;
            }
            public CharacterBuilder WithStructure(StructureType getType)
            {
                switch (getType)
                {
                    case StructureType.Speaker:
                        manager.AddComponent<Speaker>(entity);
                        break;
                    case StructureType.Antenna:
                        manager.AddComponent<Antenna>(entity);
                        break;
                    case StructureType.BatteryCell:
                        manager.AddComponent<BatteryCell>(entity);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(getType), getType, null);
                }
                return this;
            }
            
            
            
            
            
            
            
            
            public Entity Build()
            {
                return entity;
            }
        }
    }
    public enum StructureType{
        Speaker,
        Antenna, 
        BatteryCell,
    }
}
