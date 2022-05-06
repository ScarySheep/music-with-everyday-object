using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMenuManager : MonoBehaviour
{
    public GameObject BG_MASK;
    public GameObject DETECT_PLACEHOLDER;
    public GameObject DETECT_BTN;
    public GameObject REDETECT_BTN;
    public GameObject NEXT_BTN;
    public GameObject CENTER_MSG;
    public GameObject CONFIRM_MSG;

    /* assuming this will only ever be called after Close()
     * (the first time this menu is opened is through the PlaneValidator,
     * which simply sets the DetectMenu object to active, thus showing its
     * initial state from the editor)
     */
    public void Open()
    {
        // we will keep these active as long as we are in obj detection mode
        Show(BG_MASK);
        Show(DETECT_PLACEHOLDER);
        // these will change after pressing "Detect"
        Show(DETECT_BTN);
        Show(CENTER_MSG);
    }

    public void Close() { SetChildren(false); }

    private void SetChildren(bool activeStatus)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(activeStatus);
        }
    }

    /* hoped to use this to prevent "next" from showing in cases when
     * nothing was detected...but it is not working
     * 
     * private bool ObjWasDetected()
    {
        return (GameObject.Find("ObjectBox") != null);
    }*/

    public void PressedDetect()
    {
        Hide(DETECT_BTN);
        Hide(CENTER_MSG);
        Show(REDETECT_BTN);
        Show(NEXT_BTN);
        Show(CONFIRM_MSG);
    }

    public void PressedRedetect()
    {
        Show(DETECT_BTN);
        Show(CENTER_MSG);
        Hide(REDETECT_BTN);
        Hide(NEXT_BTN);
        Hide(CONFIRM_MSG);
    }

    private void Show(GameObject toShow)
    {
        toShow.SetActive(true);
    }

    private void Hide(GameObject toHide)
    {
        toHide.SetActive(false);
    }
}
