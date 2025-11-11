using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace IAUS.ECS.Component
{


    public struct DestroyTargetTag : IComponentData
    {
        public float3 TargetPosition;
        public float3 AttackPosition;
        public Entity TargetEntity;
        public HowToAttack AttackType { get; set; }

        public SerializableGuid LocationGuid;
        public float2 InfluenceAtTarget;
        public FixedList32Bytes<AttackPlan> AttackPlans;

        public float AttackResetTimer;
        public bool InAttackCooldown => AttackResetTimer != 0.0f;
    }
}
