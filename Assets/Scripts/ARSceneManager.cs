using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

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
        SettingSound
    }
    private State state = State.Placing;

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
                    if (demoCursor.hits.Count > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        GameObject obj = Instantiate(objectToPlace, demoCursor.transform.position, demoCursor.transform.rotation); // place at cursor pos
                        objectPlaced.Add(obj);
                        OpenMenu();
                        debugText.text = "State setting sound";
                        state = State.SettingSound; //switch to setting sound mode after placing the block
                    }
                }
                break;
            default: break;
        }
    }

    public void OpenMenu()
    {
        musicMenu.GetComponent<MusicMenu>().init();
        musicMenu.SetActive(true);
    }

    public void CloseMenu()
    {
        musicMenu.SetActive(false);
        state = State.Placing;
    }
}
