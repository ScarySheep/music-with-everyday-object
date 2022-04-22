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

    public void playNextSound (int soundIndex)
    {
        if (_audioSource.isPlaying){
            _audioSource.Stop();
        }

        _audioSource.clip = GetSound(soundIndex);
        // assign audioclip to audiosource
        _audioSource.Play();
    }
}
