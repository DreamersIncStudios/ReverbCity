using Sirenix.OdinInspector;
using Stats;
using UnityEngine;

namespace Bestiary
{
    [CreateAssetMenu(menuName = "Create StructureInfo", fileName = "StructureInfo", order = 0)]
    public class StructureInfo : ScriptableObject, ICharacterInfo
    {
        public SerializableGuid Guid => guid;
        public string Name;
        [SerializeField] private SerializableGuid guid;
        public PlayerCharacterClass Stats=>stats;
        [SerializeField] PlayerCharacterClass stats;
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
}