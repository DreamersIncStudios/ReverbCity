using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace AISenses.VisionSystems.Combat
{
    public struct AttackTarget : IComponentData
    {
        public float3 AttackTargetLocation;
        public int AttackTargetIndex;
        public float2 AttackDir;
        public float MoveRange;
        public bool TargetInRange { get; set; }
    }
}