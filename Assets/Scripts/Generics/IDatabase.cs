using System.Collections.Generic;
using UnityEngine;

namespace DreamersInc.Generics
{
    public interface IDatabase<T>
    where T : ScriptableObject
    {
        List<T> items { get; }
        bool isLoaded { get; }
        void ValidateDatabase();
        void LoadDatabase(bool forceLoad);
        void ClearDatabase();
        T GetItem(int itemID);
    }
}