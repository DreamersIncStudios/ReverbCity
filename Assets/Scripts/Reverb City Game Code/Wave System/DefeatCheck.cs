using System;
using DreamersInc.DamageSystem;
using DreamersInc.Trackers;
using Global.Component;
using IAUS.ECS.Component;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace DreamersInc.ReverbCity
{
    [UpdateBefore(typeof(DeathSystem))]
    public partial class DefeatCheck : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

            foreach (var (test, aiInfo) in SystemAPI.Query<BaseCharacterComponent,IAUSBrain>().WithChangeFilter<BaseCharacterComponent>())
            {
                if (!(test.HealthRatio <= 0)) continue;
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