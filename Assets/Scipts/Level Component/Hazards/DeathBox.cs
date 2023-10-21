using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{



    [RequireComponent(typeof(BoxCollider2D))]
    public class DeathBox : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                DamagePlayer(other);

                if (other.GetComponent<PlayerManager>().PlayerIsDead)
                    return;

                other.GetComponent<Rigidbody2D>().simulated = false;
                other.transform.position = GameManager.Instance.activeCheckpoint.transform.position;
                other.GetComponent<PlayerController2D>().ResetAllForces();
                other.GetComponent<Rigidbody2D>().simulated = true;
            }
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();

            Gizmos.color = Color.magenta;

            Vector3 colliderPos = new Vector3(transform.position.x + collider.offset.x, transform.position.y + collider.offset.y, transform.position.z);

            Gizmos.DrawWireCube(colliderPos, GetComponent<BoxCollider2D>().size);
        }

        private void DamagePlayer(Collider2D player)
        {
            IDamagable damagable = player.GetComponent<IDamagable>();
            if (damagable != null)
                damagable.Damage(new(1, Vector2.zero, true));
        }
    }
}