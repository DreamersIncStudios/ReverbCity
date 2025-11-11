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

    partial struct PackAttackPositionLocator : IJobEntity
    {
        [ReadOnly] public BufferLookup<MeleeAttackPosition> MeleeAttackPositions;
        void Execute(in Pack pack, in LocalToWorld transform, DynamicBuffer<PackAttackTarget> buffer, DynamicBuffer<Enemies> enemies)
        {
            int meleePositions = 0;
            int rangePositions = 0;
            int supportPositions = 0;
            if (enemies.IsEmpty)
            {
                buffer.Clear();
                return;
            }
            foreach (var role in pack.Requirements)
            {
                switch (role.Role)
                {
                    case Role.Recon:
                    case Role.Combat:
                        rangePositions += role.QtyInfo.x;
                        meleePositions += role.QtyInfo.x;
                        break;
                    case Role.Scavengers:
                    case Role.Acquisition:
                        rangePositions += role.QtyInfo.x;
                        supportPositions += role.QtyInfo.x;
                        break;
                    case Role.Support:
                    case Role.Transport:
                        supportPositions += role.QtyInfo.x;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            var countMelee = 0;
            foreach (var target in buffer)
            {
                if (target.Role == Role.Combat || target.Role == Role.Recon)
                    countMelee++;
            }

            for (var i = 0; i < meleePositions - countMelee; i++)
            {
                foreach (var enemy in enemies)
                {
                    var meleePosition = MeleeAttackPositions[enemy.Target.Entity];
                    foreach (var position in meleePosition)
                        buffer.Add(new PackAttackTarget(position.Position, Role.Combat));
                }
            }
            var countRange = 0;
            foreach (var target in buffer)
            {
                if (target.Role == Role.Scavengers || target.Role == Role.Acquisition || target.Role == Role.Recon || target.Role == Role.Combat)
                    countRange++;
            }

            for (int i = 0; i < rangePositions - countRange; i++)
            {
                buffer.Add(new PackAttackTarget());

            }

            var countSupport
                = 0;
            foreach (var target in buffer)
            {
                if (target.Role == Role.Transport || target.Role == Role.Support)
                    countMelee++;
            }


            for (int i = 0; i < supportPositions - countSupport; i++)
            {
                buffer.Add(new PackAttackTarget());

            }
        }
    }
}
