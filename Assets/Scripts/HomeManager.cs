using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public bool DEBUG_ON;
    public string TO_PLACE_NAME;
    
    public GameObject soundLibMenu;
    public GameObject beatMenu;
    public GameObject vfxUI;
    public ObjectDetector objDetector;
    public GameObject homeModeBTN;
    public GameObject detectModeBTN;

    public GameObject objectToPlace;
    public Text debugText;

    private List<GameObject> virtualBlocks = new List<GameObject>();

    private enum State
    {
        Detecting,
        Selecting,
        BlockInteracting,
        SettingSound
    }
    private State state;
    private GameObject currentGameObject;

    private void SetState(State newState)
    {
        state = newState;
        debugText.text = "State: " + state.ToString();
    }

    void Start()
    {
        SetState(State.Detecting);
        if (DEBUG_ON) Show(debugText.gameObject);
    }

    // plays or pauses all music blocks
    private void ToggleMusicBlocks(bool shouldPlay)
    {
        foreach (GameObject block in virtualBlocks)
        {
            if (shouldPlay) block.GetComponent<MusicBlock>().TurnOn();
            else block.GetComponent<MusicBlock>().TurnOff();
        }
    }

    /* called by the "Next" button on the Detection UI
     * places a virtual music block at the location selected through obj detection
     */
    public void PlaceVirtualBlock()
    {
        Vector3 position = objDetector.GetToPlacePos();
        GameObject newVirtualBlock = Instantiate(objectToPlace, position, Quaternion.identity);
        currentGameObject = newVirtualBlock;
        virtualBlocks.Add(newVirtualBlock);
    }

    /* called by the "Next" button on the Detection UI
     * opens the sound library
     * 
     * also called by a song change button on the block's interface
     */
    public void OpenSoundLib()
    {
        SetState(State.SettingSound);
        ToggleMusicBlocks(false);
        Show(soundLibMenu);
        Hide(detectModeBTN);
        Show(homeModeBTN);
    }

    public void CloseSoundLib()
    {
        Hide(soundLibMenu);
        Show(detectModeBTN);
        Hide(homeModeBTN);
        SetState(State.Selecting);
        ToggleMusicBlocks(true);
    }

    public void OpenBeatMenu()
    {
        beatMenu.GetComponent<MusicMenu>().init();
        Hide(soundLibMenu);
        Show(beatMenu);
        Show(vfxUI);
    }

    public void BeatMenuBack()
    {
        Hide(beatMenu);
        Hide(vfxUI);
        Show(soundLibMenu);
    }

    // to be called after successfully selecting a beat
    public void CloseBeatMenu(int soundIndex)
    {
        Hide(beatMenu);
        Hide(vfxUI);
        ToggleMusicBlocks(true);
        Hide(homeModeBTN);
        Show(detectModeBTN);
        SetState(State.Selecting);
        currentGameObject.GetComponent<MusicBlock>().AssignSound(soundIndex);
    }

    // called by the "GoDetectBTN"
    public void EnterDetectionMode()
    {
        ToggleMusicBlocks(false);
        Hide(detectModeBTN);
        Show(homeModeBTN);
        SetState(State.Detecting);
        // detection UI updates handled in DetectMenuManager
    }

    private void BackToSelectMode()
    {
        ToggleMusicBlocks(true);
        Show(detectModeBTN);
        Hide(homeModeBTN);
        SetState(State.Selecting);
    }

    // called (indirectly) by the "Home" button
    private void QuitDetectionMode()
    {
        BackToSelectMode();
        // detection UI updates handled in DetectMenuManager
    }

    private void QuitSettingSound()
    {
        Hide(beatMenu);
        Hide(vfxUI);
        Hide(soundLibMenu);
        GameObject soundLib = GameObject.Find("SoundManager");
        soundLib.GetComponent<SoundManager>().stopSound();
        BackToSelectMode();
    }

    // called by the "Home" button
    public void GoHome()
    {
        switch(state)
        {
            // when called from the Detection UI
            case State.Detecting:
                {
                    QuitDetectionMode();
                    break;
                }
            // when called from sound library browsing
            case State.SettingSound:
                {
                    QuitSettingSound();
                    break;
                }
            default: break;
        }
        // TODO: need nav to and from recording/uploading modes
    }

    /* for more readable code */

    private void Show(GameObject toShow)
    {
        toShow.SetActive(true);
    }

    private void Hide(GameObject toHide)
    {
        toHide.SetActive(false);
    }
}
