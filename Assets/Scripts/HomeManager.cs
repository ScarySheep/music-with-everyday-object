using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

public class HomeManager : MonoBehaviour
{
    private readonly bool DEBUG_ON = false;

    public GameObject musicMenu;
    public ObjectDetector objDetector;
    public GameObject objectToPlace;
    public Text debugText;

    private List<GameObject> virtualBlocks = new List<GameObject>();

    private enum State
    {
        Placing,
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
        SetState(State.Placing);
        if (DEBUG_ON) debugText.gameObject.SetActive(true);
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
                                OpenBlockMenu();
                                SetState(State.BlockInteracting);
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

    public void OpenBlockMenu()
    {
        MusicBlock MB = currentGameObject.GetComponent<MusicBlock>();
        if (MB != null)
        {
            MB.ShowMenu();
        }
    }

    /* TODO (in progress): changing the flow so that we open the MusicBlockMenu
     * (the block's control panel), then from there, if a certain button is
     * pressed, we open up this music selection menu (can probably do this with
     * some broadcast-y magic) */
    public void OpenMenu()
    {
        SetState(State.SettingSound);
        musicMenu.GetComponent<MusicMenu>().init();
        musicMenu.SetActive(true);
        ToggleMusicBlocks(false);
    }

    public void CloseMenu(int soundIndex)
    {
        musicMenu.SetActive(false);
        currentGameObject.GetComponent<MusicBlock>().AssignSound(soundIndex);
        ToggleMusicBlocks(true);
        SetState(State.Selecting);
    }

    private void ToggleMusicBlocks(bool shouldPlay)
    {
        foreach (GameObject block in virtualBlocks)
        {
            if (shouldPlay) block.GetComponent<MusicBlock>().Play();
            else block.GetComponent<MusicBlock>().Pause();
        }
    }

    public void PlaceVirtualBlock()
    {
        Vector3 position = objDetector.GetToPlacePos();
        GameObject newVirtualBlock = Instantiate(objectToPlace, position, Quaternion.identity);
        currentGameObject = newVirtualBlock;
        virtualBlocks.Add(newVirtualBlock);
    }
}
