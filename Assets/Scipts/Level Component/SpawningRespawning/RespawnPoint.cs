using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{



    public class RespawnPoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && GameManager.Instance.activeCheckpoint != gameObject)
            {
                Debug.Log("Checkpoint Entered");
                GameManager.Instance.activeCheckpoint = gameObject;
            }
        }

        private void OnDrawGizmos()
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.activeCheckpoint == this.gameObject)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;
            }
            else
                Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, GetComponent<CircleCollider2D>().radius);
        }
    }
}
