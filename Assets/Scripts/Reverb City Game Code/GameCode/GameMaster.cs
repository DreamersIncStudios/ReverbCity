using DreamersInc.ReverbCity;
using Unity.Entities;
using UnityEngine;


namespace DreamersInc.SceneManagement
{
    public class GameMaster : MonoBehaviour
    {
        class GameMasterBaker : Baker<GameMaster>
        {
            public override void Bake(GameMaster authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<WaveManager>(entity);
            }
        }
    }
}