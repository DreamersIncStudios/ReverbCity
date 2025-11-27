using UnityEngine.AI;
using Unity.Entities;
using Components.MovementSystem;
using Unity.Transforms;
using ProjectDawn.Navigation;

namespace IAUS.ECS.Systems
{
    public partial class MovementSystem : SystemBase
    {
        ComponentLookup<AgentBody> agentLookup;
        ComponentLookup<AgentLocomotion> agentLocLookup;
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()

        {
            agentLookup = GetComponentLookup<AgentBody>();
            agentLocLookup = GetComponentLookup<AgentLocomotion>();
            foreach (var (movement, root) in SystemAPI.Query<RefRW<Movement>, Parent>())
            {
                if(agentLookup.TryGetComponent(root.Value, out var agent))
                    movement.ValueRW.DistanceRemaining = agent.RemainingDistance;
            }
            foreach (var (movement, root) in SystemAPI.Query<RefRW<Movement>, Parent>())
            {
                if (!agentLookup.TryGetComponent(root.Value, out var agent))
                    return;
                if (!agentLocLookup.TryGetComponent(root.Value, out var agentLoc))
                    return;
                if (movement.ValueRO.CanMove)
                {
                    //rewrite with a set position bool;
                    if (!movement.ValueRW.SetTargetLocation) return;
                    if (!NavMesh.SamplePosition(movement.ValueRO.TargetLocation, out var hit, 5, NavMesh.AllAreas)) return;
                    movement.ValueRW.TargetLocation = hit.position;
                    agent.SetDestination(hit.position);
                    agentLoc.StoppingDistance = movement.ValueRO.StoppingDistance;

                    movement.ValueRW.SetTargetLocation = false;
                }
                else
                {
                    agent.IsStopped = true;
                }

                agentLookup[root.Value] = agent;
                agentLocLookup[root.Value] = agentLoc;
            }
        }
    }
}
