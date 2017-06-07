using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotAudio : MonoBehaviour {

    public void PlayAudio()
    {
        var Audio = GetComponent<AudioSource>();
        Audio.Play();
    }
}
