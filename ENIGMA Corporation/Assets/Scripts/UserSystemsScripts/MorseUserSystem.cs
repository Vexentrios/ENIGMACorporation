using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PlainWords;

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
    [Header("-------------------------------------------------")]
    [Header("Other")]
    [SerializeField] private Slider ProgressBar;
    [SerializeField] private Image MorseAppIcon;
    [SerializeField] private Image XORAppIcon;
    [SerializeField] private Image[] Lights = new Image[3];

    private IEnumerator MorseRoutine;
    private IEnumerator XORRoutine;

    //Dot => 1, Dash => 2, Break => 3, End => 4
    Dictionary<char, string> MorseTable = new Dictionary<char, string>() { {'a', "12__"}, {'b', "2111"}, {'c', "2121"}, {'d', "211_"}, {'e', "1___"}, {'f', "1121"}, {'g', "221_"}, {'h', "1111"}, {'i', "11__"}, {'j', "1222"}, {'k', "212_"}, {'l', "1211"}, {'m', "22__"}, {'n', "21__"}, {'o', "222_"}, {'p', "1221"}, {'q', "2212"}, {'r', "121_"}, {'s', "111_"}, {'t', "2___"}, {'u', "112_"}, {'v', "1112"}, {'w', "122_"}, {'x', "2112"}, {'y', "2122"}, {'z', "2211"} };

    Color[] LightsColors = { new Color(0f, 0f, 0f), new Color(1f, 0.65f, 0f), new Color(1f, 0f, 0f), new Color(0f, 1f, 0f), new Color(0f, 0f, 1f), new Color(1f, 1f, 0f), new Color(1f, 1f, 1f)};     //[0] - Default/Break, [1] - Morse, [2]-[5] - XOR, [6] - XOR Space 
    
    int[] XORKeys = new int[4] {83, 30, 97, 72};         //RED | GREEN | BLUE | YELLOW
    int activeXORKey;
    string[] MorseElements = new string[4];              //dots (1), dashes (2) or empty spaces, used to decrypt Morse
    public static string wordMorse_XOR;     //Answer which need to be given
    public static string morse_xorWord;     //Encrypted answer word container
    public static int usedXORKey;
    public static encryptingMethods usedMethod;
    public static int decryptedMessagesLevelThree = 0;
    int goal = 6;
    int hideTimer;
    int symbol;

    void Start()
    {
        MorseAppIcon.GetComponent<Button>().enabled = true;
        XORAppIcon.GetComponent<Button>().enabled = true;
        MorseApp.SetActive(false);
        XORApp.SetActive(false);
        ProgressBar.value = (float)decryptedMessagesLevelThree / (float)goal;
        ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

        if (wordMorse_XOR == null || wordMorse_XOR == "")
            Morse_XOR_EncryptFunction();
        else
            CipheredTextLength.text = morse_xorWord.Length.ToString();
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
        if (PlainWords.XOR_Morse_Words.Count > 0 && decryptedMessagesLevelThree != goal)
        {
            int number = Random.Range(0, PlainWords.XOR_Morse_Words.Count);
            wordMorse_XOR = PlainWords.XOR_Morse_Words[number];
            PlainWords.XOR_Morse_Words.RemoveAt(number);
            morse_xorWord = "";
            Debug.Log(wordMorse_XOR);

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
        while (index <= wordMorse_XOR.Length - 1)
        {
            morse_xorWord += MorseTable[wordMorse_XOR[index]];
            index++;
            if(index <= wordMorse_XOR.Length - 1)
                morse_xorWord += "3";
        }
        PlainWords.MorseCodes--;
        CipheredTextLength.text = wordMorse_XOR.Length.ToString();
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
        morse_xorWord = "";
        while (index <= wordMorse_XOR.Length - 1)
        {
            int power = 7;
            int letterValue = wordMorse_XOR[index] ^ XORKeys[usedXORKey];
            while(power >= 0)
            {
                binPower = (int)Mathf.Pow(2, power);
                if (letterValue / binPower == 1)
                {
                    morse_xorWord += "1";
                    letterValue -= binPower;
                }
                else
                    morse_xorWord += "0";             
                
                power--;
            }

            index++;
            if (index <= wordMorse_XOR.Length - 1)
                morse_xorWord += "_";
        }
        PlainWords.XORCodes--;
        CipheredTextLength.text = wordMorse_XOR.Length.ToString();
        XORRoutine = XORLightsActivation();     //The same as above case
        StartCoroutine(XORRoutine);
    }

    public void SendSolutionReport()
    {
        if (Solution.text.ToLower() == wordMorse_XOR)
        {
            Notepad.text = "";

            if (usedMethod == encryptingMethods.Morse)
                StopCoroutine(MorseRoutine);
            else
                StopCoroutine(XORRoutine);

            for (int l = 0; l < 3; l++)
                Lights[l].color = LightsColors[0];

            StartCoroutine(Cooldown());
            hideTimer = 2;
            StartCoroutine(ShowResult());
            ResultMessage.color = Color.green;
            ResultMessage.text = "Correct";
            decryptedMessagesLevelThree++;
            if (decryptedMessagesLevelThree <= goal)
            {
                ProgressBar.value = (float)decryptedMessagesLevelThree / (float)goal;
                ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

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

    private IEnumerator MorseLightsActivation()
    {
        symbol = 0;
        while (true)
        {
            if(morse_xorWord[symbol] != '_')
            {
                switch (morse_xorWord[symbol])
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
            Debug.Log(symbol);

            if (symbol == morse_xorWord.Length)
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
            switch (morse_xorWord[symbol])
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
            Debug.Log(symbol);
            yield return new WaitForSeconds(0.5f);

            if (symbol == morse_xorWord.Length)
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
