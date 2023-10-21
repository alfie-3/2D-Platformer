using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class LaunchPad : MonoBehaviour
    {
        [SerializeField] float propulsionForce;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
                return;

            collision.GetComponent<PlayerController2D>().AddForce(Vector2.up * propulsionForce);
        }
    }
}
