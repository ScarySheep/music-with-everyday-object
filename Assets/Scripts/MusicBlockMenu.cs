using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should live on the UI canvas used for the music block menu! */
public class MusicBlockMenu : MonoBehaviour
{
    private Camera camToLookAt;

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    // Update is called once per frame
    void Update() { }

    public void GiveCamToCanvas(Camera cam)
    {
        /* Even though this script lives on the Canvas, it's actually attached
         * to the GAME OBJECT (imagine this as a sort of box), so we still
         * have to use GetComponent to actually access the Canvas. */
        Canvas UICanvas = GetComponent<Canvas>();
        UICanvas.worldCamera = cam;
        // Now the Canvas can render correctly and respond to UI events.
    }

    public void Show()
    {
        SetVisibility(true);
    }

    public void Hide()
    {
        SetVisibility(false);
    }

    private void SetVisibility(bool visible)
    {
        this.gameObject.SetActive(visible);
        /* TODO: do we also need to set the child components, or does changing
         * the parent's status automatically change all the children? */
    }
}
