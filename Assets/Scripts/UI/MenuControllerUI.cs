using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControllerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _skinnyDevButton;
    [SerializeField] private Button _discordButton;
    [SerializeField] private RectTransform _headerImageTransform;

    private void Awake() 
    {
        _playButton.onClick.AddListener(OnPlayButtonClick);
        _settingsButton.onClick.AddListener(OnSettingsButtonClick);
        _creditsButton.onClick.AddListener(OnCreditsButtonClick);
        _quitButton.onClick.AddListener(OnQuitButtonClick);

        _skinnyDevButton.onClick.AddListener(() => OpenLinks("https://www.youtube.com/@skinnydev"));
        _discordButton.onClick.AddListener(() => OpenLinks("https://discord.gg/Scf6zKFtpj"));
    }
    private void OnPlayButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnSettingsButtonClick()
    {
        //TODO: Settings Menu
    }

    private void OnCreditsButtonClick()
    {
        //TODO: Credits Menu
    }

    private void OpenLinks(string link)
    {
        Application.OpenURL(link);
    }

    private void OnQuitButtonClick()
    {
        Application.Quit();
        Debug.Log("Quitting the Game...");
    }

}
