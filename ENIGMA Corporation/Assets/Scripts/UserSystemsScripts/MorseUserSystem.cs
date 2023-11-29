using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PlainWords;
using static AccessEnigmaScript;

public class MorseUserSystem : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject XORApp;
    [SerializeField] private GameObject MorseApp;
    [Header("-------------------------------------------------")]
    [Header("Buttons")]
    [SerializeField] private Button[] MorseSymbolButtons = new Button[12];
    [SerializeField] private Button[] XORKeysButton = new Button[4];
    [SerializeField] private Button CloseMorseAppButton;
    [SerializeField] private Button CloseXORAppButton;
    [SerializeField] private Button SolveReportButton;
    [SerializeField] private Button LogoutButton;
    [Header("-------------------------------------------------")]
    [Header("InputFields")]
    [SerializeField] private InputField XOREncryptedMessage;
    [SerializeField] private InputField Notepad;
    [SerializeField] private InputField Solution;
    [Header("-------------------------------------------------")]
    [Header("Texts")]
    [SerializeField] private Text ProgressValue;
    [SerializeField] private Text ResultMessage;
    [SerializeField] private Text CipheredTextLength;
    [SerializeField] private Text AccountPassword;
    [Header("-------------------------------------------------")]
    [Header("Other")]
    [SerializeField] private Slider ProgressBar;
    [SerializeField] private Image MorseAppIcon;
    [SerializeField] private Image XORAppIcon;
    [SerializeField] private Image[] Lights = new Image[3];

    private IEnumerator MorseRoutine;
    private IEnumerator XORRoutine;

    //Dot => 1, Dash => 2, Break => 3
    Dictionary<char, string> MorseTable = new Dictionary<char, string>() { {'a', "12__"}, {'b', "2111"}, {'c', "2121"}, {'d', "211_"}, {'e', "1___"}, {'f', "1121"}, {'g', "221_"}, {'h', "1111"}, {'i', "11__"}, {'j', "1222"}, {'k', "212_"}, {'l', "1211"}, {'m', "22__"}, {'n', "21__"}, {'o', "222_"}, {'p', "1221"}, {'q', "2212"}, {'r', "121_"}, {'s', "111_"}, {'t', "2___"}, {'u', "112_"}, {'v', "1112"}, {'w', "122_"}, {'x', "2112"}, {'y', "2122"}, {'z', "2211"} };

    Color[] LightsColors = { new Color(0f, 0f, 0f), new Color(1f, 0.65f, 0f), new Color(1f, 0f, 0f), new Color(0f, 1f, 0f), new Color(0f, 0f, 1f), new Color(1f, 1f, 0f), new Color(1f, 1f, 1f)};     //[0] - Default/Break, [1] - Morse, [2]-[5] - XOR, [6] - XOR Space 
    
    int[] XORKeys = new int[4] {83, 30, 97, 72};         //RED | GREEN | BLUE | YELLOW
    int activeXORKey;
    string[] MorseElements = new string[4];              //dots (1), dashes (2) or empty spaces, used to decrypt Morse
    public static string Level3Answer;     //Answer which need to be given
    public static string Level3EncryptedWord;     //Encrypted answer word container
    public static int usedXORKey;
    public static encryptingMethods usedMethod;
    int goal = 6;
    int hideTimer;
    int symbol;

    void Start()
    {
        if (AccessEnigmaScript.ProfessionalAccessGranted != true)
            AccessEnigmaScript.ProfessionalAccessGranted = true;

        if (AccessEnigmaScript.EliteAccessGranted != true && AccessEnigmaScript.Level3Completed == true)
            AccountPassword.gameObject.SetActive(true);

        MorseAppIcon.GetComponent<Button>().enabled = true;
        XORAppIcon.GetComponent<Button>().enabled = true;
        MorseApp.SetActive(false);
        XORApp.SetActive(false);
        ProgressBar.value = (float)AccessEnigmaScript.decryptedMessagesLevelThree / (float)goal;
        ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

        if (Level3Answer == null || Level3Answer == "")
            Morse_XOR_EncryptFunction();
        else
        {
            CipheredTextLength.text = Level3Answer.Length.ToString();
            if(usedMethod == encryptingMethods.Morse)
            {
                MorseRoutine = MorseLightsActivation();
                StartCoroutine(MorseRoutine);
            }
            else
            {
                XORRoutine = XORLightsActivation();
                StartCoroutine(XORRoutine);
            }
        }
    }

    //########################################################################################
    //#############################[Morse Cipher Part]########################################
    //########################################################################################

    public void OpenMorseApp()
    {
        MorseAppIcon.GetComponent<Button>().enabled = false;
        ResetMorseButtons();
        MorseApp.SetActive(true);
    }

    public void CloseMorseApp()
    {
        MorseApp.SetActive(false);
        MorseAppIcon.GetComponent<Button>().enabled = true;
    }

    public void SetMorseButtons(int buttonNumber)
    {
        int column = buttonNumber % 3;
        int row = buttonNumber / 3;

        MorseSymbolButtons[row * 3].interactable = true;
        MorseSymbolButtons[row * 3 + 1].interactable = true;
        MorseSymbolButtons[row * 3 + 2].interactable = true;

        MorseSymbolButtons[buttonNumber].interactable = false;

        MorseElements[row] = (column == 0 ? "1" : (column == 1 ? "2" : "_"));
    }

    public void ResetMorseButtons()
    {
        for (int i = 0; i < 12; i++)
            MorseSymbolButtons[i].interactable = true;
        for (int s = 0; s < 4; s++)
            MorseElements[s] = " ";
    }

    public void DecryptMorse()
    {
        string receivedPattern = MorseElements[0] + MorseElements[1] + MorseElements[2] + MorseElements[3];
        char test = MorseTable.FirstOrDefault(x => x.Value == receivedPattern).Key;
        char letter;

        if (!MorseTable.ContainsKey(test))
            letter = '?';
        else
            letter = test;

        Notepad.text += letter;
    }

    //########################################################################################
    //##############################[XOR Cipher Part]#########################################
    //########################################################################################

    public void OpenXORApp()
    {
        XORAppIcon.GetComponent<Button>().enabled = false;
        XORApp.SetActive(true);
        ChangeXORKey(0);
        XOREncryptedMessage.text = "";
    }

    public void CloseXORApp()
    {
        XORApp.SetActive(false);
        XORAppIcon.GetComponent<Button>().enabled = true;
    }

    public void ChangeXORKey(int keyIndex)
    {
        for (int x = 0; x < 4; x++)
            XORKeysButton[x].interactable = (x == keyIndex ? false : true);
        activeXORKey = XORKeys[keyIndex];
    }

    public void ClearXORField() =>
        XOREncryptedMessage.text = "";

    public void DecryptXOR()
    {
        if(XOREncryptedMessage.text.Length == 8)
        {
            int power = 7;
            int charValue = 0;

            foreach(char c in XOREncryptedMessage.text)
            {
                charValue += (int)(Mathf.Pow(2, power) * (c == '0' ? 0 : 1));
                power--;
            }

            int letterCode = charValue ^ activeXORKey;
            char letter;

            if (letterCode >= 97 && letterCode <= 122)
                letter = (char)letterCode;
            else
                letter = '?';
            Notepad.text += letter;
        }
        else
            Notepad.text += "#";
    }

    //########################################################################################
    //##############################[ENCRYPTION Part]#########################################
    //########################################################################################

    public void Morse_XOR_EncryptFunction()
    {
        ///THIS CONTENT IS MOVED TO FINAL VERSION
        //if (PlainWords.XOR_Morse_Words.Count > 0)
        if (PlainWords.XOR_Morse_Words.Count > 0 && AccessEnigmaScript.decryptedMessagesLevelThree != goal)
        {
            int number = Random.Range(0, PlainWords.XOR_Morse_Words.Count);
            Level3Answer = PlainWords.XOR_Morse_Words[number];
            PlainWords.XOR_Morse_Words.RemoveAt(number);
            Level3EncryptedWord = "";

            if (PlainWords.MorseCodes > 0 && PlainWords.XORCodes > 0)
            {
                int choice = Random.Range(0, 2);
                if (choice == 0)
                {
                    usedMethod = encryptingMethods.Morse;
                    MorseAlgorithm();
                }
                else
                {
                    usedMethod = encryptingMethods.XOR;
                    XORAlgorithm();
                }
            }
            else if (PlainWords.MorseCodes == 0 && PlainWords.XORCodes > 0)
            {
                usedMethod = encryptingMethods.XOR;
                XORAlgorithm();
            }
            else if (PlainWords.MorseCodes > 0 && PlainWords.XORCodes == 0)
            {
                usedMethod = encryptingMethods.Morse;
                MorseAlgorithm();
            }
        }
        else
        {
            CipheredTextLength.text = "N/A";
            Solution.enabled = false;
            SolveReportButton.enabled = false;
        }
    }

    public void MorseAlgorithm()
    {
        int index = 0;
        while (index <= Level3Answer.Length - 1)
        {
            Level3EncryptedWord += MorseTable[Level3Answer[index]];
            index++;
            if(index <= Level3Answer.Length - 1)
                Level3EncryptedWord += "3";
        }
        PlainWords.MorseCodes--;
        CipheredTextLength.text = Level3Answer.Length.ToString();
        MorseRoutine = MorseLightsActivation();     //I honestly think this it utterly stupid to assign this one
                                                    //variable each time when this function is called.
                                                    //But honestly, this is the only solution how I found
                                                    //to reset this Coroutine from the very start :/
        StartCoroutine(MorseRoutine);
    }

    public void XORAlgorithm()
    {
        usedXORKey = Random.Range(0, 4);
        int index = 0;
        int binPower;
        Level3EncryptedWord = "";
        while (index <= Level3Answer.Length - 1)
        {
            int power = 7;
            int letterValue = Level3Answer[index] ^ XORKeys[usedXORKey];
            while(power >= 0)
            {
                binPower = (int)Mathf.Pow(2, power);
                if (letterValue / binPower == 1)
                {
                    Level3EncryptedWord += "1";
                    letterValue -= binPower;
                }
                else
                    Level3EncryptedWord += "0";             
                
                power--;
            }

            index++;
            if (index <= Level3Answer.Length - 1)
                Level3EncryptedWord += "_";
        }
        PlainWords.XORCodes--;
        CipheredTextLength.text = Level3Answer.Length.ToString();
        XORRoutine = XORLightsActivation();     //The same as above case
        StartCoroutine(XORRoutine);
    }

    public void SendSolutionReport()
    {
        if (Solution.text.ToLower() == Level3Answer)
        {
            Notepad.text = "";

            if (usedMethod == encryptingMethods.Morse)
                StopCoroutine(MorseRoutine);
            else
                StopCoroutine(XORRoutine);

            for (int l = 0; l < 3; l++)
                Lights[l].color = LightsColors[0];

            hideTimer = 2;
            StartCoroutine(ShowResult());
            ResultMessage.color = Color.green;
            ResultMessage.text = "Correct";
            AccessEnigmaScript.decryptedMessagesLevelThree++;
            StartCoroutine(Cooldown());
            if (AccessEnigmaScript.decryptedMessagesLevelThree <= goal)
            {
                ProgressBar.value = (float)AccessEnigmaScript.decryptedMessagesLevelThree / (float)goal;
                ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

                if (AccessEnigmaScript.decryptedMessagesLevelThree == goal)
                {
                    AccessEnigmaScript.Level3Completed = true;
                    AccountPassword.gameObject.SetActive(true);
                }

                ///THIS CONTENT IS MOVED TO FINAL VERSION
                //if (AccessEnigmaScript.decryptedMessagesLevelThree == goal)
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

    private IEnumerator MorseLightsActivation()
    {
        symbol = 0;
        while (true)
        {
            if(Level3EncryptedWord[symbol] != '_')
            {
                switch (Level3EncryptedWord[symbol])
                {
                    case '1':
                        Lights[0].color = LightsColors[1];
                        yield return new WaitForSeconds(0.5f);
                        break;
                    case '2':
                        Lights[0].color = LightsColors[1];
                        yield return new WaitForSeconds(1.5f);
                        break;
                    case '3':
                        Lights[0].color = LightsColors[0];
                        yield return new WaitForSeconds(1.5f);
                        break;
                }
                Lights[0].color = LightsColors[0];
                yield return new WaitForSeconds(0.5f);
            }
            symbol++;

            if (symbol == Level3EncryptedWord.Length)
            {
                symbol = 0;
                for (int l = 0; l < 3; l++)
                    Lights[l].color = LightsColors[1];
                yield return new WaitForSeconds(2.5f);
                for (int l = 0; l < 3; l++)
                    Lights[l].color = LightsColors[0];
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator XORLightsActivation()
    {
        while (true)
        {
            switch (Level3EncryptedWord[symbol])
            {
                case '1':
                    Lights[2].color = LightsColors[usedXORKey + 2];
                    yield return new WaitForSeconds(0.5f);
                    Lights[2].color = LightsColors[0];
                    break;
                case '0':
                    Lights[1].color = LightsColors[usedXORKey + 2];
                    yield return new WaitForSeconds(0.5f);
                    Lights[1].color = LightsColors[0];
                    break;
                case '_':
                    Lights[0].color = LightsColors[6];
                    yield return new WaitForSeconds(1f);
                    Lights[0].color = LightsColors[0];
                    break;
            }
            symbol++;
            yield return new WaitForSeconds(0.5f);

            if (symbol == Level3EncryptedWord.Length)
            {
                symbol = 0;
                for (int l = 0; l < 3; l++)
                    Lights[l].color = LightsColors[6];
                yield return new WaitForSeconds(2.5f);
                for (int l = 0; l < 3; l++)
                    Lights[l].color = LightsColors[0];
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(3);
        symbol = 0;
        Morse_XOR_EncryptFunction();
        yield break;
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
