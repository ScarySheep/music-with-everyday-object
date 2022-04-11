using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/* CREDIT: from the tutorial "Get Started with AR in Unity in 6 minutes!" */

public class DemoCursor : MonoBehaviour
{
    public GameObject cursorChildObject;
    public GameObject objectToPlace;
    public ARRaycastManager rayCastManager;

   public bool useCursor = true;

    // Start is called before the first frame update
    void Start()
    {
        cursorChildObject.SetActive(useCursor);
        //objectToPlace.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (useCursor)
        {
            UpdateCursor();
        }
        //else
        //{
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            rayCastManager.Raycast(Input.GetTouch(0).position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
            if (hits.Count > 0)
            {
                Instantiate(objectToPlace, hits[0].pose.position, hits[0].pose.rotation);
            }
       //}
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
}
