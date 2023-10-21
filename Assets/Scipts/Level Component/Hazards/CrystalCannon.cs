using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class CrystalCannon : MonoBehaviour, IActivatable
    {
        [SerializeField] GameObject projectilePrefab;
        [Space]
        [SerializeField] AudioClip chargeSfx;
        [SerializeField] AudioClip fireSfx;
        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Charge()
        {
            //audioSource.PlayOneShot(chargeSfx);
            GetComponent<Animator>().Play("Charge");
        }

        public void Fire()
        {
            audioSource.PlayOneShot(fireSfx);
            Instantiate(projectilePrefab, transform.position + (transform.right * 1.5f), transform.rotation);
        }

        public void Activate() => Charge();
    }
}
