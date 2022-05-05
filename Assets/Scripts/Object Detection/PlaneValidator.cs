using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneValidator : MonoBehaviour
{
    [SerializeField] GameObject detectMenu;
    [SerializeField] GameObject planeText;
    public float planeFilterSize = 10f;
    public bool enoughPlanes = false;

    private ARPlaneManager arPlaneManager;
    private List<ARPlane> arPlanes;

    private void OnEnable()
    {
        arPlanes = new List<ARPlane>();
        arPlaneManager = gameObject.GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.updated != null && args.updated.Count > 0)
        {
            arPlanes.AddRange(args.updated);
        }

        if (arPlanes.Where(plane => plane.extents.x * plane.extents.y >= planeFilterSize).Count() > 1)
        {
            enoughPlanes = true;
            detectMenu.SetActive(true);
            planeText.SetActive(false);
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
