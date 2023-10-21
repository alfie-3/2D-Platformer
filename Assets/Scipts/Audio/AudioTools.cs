using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class AudioTools : MonoBehaviour
    {
        public static void CreateTempAudioSource(Vector3 position, AudioClip clip, float volume = 1, float pitch = 1)
        {
            GameObject tempGO = new GameObject(("TempAudio"));
            tempGO.transform.position = position;
            AudioSource tempAudiouSource = tempGO.AddComponent<AudioSource>();
            tempAudiouSource.clip = clip;
            tempAudiouSource.volume = volume;
            tempAudiouSource.pitch = pitch;

            tempAudiouSource.Play();
            Destroy(tempAudiouSource.gameObject, tempAudiouSource.clip.length);
        }
    }
