using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DreamersInc.PhysicsSpawnSystem
{


    public class ComplexColliderSpawn : MonoBehaviour
    {
        protected Collider[] Testing;
        [SerializeField] private bool damageable;
        private void OnValidate()
        {
            Testing = gameObject.GetComponentsInChildren<Collider>();
        }

        private class ColliderAuthorBaker : Baker<ComplexColliderSpawn>
        {
     
            public override void Bake(ComplexColliderSpawn authoring)
            {
       
                var colliders = authoring.gameObject.GetComponentsInChildren<Collider>();
                foreach (var col in colliders)
                {
                    var entity = CreateAdditionalEntity(TransformUsageFlags.Dynamic, false, col.gameObject.name);
                    switch (col)
                    {
                        case BoxCollider box:
                            AddComponent(entity, new BoxColliderData(box, col.gameObject.transform, authoring.damageable));
                            break;
                        case CapsuleCollider capsule:
                            AddComponent(entity, new CapsuleColliderData(capsule,col.gameObject.transform, authoring.damageable));
                            break;
                        case MeshCollider mesh:
                            AddComponent(entity, new MeshColliderData(mesh, col.gameObject.transform, authoring.damageable));
                            break;
                    }
               

                }
            }
        }
    }
}
    
