using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

public class ARSceneManager : MonoBehaviour
{
    public GameObject musicMenu;
    public GameObject objectToPlace;
    public GameObject ARCursor;
    public Text debugText;

    private DemoCursor demoCursor;
    private List<GameObject> objectPlaced = new List<GameObject>();
    private enum State
    {
        Placing,
        Selecting,
        SettingSound
    }
    private State state = State.Placing;
    private GameObject currentGameObject;

    void Start()
    {
        debugText.text = "State placing";
        demoCursor = ARCursor.GetComponent<DemoCursor>();
    }

    void Update()
    {
        switch (state)
        {
            case State.Placing:
                {
                    //if point is on a plane and there is a on touch down
                    if (demoCursor.hits.Count > 0 && Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        Vector3 adjustment = new Vector3(0.0f, 0.025f, 0.0f);
                        Vector3 pos = demoCursor.transform.position + adjustment;
                        GameObject obj = Instantiate(objectToPlace, pos, demoCursor.transform.rotation); // place at cursor pos
                        objectPlaced.Add(obj);
                    }
                    break;
                }

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
                                debugText.text = "State setting sound";
                                state = State.SettingSound; //switch to setting sound mode after placing the block
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

    public void SwitchMode()
    {
        if (state != State.SettingSound)
        {
            if (state == State.Placing)
            {
                debugText.text = "selecting!";
                state = State.Selecting;
            }
            else if (state == State.Selecting)
            {
                debugText.text = "placing!";
                state = State.Placing;
            }
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
        musicMenu.GetComponent<MusicMenu>().init();
        musicMenu.SetActive(true);
    }

    public void CloseMenu(int soundIndex)
    {
        musicMenu.SetActive(false);
        currentGameObject.GetComponent<MusicBlock>().AssignSound(soundIndex);
        //currentGameObject.GetComponent<Material>().color = Color.cyan;
        state = State.Selecting;
        debugText.text = "selecting!";
    }
}
