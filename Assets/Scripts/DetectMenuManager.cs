using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMenuManager : MonoBehaviour
{
    public GameObject DETECT_BTN;
    public GameObject REDETECT_BTN;
    public GameObject NEXT_BTN;
    public GameObject CENTER_MSG;
    public GameObject CONFIRM_MSG;

    // might not work bc we won't make the top object inactive...
    public void Open() { SetChildren(true); }

    public void Close() { SetChildren(false); }

    private void SetChildren(bool activeStatus)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(activeStatus);
        }
    }

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
