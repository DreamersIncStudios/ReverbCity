using UnityEngine;
using Dreamers.InventorySystem;

namespace Dreamers.InventorySystem.Interfaces
{
    public interface IPurchasable
    {
        public uint Value { get; }
        public uint MaxStackCount { get; }

        //TODO COnsider adding can cell?
    }
}