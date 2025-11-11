using Global.Component;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static DreamersInc.QuadrantSystems.GenericQuadrantSystem;

namespace DreamersInc.QuadrantSystems
{
    public partial struct NPCQuadrantSystem : ISystem
    {
        public static int GetPositionHashMapKey(float3 position)
        {
            return (int)(Mathf.Floor(position.x / QuadrantCellSize) +
                         (QuadrantZMultiplier * Mathf.Floor(position.z / QuadrantCellSize)));
        }

        public int GetEntityCountInHashMap(NativeParallelMultiHashMap<int, NPCQuadrantData> quadrantMap, int hashMapKey)
        {
            int count = 0;
            if (quadrantMap.TryGetFirstValue(hashMapKey, out NPCQuadrantData quadrantData,
                    out NativeParallelMultiHashMapIterator<int> iterator))
            {
                do
                {
                    count++;
                } while (quadrantMap.TryGetNextValue(out quadrantData, ref iterator));
            }

            return count;
        }

        private static void DebugDrawQuadrant(float3 position)
        {
            Vector3 lowerLeft = new(Mathf.Floor(position.x / QuadrantCellSize) * QuadrantCellSize,
                (QuadrantZMultiplier * Mathf.Floor(position.z / QuadrantCellSize) * QuadrantCellSize));
            Debug.DrawLine(lowerLeft, lowerLeft + new Vector3(+1, +0, +0) * QuadrantCellSize);
            Debug.DrawLine(lowerLeft, lowerLeft + new Vector3(+0, +0, +1) * QuadrantCellSize);
            Debug.DrawLine(lowerLeft + new Vector3(+1, +0, +0) * QuadrantCellSize,
                lowerLeft + new Vector3(+1, +0, +1) * QuadrantCellSize);
            Debug.DrawLine(lowerLeft + new Vector3(+0, +0, +1) * QuadrantCellSize,
                lowerLeft + new Vector3(+0, +0, +0) * QuadrantCellSize);
            Debug.Log(GetPositionHashMapKey(position) + "" + position);
        }

        private NativeParallelMultiHashMap<int, NPCQuadrantData> quadrantMultiHashMap;
        public const int QuadrantZMultiplier = 1000;
        private const int QuadrantCellSize = 300;
        private EntityQuery query;

        public void OnCreate(ref SystemState state)
        {
            quadrantMultiHashMap = new NativeParallelMultiHashMap<int, NPCQuadrantData>(0, Allocator.Persistent);
            query = state.GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                    { ComponentType.ReadWrite(typeof(LocalTransform)), ComponentType.ReadWrite(typeof(AITarget)) }
            });
        }

        public void OnDestroy(ref SystemState state)
        {
            quadrantMultiHashMap.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (query.CalculateEntityCount() > quadrantMultiHashMap.Capacity)
            {
                quadrantMultiHashMap.Clear();
                quadrantMultiHashMap.Capacity = query.CalculateEntityCount();
            }

            quadrantMultiHashMap.Clear();
            new SetQuadrantDataHashMapJob()
                { quadrantMap = quadrantMultiHashMap.AsParallelWriter() }.ScheduleParallel(query);
        }

        public struct NPCQuadrantData
        {
            public Entity entity;
            public float3 position;
            public AITarget npcData;
        }

        public struct NPCQuadrantEntity : IComponentData
        {
        }

        [BurstCompile]
        public partial struct SetQuadrantDataHashMapJob : IJobEntity
        {
            public NativeParallelMultiHashMap<int, NPCQuadrantData>.ParallelWriter quadrantMap;

            public void Execute(Entity entity, [ReadOnly] in LocalTransform transform, in AITarget targetInfo)
            {
                int hashMapKey = GetPositionHashMapKey(transform.Position);
                quadrantMap.Add(hashMapKey, new NPCQuadrantData
                {
                    entity = entity,
                    position = transform.Position,
                    npcData = targetInfo
                });
            }
        }
    }
}