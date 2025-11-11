using IAUS.ECS.Component;
using IAUS.ECS.Systems.Reactive;
using System.Collections;
using System.Collections.Generic;
using ProjectDawn.Navigation;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using Utilities.ReactiveSystem;

namespace Components.MovementSystem
{
    public partial class ChangeMoveSpeedSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities.WithChangeFilter<PatrolActionTag>().ForEach((ref AgentLocomotion agent, ref Movement mover) =>
            {
                agent.Speed = .65f * mover.MaxMovementSpeed;
            }).Schedule();

            Entities.WithChangeFilter<TraverseActionTag>().ForEach((ref AgentLocomotion agent, ref Movement mover) =>
            {
                agent.Speed = .65f * mover.MaxMovementSpeed;
            }).Schedule();
        }
    }
}