using Unity.Entities;
using Unity.Mathematics;

namespace Components.MovementSystem
{
    public struct Movement : IComponentData
    {
        public float3 TargetLocation;
        public float MaxMovementSpeed;

        //public float SprintSpeed // To Be Added if needed
        public bool CanMove;

        public float DistanceRemaining;


        public bool SetTargetLocation { get; set; }

        public void SetLocation(float3 position)
        {
            TargetLocation = position;
            SetTargetLocation = true;
            CanMove = true;
        }

    }



}
