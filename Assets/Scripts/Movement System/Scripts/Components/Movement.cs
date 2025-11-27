using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components.MovementSystem
{
    public struct Movement : IComponentData
    {
        public float3 TargetLocation;
        public float MaxMovementSpeed;

        //public float SprintSpeed // To Be Added if needed
        public bool CanMove;

        public float DistanceRemaining;
        public float StoppingDistance;

        public bool SetTargetLocation { get; set; }

        public void SetLocation(float3 position, float i = 0.0f)
        {
            TargetLocation = position;
            SetTargetLocation = true;
            CanMove = true;
        }
        public bool PositionCheck(float3 position)
        {
            return Mathf.Approximately(position.x, TargetLocation.x) && Mathf.Approximately(position.z, TargetLocation.z);

        }
    }



}
