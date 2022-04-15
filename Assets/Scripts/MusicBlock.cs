using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBlock : MonoBehaviour
{
    private AudioSource soundPlayer;
    public string soundManagerName;
    private bool isPlaying;

    // Start is called before the first frame update
    void Start()
    {
        soundPlayer = GetComponent<AudioSource>();
        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        // audio visualization?
    }

    void AssignSound(int soundIndex)
    {
        SoundManager SM = GameObject.Find(soundManagerName).GetComponent<SoundManager>();
        // TODO: we might need to return pairs: AudioClip & bool
        // (sound to play & whether or not it loops)
        // (could alternatively have a parallel array and method)
        soundPlayer.clip = SM.GetSound(soundIndex);
        soundPlayer.loop = true;
        Play();
    }

    void Play()
    {
        if (!isPlaying)
        {
            soundPlayer.Play();
            isPlaying = true;
        }
    }

    void Pause()
    {
        if (isPlaying)
        {
            soundPlayer.Pause();
            isPlaying = false;
        }
    }

    // TODO

    void Restart() { }

    void IncreaseVolume() { }

    void DecreaseVolume() { }

}
