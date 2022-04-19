using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicBlock : MonoBehaviour
{
    public string soundManagerName;
    public float volumeAdjustmentAmt;
    public MusicBlockMenu MBMenu;

    private AudioSource soundPlayer;
    private bool isPlaying;
    private GameObject ARCam;
    private bool menuIsShowing;

    // Start is called before the first frame update
    void Start()
    {
        AssignSoundPlayer();
        SetInitialState();
        AssignCam();
    }

    private void AssignSoundPlayer()
    {
        soundPlayer = GetComponent<AudioSource>();
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
        HideMenu();
        ARSceneManager ARSM = GameObject.Find("SceneManager").GetComponent<ARSceneManager>();
        ARSM.OpenMenu();
    }

    // Update is called once per frame
    void Update()
    {
        // audio visualization?
        if (menuIsShowing && (ARCam != null))
        {
            /* Using a built-in Unity function, tell this game object's
             * transform (position and rotation matrix) to rotate toward
             * the camera! */
            /* TODO TODO TODO: we probably have to update this code to rotate 
             * AROUND A POINT because just changing the direction that the
             * menu is facing will probably mean that it will end up behind
             * the object sometimes...
             * OR
             * since the image detector creates a box around the object,
             * maybe we can update this virtual cube to have the width of
             * that box, and that might allow us to get a cube that covers
             * approximately the same area? *screams in math* */
             /* TODO: need to make it "look" only along the Y-axis...hmmm... */
            transform.LookAt(ARCam.transform);
        }
    }

    public void AssignSound(int soundIndex)
    {
        Pause(); // in case a different sound was assigned before and is playing
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
