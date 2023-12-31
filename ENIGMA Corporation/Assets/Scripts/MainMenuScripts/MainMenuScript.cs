using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PlainWords;
using static AccessEnigmaScript;

public class MainMenuScript : MonoBehaviour
{ 
    [Header("Canvas Panels")]
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject GamePanel;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject CreditsPanel;

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
        CreditsPanel.SetActive(false);
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
        PlainWords.CaesarCodes = 4;
        PlainWords.AtBashCodes = 6;

        PlainWords.XOR_Morse_Words.AddRange(PlainWords.XOR_Morse_WordDataBase);
        PlainWords.MorseCodes = 3;
        PlainWords.XORCodes = 3;

        PlainWords.ADFVGX_PlayFair_Vigenere_Words.AddRange(PlainWords.ADFVGX_PlayFair_Vigenere_WordDataBase);
        PlainWords.ADFVGXCodes = 3;
        PlainWords.PlayFairCodes = 2;
        PlainWords.VigenereCodes = 2;

        AccessEnigmaScript.decryptedMessagesLevelOne = 0;
        AccessEnigmaScript.Level1Completed = false;
        AccessEnigmaScript.decryptedMessagesLevelTwo = 0;
        AccessEnigmaScript.Level2Completed = false;
        AccessEnigmaScript.decryptedMessagesLevelThree = 0;
        AccessEnigmaScript.Level3Completed = false;
        AccessEnigmaScript.decryptedMessagesLevelFour = 0;
        AccessEnigmaScript.Level4Completed = false;

        AccessEnigmaScript.RookieAccessGranted = false;
        AccessEnigmaScript.AmateurAccessGranted = false;
        AccessEnigmaScript.ProfessionalAccessGranted = false;
        AccessEnigmaScript.EliteAccessGranted = false;

        SceneManager.LoadSceneAsync("Game_AllUsers");
    }

    public void ChangeSettings()
    {
        SettingsPanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        CreditsPanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void BackToMenu(int panel)
    {
        if(panel == 0)
            SettingsPanel.SetActive(false);
        else if(panel == 1)
            CreditsPanel.SetActive(false);
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
