using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DreamersInc.PhysicsSpawnSystem
{
    
    internal class ColliderGameObjectData : IComponentData
    {
        public UnityObjectRef<GameObject> ColliderGO;

    }
    internal struct BoxColliderData : IComponentData
    {
        public readonly float3 Center;
        public readonly float3 Size;
        public readonly float3 Scale;
        public readonly float3 Position;
        public readonly Quaternion Rotation;

        public BoxColliderData(BoxCollider col, Transform gameObjectTransform, bool damageable = false)
        {
            Center = col.center;
            Size = col.size;
            Position=gameObjectTransform.position;
            Rotation = gameObjectTransform.rotation;
            Scale = gameObjectTransform.lossyScale;
        }
    }

    internal struct MeshColliderData : IComponentData
    {
        public UnityObjectRef<Mesh> mesh;
        public readonly float3 Position;
        public readonly float3 Scale;
        public readonly Quaternion Rotation;

        public MeshColliderData(MeshCollider meshCollider, Transform gameObjectTransform, bool damageable = false)
        {
            mesh = meshCollider.sharedMesh;
            Scale = gameObjectTransform.lossyScale;

            Position=gameObjectTransform.position;
            Rotation = gameObjectTransform.rotation;
        }
    }

    public struct SphereColliderData : IComponentData
    {
        public float3 Center;
        public float Radius;

    }

    public struct CylinderColliderData : IComponentData
    {
        public float3 Center;
        public float Radius;
        public float Height;

    }

    public struct CapsuleColliderData : IComponentData
    {
        public readonly float3 Center;
        public readonly float Radius;
        public readonly float Height;
        public readonly float3 Scale;

        public readonly float3 Position;
        public readonly Quaternion Rotation;
        public CapsuleColliderData(CapsuleCollider col, Transform gameObjectTransform,bool damageable = false)
        {
            Center = col.center;
            Height = col.height;
            Radius = col.radius;
            Position=gameObjectTransform.position;
            Rotation = gameObjectTransform.rotation;
            Scale = gameObjectTransform.lossyScale;
        }
    }

    class ColliderCleanup : ICleanupComponentData
    {
        public Collider DestroyThisCollider;
    }
    class GameObjectCleanup : ICleanupComponentData
    {
        public GameObject DestroyThisGameObject;
    }
}