using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using static Bestiary.BestiaryManager;
namespace DreamersInc.ReverbCity.GameCode
{
 
    public class Speaker : MonoBehaviour
    {
       public Task Init()
        {
            var entity = new CharacterBuilder("testing")
                .Build();
            RegisterStructure(entity);
            return Task.CompletedTask;
        }
    } 
}

namespace DreamersInc.ReverbCity.GameCode.Entity
{
    public struct Speaker : IComponentData
    {

    }
}