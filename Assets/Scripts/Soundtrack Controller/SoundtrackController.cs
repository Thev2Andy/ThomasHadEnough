using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackController : MonoBehaviour
{
    public AudioSource SoundtrackSource;
    public AudioClip[] Soundtracks;

    // Private / Hidden variables..
    private int LastSoundtrackIndex = -1;


    private void Update()
    {
        if (!SoundtrackSource.isPlaying && Soundtracks.Length > 0)
        {
            int SoundtrackIndex = Random.Range(0, Soundtracks.Length);
            if (SoundtrackIndex == LastSoundtrackIndex) {
                SoundtrackIndex += 1;
                
                if (SoundtrackIndex >= Soundtracks.Length) {
                    SoundtrackIndex = 0;
                }
            }

            SoundtrackSource.PlayOneShot(Soundtracks[SoundtrackIndex]);
            LastSoundtrackIndex = SoundtrackIndex;
        }
    }
}
