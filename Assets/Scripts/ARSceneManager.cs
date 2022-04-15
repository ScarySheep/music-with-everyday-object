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
                    if (demoCursor.hits.Count > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        GameObject obj = Instantiate(objectToPlace, demoCursor.transform.position, demoCursor.transform.rotation); // place at cursor pos
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
                            if (hit.collider.gameObject.name == "ToPlace(Clone)")
                            {
                                currentGameObject = hit.collider.gameObject;
                                OpenMenu();
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

    public void OpenMenu()
    {
        musicMenu.GetComponent<MusicMenu>().init();
        musicMenu.SetActive(true);
    }

    public void CloseMenu(int soundIndex)
    {
        musicMenu.SetActive(false);
        currentGameObject.GetComponent<MusicBlock>().AssignSound(soundIndex);
        currentGameObject.GetComponent<Material>().color = Color.cyan;
        state = State.Selecting;
    }
}
