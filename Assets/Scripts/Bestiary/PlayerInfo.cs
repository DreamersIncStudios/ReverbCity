using UnityEngine;

namespace Bestiary
{
    [CreateAssetMenu(menuName = "Create PlayerInfo", fileName = "PlayerInfo", order = 0)]
    public class PlayerInfo : ScriptableObject,ICharacterInfo
    {
        public SerializableGuid Guid { get; set; }
        public uint Level => level;
        [SerializeField] private uint level;
        [SerializeField] private GameObject prefab;
    }
}