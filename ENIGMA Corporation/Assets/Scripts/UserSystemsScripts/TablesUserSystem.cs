using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PlainWords;
using static AccessEnigmaScript;

public class TablesUserSystem : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject VigenereApp;
    //[SerializeField] private GameObject MorseApp;
    [Header("-------------------------------------------------")]
    [Header("Buttons")]
    [SerializeField] private Button VigenereCreateLinesButton;
    [SerializeField] private Button CloseVigenereAppButton;
    //[SerializeField] private Button CloseXORAppButton;
    [SerializeField] private Button SolveReportButton;
    //[SerializeField] private Button LogoutButton;
    [Header("-------------------------------------------------")]
    [Header("InputFields")]
    [SerializeField] private InputField VigenereKey;
    //[SerializeField] private InputField Notepad;
    [SerializeField] private InputField Solution;
    [Header("-------------------------------------------------")]
    [Header("Texts")]
    [SerializeField] private Text[] VigenereKeyLetter = new Text[10];
    [SerializeField] private Text[] VigenereLine = new Text[10];
    [SerializeField] private Text VigenereError;

    [SerializeField] private Text ProgressValue;
    [SerializeField] private Text ResultMessage;
    [SerializeField] private Text CipheredText;
    [SerializeField] private Text AccountPassword;
    [Header("-------------------------------------------------")]
    [Header("Other")]
    [SerializeField] private Slider ProgressBar;
    [SerializeField] private Image KeyIndicator;
    [SerializeField] private Image VigenereAppIcon;
    //[SerializeField] private Image XORAppIcon;

    Color[] KeyColors = { new Color(1f, 0f, 0f), new Color(0f, 1f, 0f), new Color(0f, 0f, 1f), new Color(1f, 1f, 0f), new Color(0.73f, 0f, 1f), new Color(0f, 0f, 0f)};

    string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    string[] keyWords = { "IBEX", "FAWN", "TAHR", "DRAKE", "DUCK", "GOOSE", "NARWHAL", "CUTTLEFISH", "HIPPO", "HERON", "QUETZAL", "OWL", "WEREWOLF", "VAMPIRE", "MINOTAUR" };

    public static int keyColorID;
    public static string Level4Answer;       //Answer which need to be given
    public static string Level4EncryptedWord;     //Encrypted answer word container
    public static int usedXORKey;
    public static encryptingMethods usedMethod;
    int goal = 6;
    int hideTimer;

    void Start()
    {
        if (AccessEnigmaScript.EliteAccessGranted != true)
            AccessEnigmaScript.EliteAccessGranted = true;

        //if (AccessEnigmaScript.EliteAccessGranted != true && AccessEnigmaScript.Level3Completed == true)
        //    AccountPassword.gameObject.SetActive(true);

        VigenereAppIcon.GetComponent<Button>().enabled = true;
        //XORAppIcon.GetComponent<Button>().enabled = true;
        VigenereApp.SetActive(false);
        //XORApp.SetActive(false);*/
        ProgressBar.value = (float)AccessEnigmaScript.decryptedMessagesLevelFour / (float)goal;
        ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

        if (Level4Answer == null || Level4Answer == "")
            ADFVGX_PlayFair_Vigenere_EncryptFunction();
        else
            CipheredText.text = Level4EncryptedWord.ToString();
    }

    public bool TestEnteredKey(string key)
    {
        bool correct = true;
        foreach(char c in key)
            if(c < 65 || c > 90)
            {
                correct = false;
                break;
            }
        return correct;
    }

    //########################################################################################
    //###########################[Vigenere Cipher Part]#######################################
    //########################################################################################

    public void OpenVigenereApp()
    {
        VigenereKey.text = "";
        VigenereAppIcon.GetComponent<Button>().enabled = false;
        ResetVigenere();
        VigenereApp.SetActive(true);
    }

    public void CloseVigenereApp()
    {
        VigenereApp.SetActive(false);
        VigenereAppIcon.GetComponent<Button>().enabled = true;
    }

    public void GenerateVigenere()
    {
        string vinKey = VigenereKey.text.ToUpper();
        ResetVigenere();
        if (vinKey.Length > 0)
            if (TestEnteredKey(vinKey))
            {
                int index = 0;
                int alphaIndex;

                while (index < vinKey.Length)
                {
                    alphaIndex = vinKey[index] - 65;
                    VigenereKeyLetter[index].text = "Letter " + vinKey[index].ToString();
                    for (int l = 0; l < 26; l++)
                    {
                        VigenereLine[index].text += alphabet[alphaIndex] + " ";
                        alphaIndex = ((alphaIndex + 1) == 26 ? 0 : (alphaIndex + 1));
                    }
                    index++;
                }
            }
            else
            {
                VigenereKey.text = "";
                VigenereError.gameObject.SetActive(true);
                hideTimer = 2;
                StartCoroutine(ShowVigenereError());
            }
    }

    public void ResetVigenere()
    {
        for (int v = 0; v < 10; v++)
        {
            VigenereKeyLetter[v].text = "Letter -";
            VigenereLine[v].text = "";
        }
    }

    //########################################################################################
    //##############################[XOR Cipher Part]#########################################
    //########################################################################################

    public void OpenXORApp()
    {
/*        XORAppIcon.GetComponent<Button>().enabled = false;
        XORApp.SetActive(true);
        ChangeXORKey(0);
        XOREncryptedMessage.text = "";*/
    }

    public void CloseXORApp()
    {
/*        XORApp.SetActive(false);
        XORAppIcon.GetComponent<Button>().enabled = true;*/
    }

    //########################################################################################
    //##############################[ENCRYPTION Part]#########################################
    //########################################################################################

    public void ADFVGX_PlayFair_Vigenere_EncryptFunction()
    {
        ///THIS CONTENT IS MOVED TO FINAL VERSION
        //if (PlainWords.ADFVGX_PlayFair_Vigenere_Words.Count > 0)
        if (PlainWords.ADFVGX_PlayFair_Vigenere_Words.Count > 0 && AccessEnigmaScript.decryptedMessagesLevelFour != goal)
        {
            int number = Random.Range(0, PlainWords.ADFVGX_PlayFair_Vigenere_Words.Count);
            Level4Answer = PlainWords.ADFVGX_PlayFair_Vigenere_Words[number];
            //PlainWords.ADFVGX_PlayFair_Vigenere_Words.RemoveAt(number);
            Level4EncryptedWord = "";

            if (PlainWords.ADFVGXCodes > 0 && PlainWords.PlayfairCodes > 0 && PlainWords.VigenereCodes > 0)
            {
                int choice = Random.Range(0, 3);
                if (choice == 0)
                    Debug.Log("ADFVGXAlgorithm"); //ADFVGXAlgorithm();
                else if (choice == 1)
                    Debug.Log("PlayfairAlgorithm"); //PlayfairAlgorithm();
                else
                    VigenereAlgorithm();
            }
            else if (PlainWords.ADFVGXCodes == 0 && PlainWords.PlayfairCodes > 0 && PlainWords.VigenereCodes > 0)
            {
                int choice = Random.Range(0, 2);
                if (choice == 0)
                    Debug.Log("PlayFairAlgorithm"); //ADFVGXAlgorithm();
                else
                    VigenereAlgorithm();
            }
            else if (PlainWords.ADFVGXCodes > 0 && PlainWords.PlayfairCodes == 0 && PlainWords.VigenereCodes > 0)
            {
                int choice = Random.Range(0, 2);
                if (choice == 0)
                    Debug.Log("ADFVGXAlgorithm"); //ADFVGXAlgorithm();
                else
                    VigenereAlgorithm();
            }
            else if (PlainWords.ADFVGXCodes > 0 && PlainWords.PlayfairCodes > 0 && PlainWords.VigenereCodes == 0)
            {
                int choice = Random.Range(0, 2);
                if (choice == 0)
                    Debug.Log("PlayFairAlgorithm"); //ADFVGXAlgorithm();
                else
                    Debug.Log("PlayFairAlgorithm"); //ADFVGXAlgorithm();
            }
            //else if (PlainWords.ADFVGXCodes > 0 && PlainWords.PlayfairCodes == 0 && PlainWords.VigenereCodes == 0)
            //    ADFVGXAlgorithm();
            //else if (PlainWords.ADFVGXCodes == 0 && PlainWords.PlayfairCodes > 0 && PlainWords.VigenereCodes == 0)
            //    PlayFairAlgorithm();
            else if (PlainWords.ADFVGXCodes == 0 && PlainWords.PlayfairCodes == 0 && PlainWords.VigenereCodes > 0)
                VigenereAlgorithm();
        }
        else
        {
            CipheredText.text = "N/A";
            KeyIndicator.color = KeyColors[5];
            Solution.enabled = false;
            SolveReportButton.enabled = false;
        }
    }

    public void MorseAlgorithm()
    {
/*        int index = 0;
        while (index <= Level4Answer.Length - 1)
        {
            Level4EncryptedWord += MorseTable[Level4Answer[index]];
            index++;
            if (index <= Level4Answer.Length - 1)
                Level4EncryptedWord += "3";
        }
        PlainWords.MorseCodes--;
        CipheredTextLength.text = Level4Answer.Length.ToString();
        MorseRoutine = MorseLightsActivation();     //I honestly think this it utterly stupid to assign this one
                                                    //variable each time when this function is called.
                                                    //But honestly, this is the only solution how I found
                                                    //to reset this Coroutine from the very start :/
        StartCoroutine(MorseRoutine);*/
    }

    public void XORAlgorithm()
    {
/*        usedXORKey = Random.Range(0, 4);
        int index = 0;
        int binPower;
        Level4EncryptedWord = "";
        while (index <= Level4Answer.Length - 1)
        {
            int power = 7;
            int letterValue = Level4Answer[index] ^ XORKeys[usedXORKey];
            while (power >= 0)
            {
                binPower = (int)Mathf.Pow(2, power);
                if (letterValue / binPower == 1)
                {
                    Level4EncryptedWord += "1";
                    letterValue -= binPower;
                }
                else
                    Level4EncryptedWord += "0";

                power--;
            }

            index++;
            if (index <= Level4Answer.Length - 1)
                Level4EncryptedWord += "_";
        }
        PlainWords.XORCodes--;
        CipheredTextLength.text = Level4Answer.Length.ToString();
        XORRoutine = XORLightsActivation();     //The same as above case
        StartCoroutine(XORRoutine);*/
    }

    public void VigenereAlgorithm()
    {
        int chosenKey = Random.Range(0, 15);
        keyColorID = chosenKey / 3;
        int index = 0;
        int keyIndex = 0;
        char letter;

        while (index < Level4Answer.Length)
        {
            letter = (char)((Level4Answer[index] - 32) + (keyWords[chosenKey][keyIndex] - 65));
            if(letter > 90)
                letter = (char)(letter - 26);
            Level4EncryptedWord += letter;
            keyIndex = (keyIndex + 1) >= keyWords[chosenKey].Length ? 0 : (keyIndex + 1);
            index++;
        }
        //PlainWords.VigenereCodes--;
        KeyIndicator.color = KeyColors[keyColorID];
        CipheredText.text = Level4EncryptedWord;
    }

    public void SendSolutionReport()
    {
        if (Solution.text.ToLower() == Level4Answer)
        {
            hideTimer = 2;
            StartCoroutine(ShowResult());
            ResultMessage.color = Color.green;
            ResultMessage.text = "Correct";
            AccessEnigmaScript.decryptedMessagesLevelFour++;
            ADFVGX_PlayFair_Vigenere_EncryptFunction();
            if (AccessEnigmaScript.decryptedMessagesLevelFour <= goal)
            {
                ProgressBar.value = (float)AccessEnigmaScript.decryptedMessagesLevelFour / (float)goal;
                ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

                if (AccessEnigmaScript.decryptedMessagesLevelFour == goal)
                {
                    AccessEnigmaScript.Level3Completed = true;
                    AccountPassword.gameObject.SetActive(true);
                }

                ///THIS CONTENT IS MOVED TO FINAL VERSION
                //if (decryptedMessagesLevelThree == goal)
                //{
                //    PlainWords.MorseCodes = 7;
                //    PlainWords.XORCodes = 7;
                //    Morse_XOR_EncryptFunction();
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

    private IEnumerator ShowVigenereError()
    {
        while (hideTimer > 0)
        {
            yield return new WaitForSeconds(1);
            hideTimer--;
        }

        VigenereError.gameObject.SetActive(false);
        yield break;
    }

    public void LogOutFunc() =>
        SceneManager.LoadSceneAsync("Game_AllUsers");
}
