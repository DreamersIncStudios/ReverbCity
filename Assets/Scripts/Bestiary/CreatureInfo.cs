using System.Threading.Tasks;
using Global.Component;
using MotionSystem.Components;
using Sirenix.OdinInspector;
using Stats;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bestiary
{
    [CreateAssetMenu(menuName = "Create CreatureInfo", fileName = "CreatureInfo", order = 0)]
    public class CreatureInfo : ScriptableObject, ICharacterInfo
    {
        public SerializableGuid Guid => guid;
        public string Name;
       [SerializeField] private SerializableGuid guid;
       public PlayerCharacterClass Stats=>stats;
       [SerializeField] PlayerCharacterClass stats;
        public GameObject Prefab=>prefab;
        [SerializeField] GameObject prefab;
        public PhysicsInfo PhysicsInfo;
        public MovementData Move;

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
                WithEntityPhysics(info.PhysicsInfo, true).
                WithStats(info.Stats, guid, waveLevel, info.Name).
                WithMovement(info.Move,CreatureType.biped,false).
                Build();
            RegisterNPCEnemy(waveLevel,entity);
            return Task.CompletedTask;
        }
    }
}