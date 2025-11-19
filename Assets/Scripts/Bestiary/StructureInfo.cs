using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Stats;
using UnityEngine;

namespace Bestiary
{
    [CreateAssetMenu(menuName = "Bestiary/Create StructureInfo", fileName = "StructureInfo", order = 2)]
    public class StructureInfo : ScriptableObject, ICharacterInfo
    {
        public SerializableGuid Guid => guid;
        public string Name;
        [SerializeField] private SerializableGuid guid;
        public ICharacterData Stats=>stats;
        [SerializeField] NPCCharacterClass stats;
        public GameObject Prefab=>prefab;
        [SerializeField] GameObject prefab;
        
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
        public static Task SpawnStructure(SerializableGuid guid, Vector3 position, uint waveLevel)
        {
            var info = GetCreature();
            var entity = new CharacterBuilder(info.Name).
                WithModel(info.Prefab, position, Quaternion.identity, "NPC").
                WithEntityPhysics(info.PhysicsInfo, true).
                WithStats(info.Stats, guid, waveLevel, info.Name).
                WithMovement(info.Move,CreatureType.biped,false).
                WithFactionInfluence(info.FactionID, info.Influence, 1).
                Build();
            RegisterStructure(entity);
            return Task.CompletedTask;
        }   
        public static Task SpawnStructure(SerializableGuid guid,GameObject go, Vector3 position, uint waveLevel)
        {
            var info = GetCreature();
            var entity = new CharacterBuilder(info.Name).
                WithExistingModel(go, position, Quaternion.identity, "NPC").
                WithEntityPhysics(info.PhysicsInfo, true).
                WithStats(info.Stats, guid, waveLevel, info.Name).
                WithMovement(info.Move,CreatureType.biped,false).
                WithFactionInfluence(info.FactionID, info.Influence, 1).
                Build();
            RegisterStructure(entity);
            return Task.CompletedTask;
        }
    }
}