using System.Threading.Tasks;
using Dreamers.InventorySystem.Base;
using DreamersInc.ComboSystem;
using DreamersIncStudio.FactionSystem;
using Global.Component;
using MotionSystem.Components;
using Sirenix.OdinInspector;
using Stats;
using UnityEngine;

namespace Bestiary
{
    [CreateAssetMenu(menuName = "Create PlayerInfo", fileName = "PlayerInfo", order = 0)]
    public class PlayerInfo : ScriptableObject, ICharacterInfo
    {
        public SerializableGuid  Guid => guid;
        [SerializeField] private SerializableGuid guid;
        public string Name;
        public PlayerCharacterClass Stats=>stats;
        [SerializeField] PlayerCharacterClass stats;
        
        public GameObject Prefab=>prefab;
        [SerializeField] GameObject prefab;
        public PhysicsInfo PhysicsInfo;
        public MovementData Move;

        [Header("Combat")]
        public ComboSO Combo;
        [Header("Inventory")]
        public EquipmentSave Equipment => equipment;
        [SerializeField]private EquipmentSave equipment;
        public InventorySave Inventory => inventory;
        [SerializeField]private InventorySave inventory;
            
        
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
    public static partial class BestiaryManager
    {
        public static Task SpawnPlayer(SerializableGuid guid, Vector3 position)
        {
            var info = GetPlayerInfo(guid);
            var playerEntity = new CharacterBuilder(info.name).
                WithModel(info.Prefab, position, Quaternion.identity, "Player").
                WithEntityPhysics(info.PhysicsInfo, true).
                WithStats(info.Stats,guid,2,info.Name).
                WithInventorySystem(info.Inventory, info.Equipment).
                WithMovement(info.Move,CreatureType.biped,false).
                WithCombat(info.Combo).
                WithVFX().
                WithFactionInfluence(FactionNames.Player, 100, 1).
                WithAnimation().
                WithPlayerControl().
            Build();
            RegisterPlayer(playerEntity);
            return Task.CompletedTask;
        }
    }
    public enum CreatureType { biped, quadruped, mecha, spirit, stationary, flying }
}