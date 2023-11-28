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

    public static string wordCaesar_AtBash;     //Answer which need to be given
    public static string caesar_atbashWord;     //Encrypted answer word container
    public static int decryptedMessagesLevelTwo = 0;
    int goal = 10;
    int hideTimer;

    void Start()
    {
        if (AccessEnigmaScript.AmateurAccessGranted != true)
            AccessEnigmaScript.AmateurAccessGranted = true;

        if (AccessEnigmaScript.ProfessionalAccessGranted != true && AccessEnigmaScript.Level2Completed == true)
            AccountPassword.gameObject.SetActive(true);

        CaesarAppIcon.GetComponent<Button>().enabled = true;
        AtBashAppIcon.GetComponent<Button>().enabled = true;
        CaesarApp.SetActive(false);
        AtBashApp.SetActive(false);
        ProgressBar.value = (float)decryptedMessagesLevelTwo / (float)goal;
        ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

        if (wordCaesar_AtBash == null || wordCaesar_AtBash == "")
            Caesar_AtBash_EncryptFunction();
        else
            CipheredText.text = caesar_atbashWord.ToUpper();
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

    public void Caesar_AtBash_EncryptFunction()
    {
        ///THIS CONTENT IS MOVED TO FINAL VERSION
        //if (PlainWords.Caesar_AtBash_Words.Count > 0)
        if (PlainWords.Caesar_AtBash_Words.Count > 0 && decryptedMessagesLevelTwo != goal)
        {
            int number = Random.Range(0, PlainWords.Caesar_AtBash_Words.Count);
            wordCaesar_AtBash = PlainWords.Caesar_AtBash_Words[number].ToUpper();
            PlainWords.Caesar_AtBash_Words.RemoveAt(number);
            caesar_atbashWord = "";

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
        while (index <= wordCaesar_AtBash.Length - 1)
        {
            letter = (char)(wordCaesar_AtBash[index] + key);
            if (letter >= 91)
                caesar_atbashWord += ((char)(letter - 26)).ToString();
            else
                caesar_atbashWord += letter.ToString();
            index++;
        }
        PlainWords.CaesarCodes--;
        CipheredText.text = caesar_atbashWord.ToUpper();
    }

    public void AtBashAlgorithm()
    {
        char letter;

        int index = 0;
        while (index <= wordCaesar_AtBash.Length - 1)
        {
            letter = (char)(90 - (wordCaesar_AtBash[index] - 65));
            caesar_atbashWord += letter.ToString();
            index++;
        }
        PlainWords.AtBashCodes--;
        CipheredText.text = caesar_atbashWord.ToUpper();
    }

    public void SendSolutionReport()
    {
        if (Solution.text.ToUpper() == wordCaesar_AtBash)
        {
            Caesar_AtBash_EncryptFunction();
            hideTimer = 2;
            StartCoroutine(ShowResult());
            ResultMessage.color = Color.green;
            ResultMessage.text = "Correct";
            decryptedMessagesLevelTwo++;
            if (decryptedMessagesLevelTwo <= goal)
            {
                ProgressBar.value = (float)decryptedMessagesLevelTwo / (float)goal;
                ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

                if (decryptedMessagesLevelTwo == goal)
                {
                    AccessEnigmaScript.Level2Completed = true;
                    AccountPassword.gameObject.SetActive(true);
                }

                ///THIS CONTENT IS MOVED TO FINAL VERSION
                //if (decryptedMessagesLevelTwo == goal)
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
