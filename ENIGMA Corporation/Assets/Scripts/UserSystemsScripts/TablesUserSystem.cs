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
    [SerializeField] private GameObject PlayFairApp;
    [SerializeField] private GameObject ADFVGXApp;
    [SerializeField] private GameObject ADFVGXInstructionPanel;
    [SerializeField] private GameObject PlayFairInstructionPanel;
    [SerializeField] private GameObject VigenereInstructionPanel;
    [Header("-------------------------------------------------")]
    [Header("Buttons")]
    [SerializeField] private Button VigenereCreateLinesButton;
    [SerializeField] private Button CloseVigenereAppButton;
    [SerializeField] private Button PlayFairGenerateButton;
    [SerializeField] private Button ClosePlayFairAppButton;
    [SerializeField] private Button ADFVGXFillSea;
    [SerializeField] private Button CloseADFVGXAppButton;
    [SerializeField] private Button SolveReportButton;
    [SerializeField] private Button LogoutButton;
    [Header("-------------------------------------------------")]
    [Header("InputFields")]
    [SerializeField] private InputField VigenereKey;
    [SerializeField] private InputField PlayFairKey;
    [SerializeField] private InputField ADFVGXKey;
    [SerializeField] private InputField Solution;
    [Header("-------------------------------------------------")]
    [Header("Texts")]
    [SerializeField] private Text[] VigenereKeyLetter = new Text[10];
    [SerializeField] private Text[] VigenereLine = new Text[10];
    [SerializeField] private Text VigenereError;

    [SerializeField] private Text PlayFairError;
    [SerializeField] private Text PlayFairTableVisual;

    [SerializeField] private Text ADFVGXError;
    [SerializeField] private Text ADFVGXTableVisual;

    [SerializeField] private Text ProgressValue;
    [SerializeField] private Text ResultMessage;
    [SerializeField] private Text CipheredText;
    [SerializeField] private Text AccountPassword;
    [Header("-------------------------------------------------")]
    [Header("Other")]
    [SerializeField] private Slider ProgressBar;
    [SerializeField] private Image KeyIndicator;
    [SerializeField] private Image VigenereAppIcon;
    [SerializeField] private Image PlayFairAppIcon;
    [SerializeField] private Image ADFVGXAppIcon;

    enum EncryptTools {Vigenere, PlayFair, ADFVGX};

    Color[] KeyColors = { new Color(1f, 0f, 0f), new Color(0f, 1f, 0f), new Color(0f, 0f, 1f), new Color(1f, 1f, 0f), new Color(0.73f, 0f, 1f), new Color(0f, 0f, 0f)};

    string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    string[] ADFVGXTableValues = { "A", "D", "F", "V", "G", "X" };

    string[] keyWords = { "IBEX", "FAWN", "TAHR", "DRAKE", "DUCK", "GOOSE", "NARWHAL", "CUTTLEFISH", "HIPPO", "HERON", "QUETZAL", "OWL", "WEREWOLF", "VAMPIRE", "MINOTAUR" };

    int chosenKey;
    public static int keyColorID;
    public static string Level4Answer;            //Answer which need to be given
    public static string Level4EncryptedWord;     //Encrypted answer word container
    public static int usedXORKey;
    EncryptTools ActiveTool;
    int goal = 7;
    int hideTimer;

    void Start()
    {
        AccessEnigmaScript.EliteAccessGranted = true;

        if (AccessEnigmaScript.Level4Completed == true)
            AccountPassword.gameObject.SetActive(true);

        VigenereAppIcon.GetComponent<Button>().enabled = true;
        PlayFairAppIcon.GetComponent<Button>().enabled = true;
        ADFVGXAppIcon.GetComponent<Button>().enabled = true;
        VigenereApp.SetActive(false);
        PlayFairApp.SetActive(false);
        ADFVGXApp.SetActive(false);
        ProgressBar.value = (float)AccessEnigmaScript.decryptedMessagesLevelFour / (float)goal;
        ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

        if (!AccessEnigmaScript.Level4Completed)
        {
            if (Level4Answer == null || Level4Answer == "")
                ADFVGX_PlayFair_Vigenere_EncryptFunction();
            else
                CipheredText.text = Level4EncryptedWord.ToString();
        }
        else
        {
            CipheredText.text = "N/A";
            KeyIndicator.color = KeyColors[5];
            Solution.enabled = false;
            SolveReportButton.enabled = false;
        }
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

    public void ShowADFVGXInstruction() =>
        ADFVGXInstructionPanel.SetActive(true);

    public void HideADFVGXInstruction() =>
        ADFVGXInstructionPanel.SetActive(false);

    public void ShowPlayFairInstruction() =>
        PlayFairInstructionPanel.SetActive(true);

    public void HidePlayFairInstruction() =>
        PlayFairInstructionPanel.SetActive(false);

    public void ShowVigenereInstruction() =>
        VigenereInstructionPanel.SetActive(true);

    public void HideVigenereInstruction() =>
        VigenereInstructionPanel.SetActive(false);

    //########################################################################################
    //##############################[ADFVGX Cipher Part]######################################
    //########################################################################################

    public void OpenADFVGXApp()
    {
        ActiveTool = EncryptTools.ADFVGX;
        ADFVGXAppIcon.GetComponent<Button>().enabled = false;
        ADFVGXApp.SetActive(true);
        ADFVGXKey.text = "";
        ADFVGXTableVisual.text = "";
    }

    public void CloseADFVGXApp()
    {
        ADFVGXApp.SetActive(false);
        ADFVGXAppIcon.GetComponent<Button>().enabled = true;
    }

    public void GenerateADFVGX()
    {
        string ADKey = ADFVGXKey.text.ToUpper();
        int indexer;
        ADFVGXTableVisual.text = "";
        if (ADKey.Length > 0)
            if (TestEnteredKey(ADKey))
            {
                string allSymbolsTable = "A1B2C3D4E5F6G7H8I9J0KLMNOPQRSTUVWXYZ";
                int lettersInLine = 0;

                while (ADKey.Length > 0)
                {
                    ADFVGXTableVisual.text += ADKey[0];
                    if(ADKey[0] >= 65 && ADKey[0] <= 74)
                    {
                        if (lettersInLine % 6 == 5)
                            ADFVGXTableVisual.text += "\n";
                        else
                            ADFVGXTableVisual.text += " ";
                        lettersInLine++;

                        indexer = allSymbolsTable.IndexOf(ADKey[0]) + 1;
                        ADFVGXTableVisual.text += allSymbolsTable[indexer];
                        allSymbolsTable = allSymbolsTable.Replace(ADFVGXTableVisual.text[ADFVGXTableVisual.text.Length - 1].ToString(), "");
                    }
                    allSymbolsTable = allSymbolsTable.Replace(ADKey[0].ToString(), "");
                    ADKey = ADKey.Replace(ADKey[0].ToString(), "");

                    if (lettersInLine % 6 == 5)
                        ADFVGXTableVisual.text += "\n";
                    else
                        ADFVGXTableVisual.text += " ";
                    lettersInLine++;
                }
                int index = 0;
                while (index < allSymbolsTable.Length)
                {
                    ADFVGXTableVisual.text += allSymbolsTable[index];

                    if (lettersInLine % 6 == 5)
                        ADFVGXTableVisual.text += "\n";
                    else
                        ADFVGXTableVisual.text += " ";
                    lettersInLine++;
                    index++;
                }
            }
            else
            {
                ADFVGXKey.text = "";
                ADFVGXError.gameObject.SetActive(true);
                hideTimer = 2;
                StartCoroutine(ShowError());
            }
    }

    //########################################################################################
    //############################[PlayFair Cipher Part]######################################
    //########################################################################################

    public void OpenPlayFairApp()
    {
        ActiveTool = EncryptTools.PlayFair;
        PlayFairAppIcon.GetComponent<Button>().enabled = false;
        PlayFairApp.SetActive(true);
        PlayFairKey.text = "";
        PlayFairTableVisual.text = "";
    }

    public void ClosePlayFairApp()
    {
        PlayFairApp.SetActive(false);
        PlayFairAppIcon.GetComponent<Button>().enabled = true;
    }

    public void GeneratePlayFair()
    {
        string playKey = PlayFairKey.text.ToUpper();
        PlayFairTableVisual.text = "";
        if (playKey.Length > 0)
            if (TestEnteredKey(playKey))
            {
                playKey = playKey.Replace("J", "I");
                string wholeAlphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
                int lettersInLine = 0;

                while (playKey.Length > 0)
                {
                    PlayFairTableVisual.text += playKey[0];
                    wholeAlphabet = wholeAlphabet.Replace(playKey[0].ToString(), "");
                    playKey = playKey.Replace(playKey[0].ToString(), "");

                    if (lettersInLine % 5 == 4)
                        PlayFairTableVisual.text += "\n";
                    else
                        PlayFairTableVisual.text += " ";
                    lettersInLine++;
                }
                int index = 0;
                while (index < wholeAlphabet.Length)
                {
                    PlayFairTableVisual.text += wholeAlphabet[index];

                    if (lettersInLine % 5 == 4)
                        PlayFairTableVisual.text += "\n";
                    else
                        PlayFairTableVisual.text += " ";
                    lettersInLine++;
                    index++;
                }
            }
            else
            {
                PlayFairKey.text = "";
                PlayFairError.gameObject.SetActive(true);
                hideTimer = 2;
                StartCoroutine(ShowError());
            }
    }

    //########################################################################################
    //###########################[Vigenere Cipher Part]#######################################
    //########################################################################################

    public void OpenVigenereApp()
    {
        ActiveTool = EncryptTools.Vigenere;
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
                StartCoroutine(ShowError());
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
            PlainWords.ADFVGX_PlayFair_Vigenere_Words.RemoveAt(number);
            Level4EncryptedWord = "";
            chosenKey = Random.Range(0, 15);
            keyColorID = chosenKey / 3;

            if (PlainWords.ADFVGXCodes > 0 && PlainWords.PlayFairCodes > 0 && PlainWords.VigenereCodes > 0)
            {
                int choice = Random.Range(0, 3);
                if (choice == 0)
                    ADFVGXAlgorithm();
                else if (choice == 1)
                    PlayFairAlgorithm();
                else
                    VigenereAlgorithm();
            }
            else if (PlainWords.ADFVGXCodes == 0 && PlainWords.PlayFairCodes > 0 && PlainWords.VigenereCodes > 0)
            {
                int choice = Random.Range(0, 2);
                if (choice == 0)
                    PlayFairAlgorithm();
                else
                    VigenereAlgorithm();
            }
            else if (PlainWords.ADFVGXCodes > 0 && PlainWords.PlayFairCodes == 0 && PlainWords.VigenereCodes > 0)
            {
                int choice = Random.Range(0, 2);
                if (choice == 0)
                    ADFVGXAlgorithm();
                else
                    VigenereAlgorithm();
            }
            else if (PlainWords.ADFVGXCodes > 0 && PlainWords.PlayFairCodes > 0 && PlainWords.VigenereCodes == 0)
            {
                int choice = Random.Range(0, 2);
                if (choice == 0)
                    ADFVGXAlgorithm();
                else
                    PlayFairAlgorithm();
            }
            else if (PlainWords.ADFVGXCodes > 0 && PlainWords.PlayFairCodes == 0 && PlainWords.VigenereCodes == 0)
                ADFVGXAlgorithm();
            else if (PlainWords.ADFVGXCodes == 0 && PlainWords.PlayFairCodes > 0 && PlainWords.VigenereCodes == 0)
                PlayFairAlgorithm();
            else if (PlainWords.ADFVGXCodes == 0 && PlainWords.PlayFairCodes == 0 && PlainWords.VigenereCodes > 0)
                VigenereAlgorithm();
            KeyIndicator.color = KeyColors[keyColorID];
            CipheredText.text = Level4EncryptedWord;
        }
        else
        {
            CipheredText.text = "N/A";
            KeyIndicator.color = KeyColors[5];
            Solution.enabled = false;
            SolveReportButton.enabled = false;
        }
    }

    public void ADFVGXAlgorithm()
    {
        string ADEncKey = keyWords[chosenKey];
        string allSymbolsTable = "A1B2C3D4E5F6G7H8I9J0KLMNOPQRSTUVWXYZ";
        string encryptionTable = "";
        string answerBuffor = Level4Answer.ToUpper();
        int indexer;
        char number;

        while (ADEncKey.Length > 0)
        {
            encryptionTable += ADEncKey[0];
            if (ADEncKey[0] >= 65 && ADEncKey[0] <= 74)
            {
                indexer = allSymbolsTable.IndexOf(ADEncKey[0]) + 1;
                number = allSymbolsTable[indexer];
                allSymbolsTable = allSymbolsTable.Replace(number.ToString(), "");
            }
            allSymbolsTable = allSymbolsTable.Replace(ADEncKey[0].ToString(), "");
            ADEncKey = ADEncKey.Replace(ADEncKey[0].ToString(), "");
        }
        encryptionTable += allSymbolsTable;

        int index = 0;
        int position;
        while (index < answerBuffor.Length)
        {
            position = encryptionTable.IndexOf((char)(answerBuffor[index]));
            Level4EncryptedWord += ADFVGXTableValues[position / 6];
            Level4EncryptedWord += ADFVGXTableValues[position % 6];
            index++;
        }
        PlainWords.ADFVGXCodes--;
    }

    public void PlayFairAlgorithm()
    {
        string playEncKey = keyWords[chosenKey];
        string wholeAlphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
        string encryptionTable = "";
        string answerBuffor = Level4Answer.ToUpper();

        while (playEncKey.Length > 0)
        {
            encryptionTable += playEncKey[0];
            wholeAlphabet = wholeAlphabet.Replace(playEncKey[0].ToString(), "");
            playEncKey = playEncKey.Replace(playEncKey[0].ToString(), "");
        }
        encryptionTable += wholeAlphabet;

        if (answerBuffor.Length % 2 == 1)
            answerBuffor += (char)(answerBuffor[answerBuffor.Length - 1] + 1);
        answerBuffor = answerBuffor.Replace("J", "I");

        int index = 0;
        int[] positions = new int[2];
        int move = 0;

        while (index < answerBuffor.Length)
        {
            positions[0] = encryptionTable.IndexOf((char)(answerBuffor[index]));
            positions[1] = encryptionTable.IndexOf((char)(answerBuffor[index + 1]));

            if(positions[0] / 5 == positions[1] / 5)
            {
                Level4EncryptedWord += (positions[0] / 5 == (positions[0] + 1) / 5) ? encryptionTable[positions[0] + 1] : encryptionTable[positions[0] - 4];
                Level4EncryptedWord += (positions[1] / 5 == (positions[1] + 1) / 5) ? encryptionTable[positions[1] + 1] : encryptionTable[positions[1] - 4];
            }
            else if(positions[0] % 5 == positions[1] % 5)
            {
                Level4EncryptedWord += (positions[0] + 5 <= 24) ? encryptionTable[positions[0] + 5] : encryptionTable[positions[0] - 20];
                Level4EncryptedWord += (positions[1] + 5 <= 24) ? encryptionTable[positions[1] + 5] : encryptionTable[positions[1] - 20];
            }
            else
            {
                if(positions[0] % 5 < positions[1] % 5)
                {
                    move = (positions[1] % 5) - (positions[0] % 5);
                    Level4EncryptedWord += encryptionTable[positions[0] + move];
                    Level4EncryptedWord += encryptionTable[positions[1] - move];
                }
                else
                {
                    move = (positions[0] % 5) - (positions[1] % 5);
                    Level4EncryptedWord += encryptionTable[positions[0] - move];
                    Level4EncryptedWord += encryptionTable[positions[1] + move];
                }
            }
            index += 2;
        }
        PlainWords.PlayFairCodes--;
    }

    public void VigenereAlgorithm()
    {
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
        PlainWords.VigenereCodes--;
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
                    AccessEnigmaScript.Level4Completed = true;
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

    private IEnumerator ShowError()
    {
        while (hideTimer > 0)
        {
            yield return new WaitForSeconds(1);
            hideTimer--;
        }

        if (ActiveTool == EncryptTools.Vigenere)
            VigenereError.gameObject.SetActive(false);
        else if (ActiveTool == EncryptTools.PlayFair)
            PlayFairError.gameObject.SetActive(false);
        else if (ActiveTool == EncryptTools.ADFVGX)
            ADFVGXError.gameObject.SetActive(false);

        yield break;
    }

    public void LogOutFunc() =>
        SceneManager.LoadSceneAsync("Game_AllUsers");
}
