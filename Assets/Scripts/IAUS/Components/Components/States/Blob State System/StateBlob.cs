using Unity.Collections;
using Unity.Entities;
using IAUS.ECS.Component;
using IAUS.ECS.Consideration;
using UnityEngine;

namespace IAUS.ECS.StateBlobSystem
{
    public struct StateAsset
    {
        public Identity ID;
        public ConsiderationScoringData Health;
        public ConsiderationScoringData DistanceToPlaceOfInterest;
        public ConsiderationScoringData Timer;
        public ConsiderationScoringData ManaAmmo;
        public ConsiderationScoringData ManaAmmo2;
        public ConsiderationScoringData DistanceToTargetLocation;
        public ConsiderationScoringData DistanceToTargetEnemy;
        public ConsiderationScoringData DistanceToTargetAlly;
        public ConsiderationScoringData EnemyInfluence;
        public ConsiderationScoringData FriendlyInfluence;
    }

    public struct AIStateBlobAsset
    {
        public BlobArray<StateAsset> Array;

        public int GetConsiderationIndex(Identity identify)
        {
            if (Array.Length == 0)
            {
                Debug.Log("Empty Array");
                return -1;
            }
            var index = -1;
            var allDif = identify;
            allDif.Difficulty = Difficulty.All;
            for (var i = 0; i < Array.Length; i++)
            {
                if (!Array[i].ID.Equals(identify)) continue;
                index = i;
                return index;
            }
            for (var i = 0; i < Array.Length; i++)
            {
                if (!Array[i].ID.Equals(allDif)) continue;
                index = i;
                return index;
            }
            return index;
        }

    }
    public struct Identity
    {
        public NPCLevel NPCLevel;
        public int FactionID;
        public AIStates AIStates;
        public Difficulty Difficulty;

        public override string ToString()
        {
            return NPCLevel.ToString() + " " + FactionID.ToString() + " " + Difficulty.ToString() + " " + AIStates.ToString();
        }
    }

    [UpdateBefore(typeof(IAUS.ECS.Systems.IAUSBrainSetupSystem))]
    public partial class SetupAIStateBlob : SystemBase
    {
        BlobAssetReference<AIStateBlobAsset> reference;
        protected override void OnCreate()
        {
            base.OnCreate();
            reference = CreateReference();
        }

        //TODO Get diffultity from manager singleton 

        protected override void OnUpdate()
        {
            foreach (var query in
                     SystemAPI.Query<RefRW<IAUSBrain>, DynamicBuffer<StateData>>()
                         .WithAll<SetupBrainTag>())
            {
                var brain = query.Item1;
                var statesToCheck = query.Item2;
                brain.ValueRW.State = reference;
                for (int i = 0; i < statesToCheck.Length; i++)
                {
                    var s = statesToCheck[i];

                    s.SetIndex(reference.Value.GetConsiderationIndex(new Identity
                    {
                        Difficulty = brain.ValueRO.Difficulty,
                        AIStates = s.State,
                        FactionID = (int)brain.ValueRO.FactionID,
                        NPCLevel = brain.ValueRO.NPCLevel
                    }));
                    s.SetStatus(ActionStatus.Idle);
                    statesToCheck[i] = s; // write back
                }
            }
            ;

        }

        BlobAssetReference<AIStateBlobAsset> CreateReference()
        {
            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var stateBlobAsset = ref blobBuilder.ConstructRoot<AIStateBlobAsset>();
            var assign = StateSOReader.SetupStateAsset();

            var array = blobBuilder.Allocate(ref stateBlobAsset.Array, assign.Length);

            for (int i = 0; i < assign.Length; i++)
            {
                array[i] = assign[i];
            }


            var blobAssetReference = blobBuilder.CreateBlobAssetReference<AIStateBlobAsset>(Allocator.Persistent);

            return blobAssetReference;
        }
    }

}
