using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioHandler))]
public class AudioHandler : MonoBehaviour
{
    AudioSource audioSource;

    FootFollyCollection footStepCollection;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpFolly(FootFollyCollection footFollyCollection)
    {
        audioSource.pitch = 1;
        audioSource.volume = 0.2f;
        audioSource.clip = footFollyCollection.jumpFolly[Random.Range(0, footFollyCollection.jumpFolly.Length)];
        audioSource.Play();
    }

    public void PlayFootFolly(FootFollyCollection footFollyCollection)
    {
        audioSource.pitch = 1.27f;
        audioSource.volume = 0.15f;
        audioSource.clip = footFollyCollection.walkFolly[Random.Range(0, footFollyCollection.walkFolly.Length)];
        audioSource.Play();
    }
}
