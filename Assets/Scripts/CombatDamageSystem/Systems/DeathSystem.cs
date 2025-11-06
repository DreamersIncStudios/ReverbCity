using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Stats.Entities;

namespace DreamersInc.DamageSystem
{
    public partial class DeathSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

            foreach (var (test, entity) in SystemAPI.Query<BaseCharacterComponent>().WithEntityAccess().WithChangeFilter<BaseCharacterComponent>())
            {
                if (!(test.HealthRatio <= 0)) continue;
                
                var go = test.GORepresentative;
                Object.Destroy(go);
                ecbSystem.CreateCommandBuffer().DestroyEntity(entity);

            }
            foreach (var (test, entity) in SystemAPI.Query<BaseCharacterComponent>().WithAll<DeathTag>().WithEntityAccess().WithChangeFilter<BaseCharacterComponent>())
            {
                var go = test.GORepresentative;
                Object.Destroy(go);
                ecbSystem.CreateCommandBuffer().DestroyEntity(entity);
            }
        }
    }

    public struct DeathTag : IComponentData
    {
    }
}