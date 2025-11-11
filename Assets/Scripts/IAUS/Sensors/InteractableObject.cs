using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace IAUS.ECS.Component
{
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField] InteractableType Type;
        [SerializeField] Weight Weight;

        class Baker : Baker<InteractableObject>
        {
            public override void Bake(InteractableObject authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Interactable() { Type = authoring.Type, Weight = authoring.Weight });
            }
        }
    }


    public struct Interactable : IComponentData, IEquatable<Interactable>
    {
        public InteractableType Type;
        public Weight Weight;

        public bool Equals(Interactable other)
        {
            return Type == other.Type && Weight == other.Weight;
        }

        public override bool Equals(object obj)
        {
            return obj is Interactable other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, (int)Weight);
        }
    }

    public readonly partial struct InteractableAspect : IAspect
    {
        private readonly DynamicBuffer<AISenses.Resources> interactables;
        private readonly DynamicBuffer<AISenses.PlacesOfInterest> placesOfInterest;
        private readonly RefRO<LocalToWorld> transform;

        public AISenses.Resources ClosestInteractable
        {
            get
            {
                if (interactables.Length == 0) return new AISenses.Resources();
                var maxDistance = float.MaxValue;
                var temp = new AISenses.Resources();
                foreach (var interactable in interactables)
                {
                    var dist = Vector3.Distance(transform.ValueRO.Position, interactable.Target.LastKnownPosition);
                    if (!(dist < maxDistance)) continue;
                    maxDistance = dist;
                    temp = interactable;
                }

                return temp;
            }
        }

        public float ClosestInteractableDistance
        {
            get
            {
                if (interactables.Length == 0) return float.MaxValue;
                var maxDistance = float.MaxValue;
                foreach (var interactable in interactables)
                {
                    var dist = Vector3.Distance(transform.ValueRO.Position, interactable.Target.LastKnownPosition);
                    if (!(dist < maxDistance)) continue;
                    maxDistance = dist;
                }

                return maxDistance;
            }
        }
    }

    public enum Weight
    {
        Light,
        Medium,
        Heavy,
        NotLiftable,
    }

    [Flags]
    public enum InteractableType
    {
        None = 0,
        Damageable = 1,
        Pickup = 2,
        Interactable = 4,
        Explosive = 8,
    }
}