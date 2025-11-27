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
}
