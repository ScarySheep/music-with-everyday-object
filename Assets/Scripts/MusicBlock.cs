using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBlock : MonoBehaviour
{
    public string soundManagerName;
    public float volumeAdjustmentAmt;

    private AudioSource soundPlayer;
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

    public void AssignSound(int soundIndex)
    {
        SoundManager SM = GameObject.Find(soundManagerName).GetComponent<SoundManager>();
        // TODO: we might need to return pairs: AudioClip & bool
        // (sound to play & whether or not it loops)
        // (could alternatively have a parallel array and method)
        soundPlayer.clip = SM.GetSound(soundIndex);
        soundPlayer.loop = true;
        soundPlayer.spatialize = true;
        // TODO: should we adjust dopplerLevel?
        Play();
    }

    // TODO: should we use UnPause?? (must test)
    public void Play()
    {
        if (!isPlaying)
        {
            soundPlayer.Play();
            isPlaying = true;
        }
    }

    public void Pause()
    {
        if (isPlaying)
        {
            soundPlayer.Pause();
            isPlaying = false;
        }
    }

    // TODO: do we need this? void Restart() { }

    public void IncreaseVolume() { AdjustVolume(volumeAdjustmentAmt); }

    public void DecreaseVolume() { AdjustVolume(-1.0f * volumeAdjustmentAmt); }

    private void AdjustVolume(float adjustment)
    {
        soundPlayer.volume += adjustment;
    }

}
