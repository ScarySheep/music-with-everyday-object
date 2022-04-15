using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Onboarder : MonoBehaviour
{
    /* display related */
    // the onboarding screens
    public Sprite [] images;
    // the game object where the above screens will be displayed
    public Image displayed;
    // the buttons on the welcome screen
    public Button[] welcomeBTNs;
    // the back and next buttons for the middle screens
    public Button backBTN;
    public Button nextBTN;
    // the final button on the last screen
    public Button finalBTN;
    // the screen being displayed
    private int displayingIndex;
    // the final screen's index
    private int finalIndex;

    /* other */
    // name of the scene to come after onboarding
    public string NextSceneName;

    // Start is called before the first frame update
    void Start()
    {
        displayingIndex = 0;
        finalIndex = images.Length - 1;
        InitBTNs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitBTNs()
    {
        SetWelcomeBTNs(true);
        SetBTN(backBTN, false);
        SetBTN(nextBTN, false);
        SetBTN(finalBTN, false);
    }

    public void SkipOnboarding()
    {
        ToNextScene();
    }

    public void LetsOnboard()
    {
        bool success = ShowNextImage();
        if (!success) { ToNextScene(); }
        else {
            SetWelcomeBTNs(false);
            SetBTN(backBTN, true);
            SetBTN(nextBTN, true);
        }
    }

    public void Next()
    {
        bool success = ShowNextImage();
        if (!success) { ToNextScene(); }
        else if (ShowingLastScreen())
        {
            SetBTN(nextBTN, false);
            SetBTN(finalBTN, true);
        }
    }

    public void Back()
    {
        bool wasShowingLast = ShowingLastScreen();
        bool success = ShowPreviousImage();
        if (success)
        {
            if (wasShowingLast) { SetBTN(finalBTN, false); }
            if (ShowingWelcomeScreen()) { InitBTNs(); }
            else
            {
                SetBTN(backBTN, true);
                SetBTN(nextBTN, true);
            }
        }
    }

    public void LetsGo()
    {
        ToNextScene();
    }

    private void SetWelcomeBTNs(bool activeStatus)
    {
        foreach(Button btn in welcomeBTNs)
        {
            SetBTN(btn, activeStatus);
        }
    }

    private void SetBTN(Button btn, bool activeStatus)
    {
        btn.gameObject.SetActive(activeStatus);
    }

    private bool ShowNextImage()
    {
        int next = displayingIndex + 1;
        if (next <= finalIndex)
        {
            displayingIndex++;
            displayed.sprite = images[displayingIndex];
            return true;
        }
        return false;
    }

    private bool ShowPreviousImage()
    {
        int previous = displayingIndex - 1;
        if (previous >= 0)
        {
            displayingIndex--;
            displayed.sprite = images[displayingIndex];
            return true;
        }
        return false;
    }

    private void ToNextScene()
    {
        if (ValidSceneName())
        {
            SceneManager.LoadScene(NextSceneName);
        }
        else
        {
            Debug.Log("Scene transition error in Onboarder!");
        }
    }

    private bool ValidSceneName()
    {
        return !(NextSceneName.Equals(""));
    }

    private bool ShowingWelcomeScreen()
    {
        return (displayingIndex == 0);
    }

    private bool ShowingLastScreen()
    {
        return (displayingIndex == finalIndex);
    }
}
