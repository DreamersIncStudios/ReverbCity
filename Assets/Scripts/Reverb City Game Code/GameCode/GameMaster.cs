using DreamersInc.ReverbCity;
using Unity.Entities;
using UnityEngine;


namespace DreamersInc.SceneManagement
{
    public static class GameMaster
    {
        private static SerializableGuid PlayerGuid;
        
        public static Entity PlayerEntity;
        
        public static void RegisterPlayer(Entity player)=> PlayerEntity = player;
        public static void DeregisterPlayer() => PlayerEntity = Entity.Null;

        public static void RegisterPlayerGuid(string hex) => PlayerGuid = SerializableGuid.FromHexString(hex);
        public static SerializableGuid GetPlayerGuid() => PlayerGuid;

    }
}