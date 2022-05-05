using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] sounds;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
    _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioClip GetSound(int soundIndex)
    {
        return sounds[soundIndex];
    }

    /* note: changed from playNextSound to playSound because this will also
     * be used to play the very first sound in the menu */
    public void playSound (int soundIndex)
    {
        stopSound();
        // assign audioclip to audiosource
        _audioSource.clip = GetSound(soundIndex);
        _audioSource.Play();
    }

    // separated into a separate function to be used when we close the menu, too
    public void stopSound()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
}
