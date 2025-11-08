using Sirenix.OdinInspector;
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
       public uint Level => level;
        [SerializeField] private uint level;
        public GameObject Prefab;
        
        
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