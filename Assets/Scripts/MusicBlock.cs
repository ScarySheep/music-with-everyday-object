using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBlock : MonoBehaviour
{
    // TODO: should we make other game objects' names public in case they change?
    // (specifically SceneManager and AR Camera)

    public string soundManagerName;
    private SoundManager SOUND_MANAGER;
    public float volumeAdjustmentAmt;
    public MusicBlockMenu MBMenu;

    private AudioSource soundPlayer;
    private bool isPlaying;
    private GameObject ARCam;
    private bool menuIsShowing;

    private HomeManager HOME_SCENE_MANAGER;

    // Start is called before the first frame update
    void Start()
    {
        AssignManagers();
        AssignSoundPlayer();
        SetInitialState();
        AssignCam();
    }

    private void AssignManagers()
    {
        HOME_SCENE_MANAGER = GameObject.Find("SceneManager").GetComponent<HomeManager>();
        SOUND_MANAGER = GameObject.Find(soundManagerName).GetComponent<SoundManager>();
    }

    private void AssignSoundPlayer()
    {
        soundPlayer = GetComponent<AudioSource>();
        soundPlayer.loop = true;
        soundPlayer.spatialize = true;
        // TODO: should we adjust dopplerLevel?
    }

    private void SetInitialState()
    {
        isPlaying = false;
        menuIsShowing = false;
    }

    private void AssignCam()
    {
        // First, find the camera!
        /* TODO: make sure there will only be one of these in the scene, and
         * that this will always be its name! */
        ARCam = GameObject.Find("AR Camera");
        Camera cam = ARCam.GetComponent<Camera>();
        MBMenu.GiveCamToCanvas(cam);
    }

    /* TODO: is there a better way to connect these / can we use send message
     * upwards? not sure if our blocks are instantiated as children
     * of the scene manager, but I think that might be a good idea */
    public void MB_OpenSoundMenu()
    {
        HideMenu(); // hiding THIS local menu
        HOME_SCENE_MANAGER.OpenMenu(); // opening the sound library menu
    }

    // Update is called once per frame
    void Update()
    {
        if (menuIsShowing && (ARCam != null))
        {
            /* Using a built-in Unity function, tell this game object's
             * transform (position and rotation matrix) to rotate toward
             * the camera! */
            transform.LookAt(ARCam.transform);
        }
    }

    public void AssignSound(int soundIndex)
    {
        // exit if -1 index (from "Back" button)
        if (soundIndex < 0) return;
        // in case a different sound was assigned before and is playing
        Pause();
        // then update the sound field
        soundPlayer.clip = SOUND_MANAGER.GetSound(soundIndex);
        // and play the new clip
        Play();
    }

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

    /*public void Restart()
    {
        if (isPlaying)
        {
            soundPlayer.Stop();
            isPlaying = false;
            Play();
        }
    }*/

    public void IncreaseVolume() { AdjustVolume(volumeAdjustmentAmt); }

    public void DecreaseVolume() { AdjustVolume(-1.0f * volumeAdjustmentAmt); }

    private void AdjustVolume(float adjustment)
    {
        soundPlayer.volume += adjustment;
    }

    public void ShowMenu()
    {
        MBMenu.Show();
        menuIsShowing = true;
    }

    public void HideMenu()
    {
        MBMenu.Hide();
        menuIsShowing = false;
    }

}
