using System;
using System.Linq;
using Components.MovementSystem;
using Dreamers.MotionSystem;
using ProjectDawn.Navigation;
using Stats.Entities;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace IAUS.ECS.Component
{
    public readonly partial struct  MaintAspect : IAspect
    {
        private readonly RefRW<MaintenanceState> state;
        private readonly RefRO<LocalTransform> transform;
        private readonly RefRO<AIStat> stats;
        private readonly RefRW<Movement> move;
        private readonly RefRO<AgentBody> agent;
        [Optional] private readonly RefRO<CarryingTag> carrying;

        private bool IsHealthy => stats.ValueRO.HealthRatio > .725f;
        private bool IsInDanger => stats.ValueRO.HealthRatio < .35f;
       
        public void SetPlan(Entity entity, MaintPlan plan, float3 location)
        {
            state.ValueRW.EntityWhichNeedsMaintenance = entity;
            state.ValueRW.MaintLocation = location;
            state.ValueRW.MaintNeeded = plan;
        }

        public void DeterminePlan()
        {
            if(state.ValueRO.MaintenancePlan.Length!=0)return;
            int[] scores = new[]
            {
                -1 , Rest, GetAmmo, Refuel, LocateFuel, MoveToObject, GetParts, GetTools, Repair, Rebuild, Upgrade, Destroy, ReloadMachine, GotoMachine
            };
            var sortedScores = scores.ToList().OrderByDescending(x => x);
            foreach (var score in sortedScores)
            {
                if (score <= 0) continue;
                if(state.ValueRW.MaintenancePlan.Length>=8)return;
                var index = scores.ToList().IndexOf(score);
                state.ValueRW.MaintenancePlan.Add((MaintPlan)(index));
            }
        }

        private int GotoMachine => 0;

        private int ReloadMachine =>0;

        private int Rest => state.ValueRO.MaintNeeded == MaintPlan.None ? 1 : 0;

        private int GetAmmo    {
            get
            {
                if(state.ValueRO.MaintNeeded != MaintPlan.ReloadMachine) return 0;
                return carrying is { IsValid: true, ValueRO: { ItemType: CarryItemType.Ammo } } ? 0 : 10;
            }
        }
        private int Refuel    {
            get
            {
                if(state.ValueRO.MaintNeeded != MaintPlan.Refuel) return 0;
                return carrying is { IsValid: true, ValueRO: { ItemType: CarryItemType.Fuel } } ? 7 : 6;
            }
        }
        private int LocateFuel    {
            get
            {
                if(state.ValueRO.MaintNeeded != MaintPlan.Refuel) return 0;
                return carrying is { IsValid: true, ValueRO: { ItemType: CarryItemType.Fuel } } ? 0 : 9 ;
            }
        }

        private int MoveToObject
        {
            get
            {
                if (state.ValueRO.MaintNeeded is not (MaintPlan.Refuel or MaintPlan.Repair or MaintPlan.Rebuild)) return 0;
                if (carrying.IsValid && carrying.ValueRO.EntityBeingCarried != Entity.Null)
                    return 0;
                else
                {
                    return 8;
                }
            }
        }

        private int GetParts    {
            get
            {
                if(state.ValueRO.MaintNeeded is not (MaintPlan.Repair or MaintPlan.Rebuild)) return 0;

                if (carrying.IsValid && carrying.ValueRO.EntityBeingCarried != Entity.Null)
                    return 0;
                else
                {
                    return 10;
                }
            }
        }
        private int GetTools    {
            get
            {
                if(state.ValueRO.MaintNeeded is not (MaintPlan.Repair or MaintPlan.Rebuild)) return 0;
                if (carrying.IsValid && carrying.ValueRO.EntityBeingCarried != Entity.Null)
                    return 0;
                else
                {
                    return 10;
                }
            }
        }
        private int Repair    {
            get { return 0; }
        }
        private int Rebuild    {
            get { return 0; }
        }
        private int Upgrade    {
            get { return 0; }
        }
        private int Destroy    {
            get { return 0; }
        }
    }
}