using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnigmaLevelScripts : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject EnigmaPanel;
    [SerializeField] private GameObject InstructionsPanel;
    [SerializeField] private GameObject LockedKeyboard;
    [SerializeField] private GameObject RotorsOpened;
    [SerializeField] private GameObject RotorsClosed;
    [Header("-------------------------------------------------")]
    [Header("Buttons")]
    [SerializeField] private Button[] SwitchLetter = new Button[4];     //[0-1] Letter 1 | [2-3] Letter 2
    [SerializeField] private Button MakeSwitch;
    [SerializeField] private Button ResetSwitchboard;
    [SerializeField] private Button OpenRotors;
    [SerializeField] private Button CloseRotors;
    [SerializeField] private Button[] RotorsChoiceButtons = new Button[18];
    [SerializeField] private Button[] RotorChangePosition = new Button[6];  //[0-1] Left | [2-3] Middle | [4-5] Right
    [SerializeField] private Button[] Keyboard = new Button[26];
    [SerializeField] private Button SendAnswer;
    [SerializeField] private Button SwapPanels;
    [Header("-------------------------------------------------")]
    [Header("InputFields")]
    [SerializeField] private InputField EnigmaScreen;
    [SerializeField] private InputField AnswerField;
    [Header("-------------------------------------------------")]
    [Header("Texts")]
    [SerializeField] private Text SwitchBoard;
    [SerializeField] private Text EnigmaStatus;
    [SerializeField] private Text[] SwitchedLetters = new Text[2];
    [SerializeField] private Text[] RotorPosition = new Text[3];
    [SerializeField] private Text EncryptedFinalWord;
    [SerializeField] private Text ResultMessage;
    [Header("-------------------------------------------------")]
    [Header("Other")]
    [SerializeField] private Image[] ActiveRotors = new Image[3];

    List <string> switchBoard = new List<string>();
    bool switchBoardActive;
    int[] switcherLetterIndex = new int[2];

    List<string> enterRoller = new List<string> { "J", "W", "U", "L", "C", "M", "N", "O", "H", "P", "Q", "Z", "Y", "X", "I", "R", "A", "D", "K", "E", "G", "V", "B", "T", "S", "F" };

    string alphabetDefault = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string alphabetRemains;

    int[] chosenRotor = new int[3];
    int[] currentRotorPosition = new int[3];
    bool locked;
    public static int keyColorID;
    public static int usedXORKey;
    int hideTimer;

    void Start()
    {
        InstructionsPanel.SetActive(false);
        RotorsOpened.SetActive(false);
        RotorsClosed.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            RotorsChoiceButtons[i].interactable = false;
            chosenRotor[i] = -1;
        }
        CheckKeyboardBlockade();
        ClearSwitchboard();
        EncryptedFinalWord.text = "";
    }

    //########################################################################################
    //##################################[Rotors UI Part]######################################
    //########################################################################################

    public void OpenRotorsMenu()
    {
        RotorsOpened.SetActive(true);
        RotorsClosed.SetActive(false);
    }

    public void CloseRotorsMenu()
    {
        RotorsOpened.SetActive(false);
        RotorsClosed.SetActive(true);
    }

    public void InsertRotor(int pressed)
    {
        int rotorLine = pressed / 3;
        int rotorPosition = pressed % 3;
        if(chosenRotor[rotorPosition] != -1)
        {
            int swapLine = chosenRotor[rotorPosition];
            for (int o = 0; o < 3; o++)
            {
                RotorsChoiceButtons[swapLine * 3 + o].gameObject.SetActive(true);
                RotorsChoiceButtons[swapLine * 3 + o].interactable = true;
            }
        }
        for (int o = 0; o < 3; o++)
            RotorsChoiceButtons[rotorLine * 3 + o].gameObject.SetActive(false);

        RotorsChoiceButtons[rotorPosition].interactable = true;
        RotorsChoiceButtons[pressed].interactable = false;
        RotorsChoiceButtons[pressed].gameObject.SetActive(true);
        chosenRotor[rotorPosition] = rotorLine;

        CheckKeyboardBlockade();
    }

    public void RemoveRotor(int pressed)
    {
        int rotorLine = chosenRotor[pressed];
        RotorsChoiceButtons[pressed].interactable = false;
        chosenRotor[pressed] = -1;
        for (int o = 0; o < 3; o++)
        {
            RotorsChoiceButtons[rotorLine * 3 + o].gameObject.SetActive(true);
            RotorsChoiceButtons[rotorLine * 3 + o].interactable = true;
        }
        if(!locked)
        {
            LockedKeyboard.SetActive(true);
            EnigmaStatus.gameObject.SetActive(true);
            locked = true;
        }
    }

    public void ChangeRotorPosition(int pressed)
    {
        int wheel = pressed / 2;
        currentRotorPosition[wheel] = (pressed % 2 == 0) ? (currentRotorPosition[wheel] + 1) : (currentRotorPosition[wheel] - 1);
        
        if (currentRotorPosition[wheel] > 25)
            currentRotorPosition[wheel] = 0;
        else if (currentRotorPosition[wheel] < 0)
            currentRotorPosition[wheel] = 25;

        RotorPosition[wheel].text = (currentRotorPosition[wheel] + 1).ToString();
    }

    //########################################################################################
    //################################[SwitchBoard UI Part]###################################
    //########################################################################################

    public void ChangeLetter(int pressed)
    {
        int wheel = pressed / 2;

        if (wheel == 0)
            SwitchedLetters[0].text = LeftLetters(pressed, 0, 1);
        else
            SwitchedLetters[1].text = LeftLetters(pressed, 1, 0);
    }

    public string LeftLetters(int direction, int chosenSide, int oppositeSide)
    {
        if(direction % 2 == 0)
        {
            switcherLetterIndex[chosenSide]++;
            if (switcherLetterIndex[chosenSide] == switcherLetterIndex[oppositeSide])
                switcherLetterIndex[chosenSide]++;
            if (switcherLetterIndex[chosenSide] >= alphabetRemains.Length)
                switcherLetterIndex[chosenSide] = 0;
        }
        else
        {
            switcherLetterIndex[chosenSide]--;
            if (switcherLetterIndex[chosenSide] == switcherLetterIndex[oppositeSide])
                switcherLetterIndex[chosenSide]--;
            if (switcherLetterIndex[chosenSide] < 0)
                switcherLetterIndex[chosenSide] = alphabetRemains.Length - 1;
        }
        return alphabetRemains[switcherLetterIndex[chosenSide]].ToString();
            
    }

    public void CreateEnigmaSwitch()
    {
        if(!switchBoardActive)
            switchBoardActive = true;

        SwitchBoard.text += alphabetRemains[switcherLetterIndex[0]].ToString() + " <-> " + alphabetRemains[switcherLetterIndex[1]].ToString() + "\n";

        char left = (char)(alphabetRemains[switcherLetterIndex[0]]);
        char right = (char)(alphabetRemains[switcherLetterIndex[1]]);
        alphabetRemains = alphabetRemains.Replace(left, '_').Replace(right, '_');
        alphabetRemains = alphabetRemains.Replace("_", "");

        if (alphabetRemains.Length > 0)
        {
            switcherLetterIndex[0] = 0;
            SwitchedLetters[0].text = alphabetRemains[switcherLetterIndex[0]].ToString();
            switcherLetterIndex[1] = 1;
            SwitchedLetters[1].text = alphabetRemains[switcherLetterIndex[1]].ToString();
        }
        else
        {
            SwitchedLetters[0].text = "";
            SwitchedLetters[1].text = "";
            foreach (Button x in SwitchLetter)
                x.interactable = false;
            MakeSwitch.interactable = false;
        }

    }

    public void ClearSwitchboard()
    {
        SwitchBoard.text = "";
        switchBoardActive = false;
        switchBoard.Clear();
        switchBoard.AddRange(enterRoller);
        alphabetRemains = alphabetDefault;
        switcherLetterIndex[0] = 0;
        SwitchedLetters[0].text = alphabetRemains[switcherLetterIndex[0]].ToString();
        switcherLetterIndex[1] = 1;
        SwitchedLetters[1].text = alphabetRemains[switcherLetterIndex[1]].ToString();
        foreach (Button x in SwitchLetter)
            x.interactable = true;
        MakeSwitch.interactable = true;

    }

    //########################################################################################
    //###################################[Keyboard UI Part]###################################
    //########################################################################################

    public void CheckKeyboardBlockade()
    {
        if (chosenRotor[0] != -1 && chosenRotor[1] != -1 && chosenRotor[2] != -1)
        {
            LockedKeyboard.SetActive(false);
            EnigmaStatus.gameObject.SetActive(false);
            locked = false;
        }
        else
        {
            LockedKeyboard.SetActive(true);
            EnigmaStatus.gameObject.SetActive(true);
            locked = true;
        }
    }

    //########################################################################################
    //###########################[Vigenere Cipher Part]#######################################
    //########################################################################################

    /*public void OpenVigenereApp()
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
        string allSymbolsTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string encryptionTable = "";
        string answerBuffor = Level4Answer.ToUpper();

        while (ADEncKey.Length > 0)
        {
            encryptionTable += ADEncKey[0];
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

            if (positions[0] / 5 == positions[1] / 5)
            {
                Level4EncryptedWord += (positions[0] / 5 == (positions[0] + 1) / 5) ? encryptionTable[positions[0] + 1] : encryptionTable[positions[0] - 4];
                Level4EncryptedWord += (positions[1] / 5 == (positions[1] + 1) / 5) ? encryptionTable[positions[1] + 1] : encryptionTable[positions[1] - 4];
            }
            else if (positions[0] % 5 == positions[1] % 5)
            {
                Level4EncryptedWord += (positions[0] + 5 <= 24) ? encryptionTable[positions[0] + 5] : encryptionTable[positions[0] - 20];
                Level4EncryptedWord += (positions[1] + 5 <= 24) ? encryptionTable[positions[1] + 5] : encryptionTable[positions[1] - 20];
            }
            else
            {
                if (positions[0] % 5 < positions[1] % 5)
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
            if (letter > 90)
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
        SceneManager.LoadSceneAsync("Game_AllUsers");*/
}
