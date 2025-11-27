using System;
using Components.MovementSystem;
using IAUS.ECS.Component;
using Stats.Entities;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
namespace IAUS.ECS.Systems.Reactive
{


    partial struct DetermineAttackActionGlobal : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        // Thresholds/constants extracted for readability and reuse
        private const float LowHealthThreshold = 0.35f;
        private const float SafeHealthThreshold = 0.425f;
        private const float HighHealthThreshold = 0.65f;
        private const float MeleeRange = 3f;
        private const float TravelMeleeRange = 6f;
        private const float TravelMagicRange = 10f;
        private const float TravelRangeRange = 10f;

        void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref AttackGlobalTag state, in AIStat stat, in LocalToWorld transform, in AttackCapable capable)
        {
            if (state.AttackPlans.Length != 0) return;

            state.AttackType = DeterminePrimaryAttackType(state, stat, transform, capable);

            // Build scored plan list as (plan, score) pairs to keep index without re-searching
            var scoredPlans = new (AttackPlan plan, int score)[]
            {
                (AttackPlan.None, -1),
                (AttackPlan.Rest, ComputeRestScore(state, stat)),
                (AttackPlan.Wander, WanderScore),
                (AttackPlan.GetTargetLocation, ScoreGetTargetLocation(state)),
                (AttackPlan.GetAttackLocation, ScoreGetAttackLocation(state)),
                (AttackPlan.MoveToLocationMelee, state.TargetPosition.Equals(float3.zero)?0: ScoreTravelToTargetMelee(state, stat, transform, capable)),
                (AttackPlan.MoveToLocationMagic, state.TargetPosition.Equals(float3.zero)?0: ScoreTravelToTargetMagic(state, stat, transform, capable)),
                (AttackPlan.MoveToLocationRange,state.TargetPosition.Equals(float3.zero)?0:  ScoreTravelToTargetRange(state, stat, transform, capable)),
                (AttackPlan.AttackMelee, state.AttackPosition.Equals(float3.zero)?0: ScoreMelee(state, stat, transform, capable)),
                (AttackPlan.AttackMagic, state.AttackPosition.Equals(float3.zero)?0: ScoreMagic(state, stat, transform, capable)),
                (AttackPlan.AttackRange, state.AttackPosition.Equals(float3.zero)?0: ScoreRange(state, stat, transform, capable)),
                (AttackPlan.Evade, ScoreEvade(state, stat))
            };
            
            System.Array.Sort(scoredPlans, (a, b) => b.score.CompareTo(a.score));

   
            foreach (var entry in scoredPlans)
            {
                if (entry.score <= 0) continue;
                if (state.AttackPlans.Length >= 8) return;
                state.AttackPlans.Add(entry.plan);
                if (state.AttackPlans.Length >= 8) break;
            }

        }
        private int WanderScore => 0;
        
        private HowToAttack DeterminePrimaryAttackType(IAttackState state, AIStat stat, LocalToWorld transform, AttackCapable capable)
        {
            // Score attack types and pick the best
            var typeScores = new (HowToAttack type, int score)[]
            {
                (HowToAttack.None, -1),
                (HowToAttack.Melee, ScoreMelee(state, stat, transform, capable)),
                (HowToAttack.Magic, ScoreMagic(state, stat, transform, capable)),
                (HowToAttack.Range, ScoreRange(state, stat, transform, capable))
            };
            System.Array.Sort(typeScores, (a, b) => b.score.CompareTo(a.score));
            return typeScores[0].type;
        }
        
        private static bool IsHealthAtLeast(AIStat stats, float threshold) => stats.HealthRatio > threshold;

        private bool IsCoverInRange()
        {
            return false;
        }

        private bool IsManaLow()
        {
            return false;
        }

        private bool IsAmmoLow()
        {
            return false;
        }
        private bool IsInAttackRange(in IAttackState state, float range, in LocalToWorld transform)
        {
            if (state.AttackPosition.Equals(float3.zero) && state.TargetPosition.Equals(float3.zero)) return false;
            var distance = Vector3.Distance(transform.Position, !state.AttackPosition.Equals(float3.zero) ? state.AttackPosition : state.TargetPosition);
            return distance <= range;
        }
        
        private static bool HasMultipleAttackCapabilities(in AttackCapable capable) =>
            capable is { CapableOfMagic: true, CapableOfMelee: true }
                or { CapableOfMagic: true, CapableOfProjectile: true }
                or { CapableOfMelee: true, CapableOfProjectile: true };

        private int ScoreMelee(IAttackState state, AIStat stats, LocalToWorld transform, AttackCapable capable)
        {
            if (state.InAttackCooldown || !capable.CapableOfMelee) return 0;

            var score = 2;
            if (IsHealthAtLeast(stats, SafeHealthThreshold)) score++;

            if (!IsCoverInRange() && (capable.CapableOfMagic || capable.CapableOfProjectile))
            {
                // Favor melee a bit more when no cover and other options exist
                score++;
            }

            if (HasMultipleAttackCapabilities(capable))
            {
                if (!capable.CapableOfMagic && IsManaLow()) score++;
                if (!capable.CapableOfProjectile && IsAmmoLow()) score++;
            }
            
            if (!IsInAttackRange(state, MeleeRange, transform)) return score;
            return score + 1;
        }

        private int ScoreRange(IAttackState state, AIStat stats, LocalToWorld transform, AttackCapable capable)
        {
            if (state.InAttackCooldown || !capable.CapableOfProjectile) return 0;

            var score = 2;
            if (!IsCoverInRange()) score++;

            if (HasMultipleAttackCapabilities(capable))
            {
                if (!capable.CapableOfMagic && IsManaLow()) score++;
                if (!capable.CapableOfMelee && !IsAmmoLow()) score++;
            }

            if (!IsInAttackRange(state, MeleeRange, transform)) return score;
            return score + 1;
        }

        private int ScoreMagic(IAttackState state, AIStat stats, LocalToWorld transform, AttackCapable capable)
        {
            if (state.InAttackCooldown || !capable.CapableOfMagic) return 0;

            var score = 2;
            if (!IsCoverInRange()) score++;

            if (HasMultipleAttackCapabilities(capable))
            {
                if (!capable.CapableOfMelee && !IsManaLow()) score++;
                if (!capable.CapableOfProjectile && IsAmmoLow()) score++;
            }

            if (!IsInAttackRange(state, MeleeRange, transform)) return score;
            return score + 1;
        }
        private int ComputeRestScore(IAttackState state, AIStat stats)
        {
            var score = 0;
            if (stats.HealthRatio < LowHealthThreshold) return score;
            if (state.InAttackCooldown) score = 3;
            return score;
        }
        private static int ScoreGetAttackLocation(in IAttackState state) => state.AttackPosition.Equals(float3.zero) ? 10 : 0;

        private static int ScoreGetTargetLocation(in IAttackState state) => state.TargetPosition.Equals(float3.zero) ? 15 : 0;
        private int ScoreTravelToTargetMelee(IAttackState state, AIStat stats, LocalToWorld transform, AttackCapable capable)
        {
            if (!capable.CapableOfMelee) return 0;
            if (IsInAttackRange(state, TravelMeleeRange, transform) ) return 0;

            int score = 1;
            if (stats.HealthRatio < LowHealthThreshold) return score;

            score++;
            if (stats.HealthRatio > HighHealthThreshold) score++;
            return score;
        }

        private int ScoreTravelToTargetMagic(IAttackState state, AIStat stats, LocalToWorld transform, AttackCapable capable)
        {
            if (!capable.CapableOfMagic || IsInAttackRange(state, TravelMagicRange, transform)) return 0;

            var score = 3;
            if (stats.HealthRatio > LowHealthThreshold)
            {
                if (capable.CapableOfMelee) score++;
                else return score;
            }
            if (stats.HealthRatio > HighHealthThreshold) score++;
            return score;
        }

        private int ScoreTravelToTargetRange(IAttackState state, AIStat stats, LocalToWorld transform, AttackCapable capable)
        {
            if (!capable.CapableOfProjectile || IsInAttackRange(state, TravelRangeRange, transform)) return 0;

            var score = 2;
            if (!(stats.HealthRatio < LowHealthThreshold)) return score;

            if (capable.CapableOfMelee) score++;
            return score;
        }
        private int ScoreEvade(IAttackState state, AIStat stats)
        {
            return stats.HealthRatio > LowHealthThreshold ? 0 : 2;
        }

    }

    public partial struct ExecuteAttackActionGlobal : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        private void Execute(Entity entity, [ChunkIndexInQuery] int chunkIndex, ref AttackGlobalTag state, ref Movement move, in TargetThisCommand command) 
        {
            if (state.AttackPlans.IsEmpty) return;
            switch (state.AttackPlans[0])
            {
                case AttackPlan.None:
                    break;
                case AttackPlan.Rest:
                    state.AttackResetTimer -= DeltaTime;
                    if (state.AttackResetTimer <= 0.0f)
                    {
                        state.AttackResetTimer = 0.0f;
                        state.AttackPlans.RemoveAt(0);
                    }
                    break;
                case AttackPlan.Wander:
                    break;
                case AttackPlan.GetTargetLocation:
                    state.TargetEntity= command.Target;
                    state.TargetPosition = command.LastKnownPosition;
                    if(!state.TargetPosition.Equals(float3.zero))
                        state.AttackPlans.RemoveAt(0);
                    break;
                case AttackPlan.GetAttackLocation:
                    break;
                case AttackPlan.MoveToLocationMelee:
                case AttackPlan.MoveToLocationMagic:
                case AttackPlan.MoveToLocationRange:
                    if (move.DistanceRemaining < 2 && move.PositionCheck(state.AttackPosition) && !move.TargetLocation.Equals(float3.zero))
                    {
                        state.AttackPlans.RemoveAt(0);
                    }
                    if (move.DistanceRemaining < 10 && move.PositionCheck(state.TargetPosition)&& !move.TargetLocation.Equals(float3.zero))
                    {
                        state.AttackPlans.RemoveAt(0);
                        state.AttackResetTimer = 15;
                    }

                    if (!move.TargetLocation.Equals(state.AttackPosition))
                    {
                        if (!state.AttackPosition.Equals(float3.zero))
                            move.SetLocation(state.AttackPosition);
                        return;
    
                    }
                    if (!move.TargetLocation.Equals(state.TargetPosition))
                    {
                        if (!state.TargetPosition.Equals(float3.zero))
                        {
                            move.SetLocation(state.TargetPosition, 8);
                            move.SetTargetLocation = true;
                        }
                    }

                    break;
                case AttackPlan.AttackMelee:
                case AttackPlan.AttackMagic:
                case AttackPlan.AttackRange:
                    if(state.TargetPosition.Equals(float3.zero))
                        state.AttackPlans.RemoveAt(0);
                    state.AttackResetTimer = 15; //Todo make a variable based off attack and difficulty 
                    ECB.AddComponent<SelectAndAttack>(chunkIndex, entity);
                    state.AttackPlans.RemoveAt(0);
                    break;
                case AttackPlan.Evade:
                    break;
                case AttackPlan.MoveToInRange:
                    break;
                case AttackPlan.PauseInTargetRange:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
    }
    
}
