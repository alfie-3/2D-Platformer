using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile2D : MonoBehaviour
    {
        [SerializeField] float projectileSpeed;

        [SerializeField] Rigidbody2D rb;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.GetComponent<IDamagable>() != null)
            {
                collision.collider.GetComponent<IDamagable>().Damage(new(1, Vector2.zero, false));
                Destroy(gameObject);
            }

            else
                Destroy(gameObject);
        }

        private void Start()
        {
            SetProjectileVel();
        }

        private void SetProjectileVel()
        {
            rb.velocity = transform.right * projectileSpeed;
        }
    }
}
