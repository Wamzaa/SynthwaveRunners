using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource source;

    private void Start()
    {
        source.PlayOneShot(clip, 1.0f);
    }
}
