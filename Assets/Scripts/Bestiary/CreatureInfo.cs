using System.Collections.Generic;
using System.Threading.Tasks;
using DreamersIncStudio.FactionSystem;
using Global.Component;
using IAUS.ECS;
using IAUS.ECS.Component;
using MotionSystem.Components;
using Sirenix.OdinInspector;
using Stats;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bestiary
{
    [CreateAssetMenu(menuName = "Bestiary/Create CreatureInfo", fileName = "CreatureInfo", order = 1)]
    public class CreatureInfo : ScriptableObject, ICharacterInfo
    {
        public SerializableGuid Guid => guid;
        public string Name;
       [SerializeField] private SerializableGuid guid;
       public ICharacterData Stats=>stats;
       public Rank Rank => rank;
       [SerializeField] Rank rank;
       [SerializeField] NPCCharacterClass stats;
        public GameObject Prefab=>prefab;
        
        public FactionNames FactionID;
        public int Influence;
        [SerializeField] GameObject prefab;
        public PhysicsInfo PhysicsInfo;
        public MovementData Move;
        public List<AIStates> aiStatesToAdd;

        [HorizontalGroup("ItemSplit", 0.5f), VerticalGroup("ItemSplit/Left"), Button(ButtonSizes.Large),
         GUIColor(0.4f, 0.8f, 1)]
        public void SetID()
        {
            guid = SerializableGuid.NewGuid();
        }
        [HorizontalGroup("ItemSplit", 0.5f), VerticalGroup("ItemSplit/Right"), Button(ButtonSizes.Large),
         GUIColor(0.4f, 0.8f, 1)]
        public void CopyID()
        {
            GUIUtility.systemCopyBuffer = guid.ToHexString();
                }

    }

    public partial class BestiaryManager
    {
        public static Task SpawnNPC(SerializableGuid guid, Vector3 position, uint waveLevel)
        {
            var info = GetCreature();
            var entity = new CharacterBuilder(info.Name).
                WithModel(info.Prefab, position, Quaternion.identity, "NPC").
                WithEntityPhysics(info.PhysicsInfo, true).
                WithStats(info.Stats, guid, waveLevel, info.Name).
                WithMovement(info.Move,CreatureType.biped,false).
                WithFactionInfluence(info.FactionID, info.Influence, 1).
                WithCharacterDetection(FactionNames.Daemon).
                WithAI(info.Rank,info.FactionID,info.aiStatesToAdd).
                Build();
            RegisterNPCEnemy(waveLevel,entity);
            return Task.CompletedTask;
        }     
        public static Task SpawnNPC(SerializableGuid guid, Vector3 position, uint waveLevel, Entity wavePack)
        {
            var info = GetCreature();
            var entity = new CharacterBuilder(info.Name).
                WithModel(info.Prefab, position, Quaternion.identity, "NPC").
                WithParent(wavePack).
                WithEntityPhysics(info.PhysicsInfo, true).
                WithStats(info.Stats, guid, waveLevel, info.Name).
                WithMovement(info.Move,CreatureType.biped,false).
                WithFactionInfluence(info.FactionID, info.Influence, 1).
                Build();
            RegisterNPCEnemy(waveLevel,entity);
            return Task.CompletedTask;
        }
    }
}