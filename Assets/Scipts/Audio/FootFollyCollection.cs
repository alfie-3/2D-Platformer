using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collection", menuName = "Audio/Folly Collection", order = 1), System.Serializable]
public class FootFollyCollection : ScriptableObject
{
    public AudioClip[] walkFolly;
    public AudioClip[] jumpFolly;
}
