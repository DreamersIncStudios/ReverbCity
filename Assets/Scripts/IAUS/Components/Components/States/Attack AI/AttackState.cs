using IAUS.Core.GOAP;
using Sirenix.OdinInspector;
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
    
 
        public float AttackResetTimer{ get; set; }
        public bool InAttackCooldown => AttackResetTimer != 0.0f;
    }

    public struct AttackActionTag : IAttackState
    {
        public float3 TargetPosition { get=>targetPosition; set=>targetPosition = value; }
        [SerializeField]
        private float3 targetPosition;
        public float3 AttackPosition{get => attackPosition; set => attackPosition= value; }
        [SerializeField]
        private float3 attackPosition;
        public Entity TargetEntity{get => targetEntity; set => targetEntity = value; }
        [SerializeField] Entity targetEntity;
        public int TargetPositionID{ get => targetPositionID; set => targetPositionID = value; }
        [SerializeField] int targetPositionID;
        public HowToAttack AttackType;
        public FixedList32Bytes<AttackPlan> AttackPlans;
        
        public float AttackResetTimer {get => attackResetTimer; set => attackResetTimer = value; }
        [SerializeField]  private float attackResetTimer;


    }
    public struct AttackGlobalTag : IAttackState
    {
        public float3 TargetPosition { get=>targetPosition; set=>targetPosition = value; }
        [SerializeField]
        private float3 targetPosition;
        public float3 AttackPosition{get => attackPosition; set => attackPosition= value; }
        [SerializeField]
        private float3 attackPosition;
        public Entity TargetEntity{get => targetEntity; set => targetEntity = value; }
        [SerializeField] Entity targetEntity;
        public int TargetPositionID{ get => targetPositionID; set => targetPositionID = value; }
        [SerializeField] int targetPositionID;
        public HowToAttack AttackType;
        public FixedList32Bytes<AttackPlan> AttackPlans;
        
        public float AttackResetTimer {get => attackResetTimer; set => attackResetTimer = value; }
       [SerializeField] private float attackResetTimer;
     
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
