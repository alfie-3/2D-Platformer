using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerAudioHandler : AudioHandler
{
    [SerializeField] FootFollyCollection footFollyCollection;

    public void PlayFootstep()
    {
        PlayFootFolly(footFollyCollection);
    }

    public void PlayJumpSound()
    {
        PlayJumpFolly(footFollyCollection);
    }
}
