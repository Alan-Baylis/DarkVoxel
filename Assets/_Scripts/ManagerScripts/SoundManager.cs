using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource GrassFootstep;
    public AudioSource StoneFootstep;

    public List<AudioClip> GrasFootsteps;
    public List<AudioClip> StoneFootsteps;

    [Space(20)]
    public AudioSource SwordSwing;

    private void Awake ( )
    {
        if(instance == null)
        {
            instance = this;
        }
    }
}
