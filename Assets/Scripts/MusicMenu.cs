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

    public SoundManager _soundManager;

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
        index = index % songCount;

            musicCover.GetComponent<Image>().sprite = musicCoverSprites[index];
            musicTitle.GetComponent<Text>().text = musicTitleTexts[index];
            _soundManager.playNextSound(index);
    }

    public void SelectSong()
    {
        //assign songs
        ExitMenu();
    }

}
