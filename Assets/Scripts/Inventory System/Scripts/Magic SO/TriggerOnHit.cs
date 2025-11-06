using System;
using UnityEngine;

namespace DreamersInc.MagicSystem
{
    public class TriggerOnHit: MonoBehaviour
    {
        private GameObject vfxPrefab;
        private Vector3 spawnPos;
        private float delay;
        public void Init(GameObject vfxPrefab, Vector3 spawn, float delay)
        {
          this.vfxPrefab = vfxPrefab;
          this.spawnPos = spawn;
          this.delay = delay;
        }

        private void Update()
        {    
            if (delay > 0)
                delay -= Time.deltaTime;
            else
            {
                Trigger();
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
          Trigger();
        }

        void Trigger()
        {
            Debug.Log("Trigger");
            var spawn  = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            Destroy(spawn, 5f);
            
        }
    }
}