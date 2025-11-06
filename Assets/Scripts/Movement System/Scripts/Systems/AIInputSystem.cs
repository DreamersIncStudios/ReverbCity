using Components.MovementSystem;
using DreamersInc.Global;
using MotionSystem.Components;
using MotionSystem.Systems;
using ProjectDawn.Navigation;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DreamersInc.MovementSys
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(InputSystem))]
    public partial class AIInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var moveLookup = SystemAPI.GetComponentLookup<Movement>();
            Entities.WithoutBurst().ForEach((DynamicBuffer<Child> children, Animator Anim, ref AgentBody Agent,
                ref CharControllerE Control) =>
            {
                var mover = new Movement();
                var found = false;

                // Find the first child that actually has Movement
                for (var i = 0; i < children.Length; i++)
                {
                    var child = children[i].Value;
                    if (!moveLookup.HasComponent(child)) continue;
                    mover = moveLookup[child];
                    found = true;
                    break;
                }

                if (found && mover.CanMove)
                {
                    Control.Move = Agent.Velocity;
                }
                else
                {
                    if (!Agent.IsStopped)
                    {
                        Agent.IsStopped = true;
                    }

                    Control.Move = float3.zero;
                }

                Control.Crouch = false;
                Control.Jump = false;
            }).Run();

            Entities.WithoutBurst().ForEach((DynamicBuffer<Child> children, Animator Anim, ref AgentBody Agent,
                ref BeastControllerComponent Control) =>
            {
                var mover = default(Movement);
                var found = false;

                // Find the first child that actually has Movement
                for (var i = 0; i < children.Length; i++)
                {
                    var child = children[i].Value;
                    if (!moveLookup.HasComponent(child)) continue;
                    mover = moveLookup[child];
                    found = true;
                    break;
                }

                if (found && mover.CanMove)
                {
                    Control.Move = Agent.Velocity;
                }
                else
                {
                    if (!Agent.IsStopped)
                    {
                        Agent.IsStopped = true;
                    }

                    Control.Move = float3.zero;
                }

                Control.Jump = false;
            }).Run();
        }
    }
}