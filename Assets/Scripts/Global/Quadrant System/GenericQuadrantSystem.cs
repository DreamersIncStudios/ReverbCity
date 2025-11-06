using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DreamersInc.QuadrantSystems
{
    [DisableAutoCreation]
    public partial class GenericQuadrantSystem : SystemBase
    {
        private NativeParallelMultiHashMap<int, QuadrantData> quadrantMultiHashMap;
        private const int QuadrantYMultiplier = 1000;
        private const int QuadrantCellSize = 75;
        protected EntityQuery Query;

        protected static int GetPositionHashMapKey(float3 position)
        {
            return (int)(Mathf.Floor(position.x / QuadrantCellSize) + (QuadrantYMultiplier * Mathf.Floor(position.z / QuadrantCellSize)));
        }
        public int GetEntityCountInHashMap(NativeParallelMultiHashMap<int, QuadrantData> quadrantMap, int hashMapKey)
        {
            var count = 0;
            if (!quadrantMap.TryGetFirstValue(hashMapKey, out QuadrantData quadrantData,
                    out NativeParallelMultiHashMapIterator<int> iterator)) return count;
            do
            {
                count++;
            }
            while (quadrantMap.TryGetNextValue(out quadrantData, ref iterator));
            return count;
        }
        private static void DebugDrawQuadrant(float3 position)
        {
            var lowerLeft = new Vector3(Mathf.Floor(position.x / QuadrantCellSize) * QuadrantCellSize, (QuadrantYMultiplier * Mathf.Floor(position.z / QuadrantCellSize) * QuadrantCellSize));
            Debug.DrawLine(lowerLeft, lowerLeft + new Vector3(+1,0, +0) * QuadrantCellSize);
            Debug.DrawLine(lowerLeft, lowerLeft + new Vector3(+0,0, +1) * QuadrantCellSize);
            Debug.DrawLine(lowerLeft + new Vector3(+1,0, +0) * QuadrantCellSize, lowerLeft + new Vector3(+1,0, +1) * QuadrantCellSize);
            Debug.DrawLine(lowerLeft + new Vector3(+0,0, +1) * QuadrantCellSize, lowerLeft + new Vector3(+1, 0, +1) * QuadrantCellSize);
            Debug.Log(GetPositionHashMapKey(position) + "" + position);

        }
        protected override void OnCreate()
        {
            quadrantMultiHashMap = new NativeParallelMultiHashMap<int, QuadrantData>(0, Allocator.Persistent);
            base.OnCreate();
        }
        protected override void OnDestroy()
        {
            quadrantMultiHashMap.Dispose();

            base.OnDestroy();
        }
        protected override void OnUpdate()
        {
            quadrantMultiHashMap.Clear();
            if (Query.CalculateEntityCount() >quadrantMultiHashMap.Capacity)
            {
                quadrantMultiHashMap.Clear();
                quadrantMultiHashMap.Capacity = Query.CalculateEntityCount();

            }
            new SetQuadrantDataHashMapJob() { quadrantMap = quadrantMultiHashMap.AsParallelWriter() }.Schedule(Query);

        }


        [BurstCompile]
        private partial struct SetQuadrantDataHashMapJob : IJobEntity
        {
            public NativeParallelMultiHashMap<int, QuadrantData>.ParallelWriter quadrantMap;

            private void Execute(Entity entity, [ReadOnly] in LocalTransform transform)
            {
                int hashMapKey = GetPositionHashMapKey(transform.Position);
                quadrantMap.Add(hashMapKey, new QuadrantData
                {
                    entity = entity,
                    position = transform.Position
                });
            }
        }
        public struct QuadrantData
        {
            public Entity entity;
            public float3 position;
        }
    }
}