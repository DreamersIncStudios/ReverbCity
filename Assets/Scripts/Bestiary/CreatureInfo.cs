using UnityEngine;

namespace Bestiary
{
    [CreateAssetMenu(menuName = "Create CreatureInfo", fileName = "CreatureInfo", order = 0)]
    public class CreatureInfo : ScriptableObject, ICharacterInfo
    {
        public SerializableGuid Guid { get; set; }
        public uint Level => level;
        [SerializeField] private uint level;
    }
}