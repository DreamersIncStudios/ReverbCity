using UnityEngine;

namespace Bestiary
{
    [CreateAssetMenu(menuName = "Create StructureInfo", fileName = "StructureInfo", order = 0)]
    public class StructureInfo : ScriptableObject, ICharacterInfo
    {
        public SerializableGuid Guid { get; set; }
        public uint Level => level;
        [SerializeField] private uint level;
    }
}