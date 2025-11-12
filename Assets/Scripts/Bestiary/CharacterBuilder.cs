using System;
using AISenses.VisionSystems.Combat;
using DreamersInc;
using DreamersInc.CombatSystem;
using DreamersInc.ComboSystem;
using DreamersInc.ServiceLocatorSystem;
using DreamersStudio.CameraControlSystem;
using Global.Component;
using MotionSystem.Components;
using MotionSystem.Systems;
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
                    typeof(LocalToWorld)
                    //   typeof(MeleeAttackPosition)
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
                // manager.AddBuffer<ReserveLocationTag>(baseDataEntity);
                // var meleeAttackPositions = manager.GetBuffer<MeleeAttackPosition>(baseDataEntity);
                // meleeAttackPositions.Length = 4;
                // for (var index = 0; index < meleeAttackPositions.Length; index++)
                // {
                //     var attackPosition = meleeAttackPositions[index];
                //     attackPosition.State = OccupiedState.Vacant;
                //     meleeAttackPositions[index] = attackPosition;
                // }

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
            public Entity Build()
            {
                return entity;
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

        }
    }
}
