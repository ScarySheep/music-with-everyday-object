using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public bool DEBUG_ON;

    public GameObject musicMenu;
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

    // switch based on state
    void Update()
    {
        switch (state)
        {
            case State.Selecting:
                {
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        // Construct a ray from the current touch coordinates
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.collider.gameObject.name == "VirtualMusicBlock(Clone)")
                            {
                                currentGameObject = hit.collider.gameObject;
                                SetState(State.BlockInteracting);
                                OpenBlockMenu();
                            }
                        }
                        else
                        {
                            debugText.text = "hit nothing!";
                        }
                    }
                    break;
                }
            default: break;
        }
    }

    // opens a block's local "Mickey Mouse" menu
    public void OpenBlockMenu()
    {
        MusicBlock MB = currentGameObject.GetComponent<MusicBlock>();
        if (MB != null)
        {
            MB.ShowMenu();
        }
    }

    // plays or pauses all music blocks
    private void ToggleMusicBlocks(bool shouldPlay)
    {
        foreach (GameObject block in virtualBlocks)
        {
            if (shouldPlay) block.GetComponent<MusicBlock>().Play();
            else block.GetComponent<MusicBlock>().Pause();
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
    public void OpenMenu()
    {
        SetState(State.SettingSound);
        musicMenu.GetComponent<MusicMenu>().init();
        ToggleMusicBlocks(false);
        Show(musicMenu);
    }

    // closes the sound library
    public void CloseMenu(int soundIndex)
    {
        Hide(musicMenu);
        currentGameObject.GetComponent<MusicBlock>().AssignSound(soundIndex);
        ToggleMusicBlocks(true);
        Hide(homeModeBTN);
        Show(detectModeBTN);
        SetState(State.Selecting);
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

    // called (indirectly) by the "Home" button
    private void QuitDetectionMode()
    {
        ToggleMusicBlocks(true);
        Show(detectModeBTN);
        Hide(homeModeBTN);
        SetState(State.Selecting);
        // detection UI updates handled in DetectMenuManager
    }

    // called by the "Home" button
    public void GoHome()
    {
        // when called from the Detection UI
        if (state == State.Detecting)
        {
            QuitDetectionMode();
        }
        // TODO: need nav from library browse mode, recording/uploading modes
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
