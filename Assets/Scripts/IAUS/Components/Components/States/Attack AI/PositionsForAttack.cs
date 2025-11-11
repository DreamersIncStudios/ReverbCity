using DreamersInc.InfluenceMapSystem;
using Global.Component;
using IAUS.ECS.Systems;
using IAUS.ECS.Systems.Reactive;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace IAUS.ECS.Component.Attacking
{
    internal interface AttackPosition : IBufferElementData
    {
        public OccupiedState State { get; set; }
        public float Usability { get; set; }
        void SetPosition(float3 position);
    }

    public enum OccupiedState
    {
        Vacant,
        Reserved,
        Occupied
    }

    [InternalBufferCapacity(6)]
    public struct MeleeAttackPosition : AttackPosition
    {
        public float3 Position;
        public OccupiedState State { get; set; }
        public float Usability { get; set; }

        public static implicit operator float3(MeleeAttackPosition e)
        {
            return e.Position;
        }

        public static implicit operator MeleeAttackPosition(float3 e)
        {
            return new MeleeAttackPosition { Position = e };
        }


        public void SetPosition(float3 position)
        {
            Position = position;
        }
    }


    public struct ReserveLocationTag : IBufferElementData
    {
        public int ID;
        public Entity ReserveEntity;
    }

    [UpdateInGroup(typeof(IAUSUpdateGroup))]
    [UpdateAfter(typeof(AttackTagReactor.AttackUpdateSystem))]
    public partial class UpdateAttackPositionSystem : SystemBase
    {
        private CollisionWorld collisionWorld;

        protected override void OnUpdate()
        {
            Entities.WithChangeFilter<LocalToWorld>().ForEach((DynamicBuffer<MeleeAttackPosition> attackPosition,
                    ref LocalToWorld transform) =>
                {
                    for (var i = 0; i < 4; i++)
                    {
                        var temp = attackPosition[i];

                        var target = i switch
                        {
                            0 => transform.Position + transform.Forward * 1.5f,
                            1 => transform.Position + transform.Right * 1.5f,
                            2 => transform.Position - transform.Forward * 1.5f,
                            3 => transform.Position - transform.Right * 1.5f,
                            _ => new float3()
                        };
                        temp.SetPosition(target);

                        attackPosition[i] = temp;
                    }
                })
                .ScheduleParallel();
        }
    }
}