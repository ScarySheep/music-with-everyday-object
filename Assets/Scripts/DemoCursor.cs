using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

/* CREDIT: from the tutorial "Get Started with AR in Unity in 6 minutes!" */

public class DemoCursor : MonoBehaviour
{
    public GameObject cursorChildObject;
    public GameObject objectToPlace;
    public GameObject musicMenu;
    private Canvas musicMenuCanvas;
    public ARRaycastManager rayCastManager;
    public Text debugText;
    private List<GameObject> objectPlaced = new List<GameObject>();

    public bool useCursor = true;
    private enum State
    {
        Placing,
        SettingSound
    }
    private State state = State.Placing;
    // Start is called before the first frame update
    void Start()
    {
        cursorChildObject.SetActive(useCursor);
        debugText.text = "State placing";
        musicMenuCanvas = musicMenu.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {

        if (useCursor)
        {
            UpdateCursor();
        }
        switch (state)
        {
            case State.Placing:
                {
                    debugText.text = "State placing";
                    List<ARRaycastHit> hits = new List<ARRaycastHit>();
                    rayCastManager.Raycast(Input.GetTouch(0).position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
                    if (hits.Count > 0)
                    {
                        //Instantiate(objectToPlace, hits[0].pose.position, hits[0].pose.rotation); // place at tapped pos
                        GameObject obj = Instantiate(objectToPlace, transform.position, transform.rotation); // place at cursor pos
                        objectPlaced.Add(obj);
                        musicMenuCanvas.enabled = true;
                        state = State.SettingSound; //switch to setting sound mode after placing the block
                        debugText.text = "State setting sound";
                    }

                    break;
                }
            default: break;
        }
    }

    void UpdateCursor()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        rayCastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
        }
    }

    public void CloseMenu()
    {
        musicMenuCanvas.enabled = false;
        state = State.Placing;
    }
}
