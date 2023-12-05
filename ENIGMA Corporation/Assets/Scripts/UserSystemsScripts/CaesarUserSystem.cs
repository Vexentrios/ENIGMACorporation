using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static AccessEnigmaScript;

public class CaesarUserSystem : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject CaesarApp;
    [SerializeField] private GameObject AtBashApp;
    [SerializeField] private GameObject AtBashInstructionPanel;
    [SerializeField] private GameObject CaesarInstructionPanel;
    [Header("-------------------------------------------------")]
    [Header("Buttons")]
    [SerializeField] private Button CloseCaesarAppButton;
    [SerializeField] private Button CloseAtBashAppButton;
    [SerializeField] private Button SolveReportButton;
    [SerializeField] private Button LogoutButton;
    [Header("-------------------------------------------------")]
    [Header("InputFields")]
    [SerializeField] private InputField Solution;
    [Header("-------------------------------------------------")]
    [Header("Texts")]
    [SerializeField] private Text ProgressValue;
    [SerializeField] private Text ResultMessage;
    [SerializeField] private Text CipheredText;
    [SerializeField] private Text AccountPassword;
    [Header("-------------------------------------------------")]
    [Header("Other")]
    [SerializeField] private Slider ProgressBar;
    [SerializeField] private Image CaesarAppIcon;
    [SerializeField] private Image AtBashAppIcon;

    public static string Level2Answer;     //Answer which need to be given
    public static string Level2EncryptedWord;     //Encrypted answer word container
    int goal = 10;
    int hideTimer;

    void Start()
    {
        AccessEnigmaScript.AmateurAccessGranted = true;

        if (AccessEnigmaScript.ProfessionalAccessGranted != true && AccessEnigmaScript.Level2Completed == true)
            AccountPassword.gameObject.SetActive(true);

        CaesarAppIcon.GetComponent<Button>().enabled = true;
        AtBashAppIcon.GetComponent<Button>().enabled = true;
        CaesarApp.SetActive(false);
        AtBashApp.SetActive(false);
        ProgressBar.value = (float)AccessEnigmaScript.decryptedMessagesLevelTwo / (float)goal;
        ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

        if (!AccessEnigmaScript.Level2Completed)
        {
            if (Level2Answer == null || Level2Answer == "")
                Caesar_AtBash_EncryptFunction();
            else
                CipheredText.text = Level2EncryptedWord.ToUpper();
        }
        else
        {
            CipheredText.text = "All Solved";
            Solution.enabled = false;
            SolveReportButton.enabled = false;
        }
    }

    public void OpenCaesarSpinner()
    {
        CaesarAppIcon.GetComponent<Button>().enabled = false;
        CaesarApp.SetActive(true);
    }

    public void CloseCaesarSpinner()
    {
        CaesarApp.SetActive(false);
        CaesarAppIcon.GetComponent<Button>().enabled = true;
    }

    public void OpenAtBASHSpinner()
    {
        AtBashAppIcon.GetComponent<Button>().enabled = false;
        AtBashApp.SetActive(true);
    }

    public void CloseAtBASHSpinner()
    {
        AtBashApp.SetActive(false);
        AtBashAppIcon.GetComponent<Button>().enabled = true;
    }

    public void ShowAtBashInstruction() =>
        AtBashInstructionPanel.SetActive(true);

    public void HideAtBashInstruction() =>
        AtBashInstructionPanel.SetActive(false);

    public void ShowCaesarInstruction() =>
        CaesarInstructionPanel.SetActive(true);

    public void HideCaesarInstruction() =>
        CaesarInstructionPanel.SetActive(false);

    public void Caesar_AtBash_EncryptFunction()
    {
        ///THIS CONTENT IS MOVED TO FINAL VERSION
        //if (PlainWords.Caesar_AtBash_Words.Count > 0)
        if (PlainWords.Caesar_AtBash_Words.Count > 0 && AccessEnigmaScript.decryptedMessagesLevelTwo != goal)
        {
            int number = Random.Range(0, PlainWords.Caesar_AtBash_Words.Count);
            Level2Answer = PlainWords.Caesar_AtBash_Words[number].ToUpper();
            PlainWords.Caesar_AtBash_Words.RemoveAt(number);
            Level2EncryptedWord = "";

            if (PlainWords.AtBashCodes > 0 && PlainWords.CaesarCodes > 0)
            {
                int choice = Random.Range(0, 2);
                if (choice == 0)
                    CaesarAlgorithm();
                else
                    AtBashAlgorithm();
            }
            else if(PlainWords.AtBashCodes == 0 && PlainWords.CaesarCodes > 0)
                CaesarAlgorithm();
            else if (PlainWords.AtBashCodes > 0 && PlainWords.CaesarCodes == 0)
                AtBashAlgorithm();
        }
        else
        {
            CipheredText.text = "All Solved";
            Solution.enabled = false;
            SolveReportButton.enabled = false;
        }
    }

    public void CaesarAlgorithm()
    {
        int key = Random.Range(1, 26);
        char letter;

        int index = 0;
        while (index <= Level2Answer.Length - 1)
        {
            letter = (char)(Level2Answer[index] + key);
            if (letter >= 91)
                Level2EncryptedWord += ((char)(letter - 26)).ToString();
            else
                Level2EncryptedWord += letter.ToString();
            index++;
        }
        PlainWords.CaesarCodes--;
        CipheredText.text = Level2EncryptedWord.ToUpper();
    }

    public void AtBashAlgorithm()
    {
        char letter;

        int index = 0;
        while (index <= Level2Answer.Length - 1)
        {
            letter = (char)(90 - (Level2Answer[index] - 65));
            Level2EncryptedWord += letter.ToString();
            index++;
        }
        PlainWords.AtBashCodes--;
        CipheredText.text = Level2EncryptedWord.ToUpper();
    }

    public void SendSolutionReport()
    {
        if (Solution.text.ToUpper() == Level2Answer)
        {
            hideTimer = 2;
            StartCoroutine(ShowResult());
            ResultMessage.color = Color.green;
            ResultMessage.text = "Correct";
            AccessEnigmaScript.decryptedMessagesLevelTwo++;
            Caesar_AtBash_EncryptFunction();
            if (AccessEnigmaScript.decryptedMessagesLevelTwo <= goal)
            {
                ProgressBar.value = (float)AccessEnigmaScript.decryptedMessagesLevelTwo / (float)goal;
                ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

                if (AccessEnigmaScript.decryptedMessagesLevelTwo == goal)
                {
                    AccessEnigmaScript.Level2Completed = true;
                    AccountPassword.gameObject.SetActive(true);
                }

                ///THIS CONTENT IS MOVED TO FINAL VERSION
                //if (AccessEnigmaScript.decryptedMessagesLevelTwo == goal)
                //{
                //    PlainWords.CaesarCodes =7;
                //    PlainWords.AtBashCodes = 3;
                //    Caesar_AtBash_EncryptFunction();
                //}
            }
        }
        else
        {
            hideTimer = 2;
            if (!ResultMessage.IsActive())
            {
                StartCoroutine(ShowResult());
                ResultMessage.color = Color.red;
                ResultMessage.text = "Wrong";
            }
        }
        ResultMessage.gameObject.SetActive(true);
        Solution.text = "";
    }

    private IEnumerator ShowResult()
    {
        while (hideTimer > 0)
        {
            yield return new WaitForSeconds(1);
            hideTimer--;
        }

        ResultMessage.gameObject.SetActive(false);
        yield break;
    }

    public void LogOutFunc() =>
        SceneManager.LoadSceneAsync("Game_AllUsers");
}
