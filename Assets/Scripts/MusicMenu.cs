using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicMenu : MonoBehaviour
{
    public Sprite[] musicCoverSprites;
    public String[] musicTitleTexts;
    public Image musicCover;
    public Text musicTitle;
    public GameObject sceneManager;

    private ARSceneManager arSceneManager;
    private int songCount;
    private int index = 0;

    void Start()
    {
        songCount = musicCoverSprites.Count();
        arSceneManager = sceneManager.GetComponent<ARSceneManager>();
    }

    public void ExitMenu()
    {
        arSceneManager.CloseMenu(index);
    }

    public void init()
    {
        index = 0;
        musicCover.GetComponent<Image>().sprite = musicCoverSprites[index];
        musicTitle.GetComponent<Text>().text = musicTitleTexts[index];
    }

    public void NextSong()
    {
        index++;
        if (index == songCount)
        {
            ExitMenu();
        }
        else
        {
            musicCover.GetComponent<Image>().sprite = musicCoverSprites[index];
            musicTitle.GetComponent<Text>().text = musicTitleTexts[index];
        }
    }

    public void SelectSong()
    {
        //assign songs
        ExitMenu();
    }

}
