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

    private HomeManager HOME_SCENE_MANAGER;
    private int songCount;
    private int index = 0;

    void Start()
    {
        songCount = musicCoverSprites.Count();
        HOME_SCENE_MANAGER = sceneManager.GetComponent<HomeManager>();
    }

    public void ExitMenu()
    {
        HOME_SCENE_MANAGER.CloseMenu(index);
        _soundManager.stopSound();
    }

    public void init()
    {
        index = 0;
        musicCover.GetComponent<Image>().sprite = musicCoverSprites[index];
        musicTitle.GetComponent<Text>().text = musicTitleTexts[index];
        // adding this call so that the first sound will play, too
        _soundManager.playSound(index);
    }

    public void NextSong()
    {
        index++;
        index = index % songCount;
        musicCover.GetComponent<Image>().sprite = musicCoverSprites[index];
        musicTitle.GetComponent<Text>().text = musicTitleTexts[index];
        _soundManager.playSound(index);
    }

    public void SelectSong()
    {
        //assign songs
        ExitMenu();
    }

}
