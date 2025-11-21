using System;
using DreamersInc.DamageSystem;
using DreamersInc.Trackers;
using Global.Component;
using IAUS.ECS.Component;
using Stats.Entities;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DreamersInc.ReverbCity
{
    [UpdateBefore(typeof(DeathSystem))]
    public partial class DefeatCheck : SystemBase
    {

        ComponentLookup<IAUSBrain> brainLookup;        
        
        protected override void OnUpdate()
        {
            var ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>(); 
            brainLookup =  SystemAPI.GetComponentLookup<IAUSBrain>();
            foreach (var (test, children) in SystemAPI.Query<BaseCharacterComponent,DynamicBuffer<Child>>().WithChangeFilter<BaseCharacterComponent>())
            {
                if (!(test.HealthRatio <= 0)) continue;

                var aiInfo = new IAUSBrain();
                foreach (var child in children)
                {
                    if (brainLookup.HasComponent(child.Value))
                      aiInfo = brainLookup[child.Value];
                }
             
                WaveManager.IncrementDefeatCount();
                var score = aiInfo.rank switch
                {
                    Rank.Private => 200,
                    Rank.Specialist => 350,
                    Rank.Sargeant => 500,
                    Rank.SFC => 1000,
                    Rank.FSG => 1500,
                    Rank.Lieutenant => 2500,
                    Rank.Captain => 5000,
                    _ => throw new ArgumentOutOfRangeException()
                };
                CounterManager.Increment(score); 
            }
   
        }
    }
}