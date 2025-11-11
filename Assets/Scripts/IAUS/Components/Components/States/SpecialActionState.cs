using Unity.Entities;

namespace IAUS.ECS.Component
{
    public struct SpecialActionState : IComponentData
    {
        public SpecialAction Action;
        public float DelayStart;
    }

    public enum SpecialAction
    {
        Spawn, Death, Sleep, Petrify, Berserk,
    }
}