using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script should live on the UI canvas used for the music block menu! */
public class MusicBlockMenu : MonoBehaviour
{
    private Camera camToLookAt;
    public GameObject volumeSubmenu;
    public GameObject playBTN;
    public GameObject pauseBTN;

    private bool VC_SHOWING;

    // Update is called once per frame
    void Update() { }

    void Start()
    {
        VC_SHOWING = false;
    }

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
        SetVisibility(this.gameObject, true);
        HideVolumeControls();
    }

    public void Hide()
    {
        SetVisibility(this.gameObject, false);
    }

    private void SetVisibility(GameObject obj, bool visible)
    {
        obj.SetActive(visible);
    }

    public void ToggleVolumeControls()
    {
        if (VC_SHOWING)
        {
            HideVolumeControls();
        }
        else
        {
            ShowVolumeControls();
        }
    }

    private void ShowVolumeControls()
    {
        SetVisibility(volumeSubmenu, true);
        VC_SHOWING = true;
    }

    private void HideVolumeControls()
    {
        SetVisibility(volumeSubmenu, false);
        VC_SHOWING = false;
    }

    public void ShowPlayButton()
    {
        SetVisibility(pauseBTN, false);
        SetVisibility(playBTN, true);
    }

    public void ShowPauseButton()
    {
        SetVisibility(playBTN, false);
        SetVisibility(pauseBTN, true);
    }
}
