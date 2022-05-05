using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/* CREDIT: from the tutorial "Get Started with AR in Unity in 6 minutes!" */

public class DemoCursor : MonoBehaviour
{
    public GameObject cursorChildObject;
    public ARRaycastManager rayCastManager;
    public List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public bool useCursor = true;

    void Start()
    {
        cursorChildObject.SetActive(useCursor);
    }

    void Update()
    {
        if (useCursor)
        {
            UpdateCursor();
        }
    }

    void UpdateCursor()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        hits = new List<ARRaycastHit>();
        rayCastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
        }
    }
}
