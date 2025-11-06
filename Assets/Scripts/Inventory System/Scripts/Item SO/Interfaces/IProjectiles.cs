using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamers.InventorySystem
{
    public interface IProjectile
    {
        int RoundsPerMin { get; }
        int RoundsPerShot { get; }
        float NormalSpeed { get; }
       float ShootLocationOffset { get; }
    }
    
    public interface IProjectileWeapon
    {
        public void EquipSpell(SpellSO spell);

    }
}