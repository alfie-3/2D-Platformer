using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] BoxCollider2D boxCollider;
    [Space]
    [SerializeField] float fallDelay;
    [SerializeField] float fallGrace;
    [Space]
    [SerializeField] float returnTime;
    [Space]
    [SerializeField] AudioClip fallSound;

    bool isFallen;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.enabled && isFallen == false)
        {
            if (!collision.gameObject.GetComponent<PlayerController2D>().IsGrounded())
                return;

            isFallen = true;
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        animator.Play("Rumble");
        yield return new WaitForSeconds(fallDelay);
        animator.Play("Fall");
        AudioTools.CreateTempAudioSource(transform.position, fallSound, 0.05f, Random.Range(1.05f, 1.25f));
        yield return new WaitForSeconds(fallGrace);
        boxCollider.enabled = false;
        yield return new WaitForSeconds(returnTime);
        animator.Play("Return"); boxCollider.enabled = true;
        isFallen = false;
    }
}
