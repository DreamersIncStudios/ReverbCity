using DreamersIncStudio.FactionSystem;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Global.Component
{
    [System.Serializable]
    public struct GlobalAITarget : IComponentData
    {
        public TargetType Type;
        public uint level { get; set; }
        public FactionNames FactionID;
        public int NumOfEntityTargetingMe;
        public bool CanBeTargeted => NumOfEntityTargetingMe < MaxNumberOfTarget;
        public int MaxNumberOfTarget; // base off of InfluenceValue Level
        public bool CanBeTargetByPlayer;
        public bool Attackable;
        public float3 CenterOffset;


        public float detectionScore;
    }
    [System.Serializable]
    public enum TargetType
    {
        None,
        Character,
        Location,
        Vehicle,
        Resource
    }
    public enum ClassTitle
    {
        Grunt, Soldier, Ranger, Archer, Sorcer, Mage, Monk, Swordman, Thief, Knight, Bot, Generalist, Pugiblist, Beast
    }
}

namespace DreamersIncStudio.FactionSystem
{
    public enum FactionNames
    {
        Player,
        Citizen,
        Daemon,
        Angel,
        Orcs,
    }
}
