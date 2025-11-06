using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DreamersInc.MovementSys
{
    //todo refactor for npcs using ECS.
    public class FallingOverride : MonoBehaviour
    {
        private static readonly int OnGround = Animator.StringToHash("OnGround");
        [SerializeField] private float fallingTime;
        private Animator animator;
        private Rigidbody rb;

        private Vector3 respawnPosition;
        private bool IsPlayer => this.gameObject.layer == LayerMask.NameToLayer("Player");

        private void Reset()
        {
            if (respawnPosition == Vector3.zero)
                respawnPosition =
                    GameObject.FindGameObjectWithTag("Respawn").transform
                        .position; // Todo change to finding closer Respawn point. 

            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            ;

            transform.position = respawnPosition;


            fallingTime = 0.0f;
            rb.useGravity = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            fallingTime = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (!animator.GetBool(OnGround))
            {
                fallingTime += Time.deltaTime;
            }
            else
            {
                fallingTime = 0.0f;
            }

            if (fallingTime >= 3.50f)
            {
                Reset();
            }
        }
    }
}
