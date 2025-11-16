using DreamersInc.DamageSystem;
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

            foreach (var (test, entity) in SystemAPI.Query<BaseCharacterComponent>().WithEntityAccess().WithChangeFilter<BaseCharacterComponent>())
            {
                if (!(test.HealthRatio <= 0)) continue;
                
      
            }
   
        }
    }
}