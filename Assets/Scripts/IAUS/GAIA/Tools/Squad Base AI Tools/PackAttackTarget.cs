using System;
using AISenses;
using IAUS.ECS.Component.Attacking;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
namespace DreamersIncStudio.GAIACollective.Squad_Base_AI_Tools
{
    [InternalBufferCapacity(0)]
    public struct PackAttackTarget : IBufferElementData
    {
        public float3 Position;
        public Role Role;
        public bool Occupied;
        public PackAttackTarget(float3 position, Role role)
        {
            Position = position;
            Role = role;
            Occupied = false;
        }
    }
    
}
