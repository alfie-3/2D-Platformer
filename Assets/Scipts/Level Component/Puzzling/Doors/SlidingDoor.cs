using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class SlidingDoor : MonoBehaviour
    {
        Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void OpenDoor()
        {
            anim.Play("Open");
            GetComponent<AudioSource>().Play();
        }
    }
}
