using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace IAUS.ECS.Component
{
    public interface IBattleState : IBaseStateScorer
    {
        public float3 TargetPosition { get; set; }
        public float3 AttackPosition { get; set; }
        public Entity TargetEntity { get; set; }
        public HowToAttack AttackType { get; set; }
    }
}
