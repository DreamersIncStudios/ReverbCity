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
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()

        {
            agentLookup = GetComponentLookup<AgentBody>();
            foreach (var (movement, root) in SystemAPI.Query<RefRW<Movement>, RefRO<Parent>>())
            {
                var agent = agentLookup[root.ValueRO.Value];
                movement.ValueRW.DistanceRemaining = agent.RemainingDistance;
            }
            foreach (var (movement, root) in SystemAPI.Query<RefRW<Movement>, Parent>())
            {
                var agent = agentLookup[root.Value];

                if (movement.ValueRO.CanMove)
                {
                    //rewrite with a set position bool;
                    if (!movement.ValueRW.SetTargetLocation) return;
                    if (!NavMesh.SamplePosition(movement.ValueRO.TargetLocation, out var hit, 5, NavMesh.AllAreas)) return;
                    movement.ValueRW.TargetLocation = hit.position;
                    agent.SetDestination(hit.position);

                    movement.ValueRW.SetTargetLocation = false;
                }
                else
                {
                    agent.IsStopped = true;
                }

                agentLookup[root.Value] = agent;
            }
        }
    }
}
