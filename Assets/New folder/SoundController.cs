using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource source;
    public AudioClip good, bad;

    public static SoundController instance;
    private void Awake()
    {
        instance = this;
    }

    public void GoodSound()
    {
        source.PlayOneShot(good);
    }


    public void BadSound()
    {
        source.PlayOneShot(bad);
    }

}
