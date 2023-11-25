using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PlainWords;

public class MainMenuScript : MonoBehaviour
{ 
    [Header("Canvas Panels")]
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject GamePanel;
    [SerializeField] private GameObject SettingsPanel;

    [Header("------------------------------------------------------")]

    [Header("Buttons")]
    [SerializeField] private Button NewGameButton;
    [SerializeField] private Button StartButton;
    //[SerializeField] private Button ContinueButton;
    [SerializeField] private Button OptionsButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button BackButton;

    [Header("------------------------------------------------------")]

    [Header("Introduction Text Boxes")]
    [SerializeField] private Text[] StoryText = new Text [4];

    ///#######################################
    ///Main Menu Button Functions
    ///#######################################
    void Start()
    {
        MainPanel.SetActive(true);
        GamePanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    public void NewGameIntroduction()
    {
        GamePanel.SetActive(true);
        MainPanel.SetActive(false);

        StartCoroutine(TellIntroductionStory());
    }

    public void StartGame()
    {
        PlainWords.FenceWords.AddRange(PlainWords.FenceWordDataBase);

        PlainWords.Caesar_AtBash_Words.AddRange(PlainWords.Caesar_AtBash_WordDataBase);
        PlainWords.CaesarCodes = 10;
        PlainWords.AtBashCodes = 10;

        SceneManager.LoadSceneAsync("Game_AllUsers");
    }

    public void ChangeSettings()
    {
        SettingsPanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void BackToMenu()
    {
        SettingsPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    ///#######################################
    ///Introduction Function
    ///#######################################
    private IEnumerator TellIntroductionStory()
    {
        int finishedCycles = 0;
        StoryText[finishedCycles].gameObject.SetActive(true);
        finishedCycles++;
        while (finishedCycles < 4)
        {
            yield return new WaitForSeconds(3);
            StoryText[finishedCycles].gameObject.SetActive(true);
            finishedCycles++;
        }
        StartButton.gameObject.SetActive(true);
        StopCoroutine(TellIntroductionStory());
    }
}