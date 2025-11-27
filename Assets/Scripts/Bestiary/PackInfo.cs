using System.Threading.Tasks;
using DreamersIncStudio.GAIACollective;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace Bestiary
{
    public class PackInfo : ScriptableObject
    {
        public string Name;
        public SerializableGuid Guid => guid;
        [SerializeField] private SerializableGuid guid;
        
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
        public static Task SpawnPack(SerializableGuid guid, Vector3 position)
        {
            var pack = new PackBuilder(guid.ToHexString()).Build();
            return Task.CompletedTask;
        }
        
        public class PackBuilder
        {
            private readonly Entity entity;
            private EntityManager manager;
            public PackBuilder(string entityName)
            {
                manager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var baseEntityArch = manager.CreateArchetype(
                    typeof(LocalTransform),
                    typeof(LocalToWorld),
                    typeof(Pack)
                );
                entity = manager.CreateEntity(baseEntityArch);
                manager.SetName(entity, entityName != string.Empty ? entityName : "NPC Data");
                manager.SetComponentData(entity, new LocalTransform()
                {
                    Scale = 1
                });
            }
            
            
            public Entity Build()
            {
                return entity;
            }
            
        }
    }
}