using System.Threading.Tasks;
using Bestiary;
using Stats;
using Unity.Entities;
using UnityEngine;
using static Bestiary.BestiaryManager;
namespace DreamersInc.ReverbCity.GameCode
{
 
    public class Speaker : MonoBehaviour
    {
        [SerializeField]private StructureInfo structureInfo;
       public Task Init()
       {
            return SpawnStructure(structureInfo, gameObject, transform.position, 1);
        }
    } 
}

namespace DreamersInc.ReverbCity.GameCode.Entity
{
    public struct Speaker : IComponentData
    {

    }
}