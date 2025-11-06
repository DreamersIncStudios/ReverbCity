using System;
using Dreamers.InventorySystem;
using Stats;
using Stats.Entities;
using Unity.Entities;
using UnityEngine;

namespace Dreamers.InventorySystem.Interfaces
{
    public interface ISpells
    {
        public uint Level { get; }
        public School School { get; }
        public float Duration { get; }
        public uint Range { get; }
        public uint AreaOfEffect { get; }
        public DamageType DamageType { get; }

        public uint ManaCost { get; }

    }

    public enum SpellType
    {
        Passive,
        CastByUser,
        CastByWeapon,
        Reactive
    }

    public enum School
    {
        Abjuration=1,//: Focuses on protective magic, warding off harm, and banishing creatures.
        Conjuration=3,//: Deals with summoning creatures and objects, or moving things from one place to another.
        Divination=5,//: Grants insights into the future or uncovering hidden knowledge.
        Enchantment=7,//: Affects the minds of others, influencing or controlling their behavior.
        Evocation=9,//: Creates powerful elemental effects from nothing.
        Illusion=11,//: Deceives the senses or minds of others.
        Necromancy=13,//: Focuses on manipulating life energy and death.
        Transmutation=15,//: Transforms things into other things.
    }

    public enum DamageType
    {
        Acid = 1,
        Bludgeoning = 3,
        Cold =5,
        Fire =7,
        Force= 9,
        Lightning = 11,
        Necrotic =13,
        Piercing= 15,
        Poison=17,
        Psychic=19,
        Radiant=21,
        Slashing=23,
        Thunder=25,
    }

}