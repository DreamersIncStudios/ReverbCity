using IAUS.Core.GOAP;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace IAUS.ECS.Component
{
    public struct AttackCapable : IComponentData
    {
        public bool CapableOfMelee, CapableOfMagic, CapableOfProjectile;

        public AttackCapable(bool capableOfMelee, bool capableOfMagic, bool capableOfRange)
        {
            CapableOfMelee = capableOfMelee;
            CapableOfMagic = capableOfMagic;
            CapableOfProjectile = capableOfRange;
        }
    }

    public interface IAttackState : IComponentData
    {
        public float3 TargetPosition { get; set; }
        public float3 AttackPosition{ get; set; }
        public Entity TargetEntity{ get; set; }
        public int TargetPositionID{ get; set; }
        public HowToAttack AttackType{ get; set; }
        public FixedList32Bytes<AttackPlan> AttackPlans{ get; set; }
        public float AttackResetTimer{ get; set; }
        public bool InAttackCooldown => AttackResetTimer != 0.0f;
    }

    public struct AttackActionTag : IAttackState
    {
        public float3 TargetPosition { get; set; }
        public float3 AttackPosition{ get; set; }
        public Entity TargetEntity{ get; set; }
        public int TargetPositionID{ get; set; }
        public HowToAttack AttackType{ get; set; }
        public FixedList32Bytes<AttackPlan> AttackPlans{ get; set; }
        public float AttackResetTimer{ get; set; }


    }
    public struct AttackGlobalTag : IAttackState
    {
        public float3 TargetPosition { get; set; }
        public float3 AttackPosition{ get; set; }
        public Entity TargetEntity{ get; set; }
        public int TargetPositionID{ get; set; }
        public HowToAttack AttackType{ get; set; }
        public FixedList32Bytes<AttackPlan> AttackPlans{ get; set; }
        public float AttackResetTimer{ get; set; }

    }
    public struct TargetThisCommand : IComponentData
    {
        public Entity Target;
        public float3 LastKnownPosition;
        public TargetThisCommand(Entity targetEntity, float3 targetLastKnownPosition)
        {
            Target = targetEntity;
            LastKnownPosition = targetLastKnownPosition;
        }
    }

    public enum AttackPlan
    {
        None,
        Rest,
        Wander,
        GetTargetLocation,
        GetAttackLocation,
        MoveToLocationMelee,
        MoveToLocationMagic,
        MoveToLocationRange,
        AttackMelee,
        AttackMagic,
        AttackRange,
        Evade,
        MoveToInRange,
        PauseInTargetRange
    }

    public enum HowToAttack
    {
        None,
        Melee,
        Magic,
        Range
    }
}
