using DreamersInc.CharacterControllerSys.VFX;
using Sirenix.OdinInspector;
using UnityEngine;
namespace DreamersInc.ComboSystem
{
    [CreateAssetMenu(fileName = "New VFX", menuName = "DreamersInc/VFX", order = 0)]
    public class VFXSO : ScriptableObject
    {
        public string Name;
        public SerializableGuid ID = new SerializableGuid();
        public GameObject Prefab;
        public bool CreatePoolOnLoad;
        public bool KeepPoolAlive;

        public float Forward, Up;
        public Vector3 Rot;
        [Tooltip("Time in Milliseconds")]
        public float LifeTime;
        [Range(0, 100)]
        public int ChanceToPlay;
        public void SpawnVFX(Transform characterTransform)
        {
            int prob = Mathf.RoundToInt(Random.Range(0, 99));
            if (prob < ChanceToPlay)
            {
                Vector3 forwardPos = characterTransform.forward * Forward + characterTransform.up * Up;
                VFXDatabase.PlayVFX(ID, characterTransform.position + forwardPos, characterTransform.rotation.eulerAngles + Rot, 0, LifeTime);
            }
        }
        
        
        [HorizontalGroup("ItemSplit", 0.5f), VerticalGroup("ItemSplit/Left"), Button(ButtonSizes.Large),
         GUIColor(0.4f, 0.8f, 1)]
        public void SetID()
        {
            ID = SerializableGuid.NewGuid();
        }
        [HorizontalGroup("ItemSplit", 0.5f), VerticalGroup("ItemSplit/Right"), Button(ButtonSizes.Large),
         GUIColor(0.4f, 0.8f, 1)]
        public void CopyID()
        {
            GUIUtility.systemCopyBuffer = ID.ToHexString();
        }
    }
}
