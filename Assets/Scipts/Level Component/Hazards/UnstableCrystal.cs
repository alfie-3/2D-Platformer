using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class UnstableCrystal : MonoBehaviour
    {
        Vector3 startPosition;
        [SerializeField] float sinAmount;

        [SerializeField] float knockbackForce = 5;

        public void Awake()
        {
            startPosition = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Vector2 knockbackDir = (collision.transform.position.x >= transform.position.x) ? Vector2.right : Vector2.left;
                collision.GetComponent<IDamagable>().Damage(new(0, knockbackDir * knockbackForce, false));
                GetComponent<AudioSource>().Play();
            }
        }

        private void Update()
        {
            transform.position = startPosition + new Vector3(0.0f, Mathf.Sin(Time.time) / sinAmount, 0.0f);
        }
    }
}
