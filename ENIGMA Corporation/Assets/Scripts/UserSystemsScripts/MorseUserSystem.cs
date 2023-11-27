using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    //Dot => 1, Dash => 2, Break => 3, End => 4
    Dictionary<char, string> MorseTable = new Dictionary<char, string>() { {'a', "12"}, {'b', "2111"}, {'c', "2121"}, {'d', "211"}, {'e', "1"}, {'f', "1121"}, {'g', "221"}, {'h', "1111"}, {'i', "11"}, {'j', "1222"}, {'k', "212"}, {'l', "1211"}, {'m', "22"}, {'n', "21"}, {'o', "222"}, {'p', "1221"}, {'q', "2212"},{'r', "121"}, {'s', "111"}, {'t', "2"}, {'u', "112"}, {'v', "1112"}, {'w', "122"}, {'x', "2112"}, {'y', "2122"}, {'z', "2211"}};
    
    int[] XORKeys = new int[4] {83, 30, 97, 72};         //RED | GREEN | BLUE | YELLOW
    public static string wordMorse_XOR;     //Answer which need to be given
    public static string morse_xorWord;     //Encrypted answer word container
    public static int decryptedMessagesLevelThree = 0;
    int goal = 8;
    int hideTimer;

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

/*        string abcdef = "daniel";
        foreach (char x in abcdef)
            Debug.Log(MorseTable[x]);

        char letterka = MorseTable.FirstOrDefault(x => x.Value == "211").Key;
        Debug.Log(letterka);
        letterka = MorseTable.FirstOrDefault(x => x.Value == "12").Key;
        Debug.Log(letterka);
        letterka = MorseTable.FirstOrDefault(x => x.Value == "21").Key;
        Debug.Log(letterka);
        letterka = MorseTable.FirstOrDefault(x => x.Value == "11").Key;
        Debug.Log(letterka);
        letterka = MorseTable.FirstOrDefault(x => x.Value == "1").Key;
        Debug.Log(letterka);
        letterka = MorseTable.FirstOrDefault(x => x.Value == "1211").Key;
        Debug.Log(letterka);*/
    }

    public void OpenMorseApp()
    {
        MorseAppIcon.GetComponent<Button>().enabled = false;
        MorseApp.SetActive(true);
    }

    public void CloseMorseApp()
    {
        MorseApp.SetActive(false);
        MorseAppIcon.GetComponent<Button>().enabled = true;
    }

    public void OpenXORApp()
    {
        XORAppIcon.GetComponent<Button>().enabled = false;
        XORApp.SetActive(true);
    }

    public void CloseXORApp()
    {
        XORApp.SetActive(false);
        XORAppIcon.GetComponent<Button>().enabled = true;
    }

    public void Morse_XOR_EncryptFunction()
    {
        if (PlainWords.XOR_Morse_WordDataBase.Count > 0)
        {
            int number = Random.Range(0, PlainWords.XOR_Morse_Words.Count - 1);
            wordMorse_XOR = PlainWords.XOR_Morse_Words[number];
            PlainWords.XOR_Morse_Words.RemoveAt(number);
            morse_xorWord = "";

            if (PlainWords.MorseCodes > 0 && PlainWords.XORCodes > 0)
            {
                int choice = Random.Range(0, 1);
                if (choice == 0)
                    MorseAlgorithm();
                else
                    XORAlgorithm();
            }
            else if (PlainWords.MorseCodes == 0 && PlainWords.XORCodes > 0)
                XORAlgorithm();
            else if (PlainWords.MorseCodes > 0 && PlainWords.XORCodes == 0)
                MorseAlgorithm();
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
        char letter;

        int index = 0;
        while (index <= wordMorse_XOR.Length - 1)
        {
            index++;
        }
        PlainWords.MorseCodes--;
        CipheredTextLength.text = morse_xorWord.Length.ToString();
    }

    public void XORAlgorithm()
    {
        int key = Random.Range(1, 4);
        char letter;

        int index = 0;
        while (index <= wordMorse_XOR.Length - 1)
        {
            index++;
        }
        PlainWords.AtBashCodes--;
        CipheredTextLength.text = morse_xorWord.Length.ToString();
    }

    public void SendSolutionReport()
    {
        if (Solution.text.ToLower() == wordMorse_XOR)
        {
            Morse_XOR_EncryptFunction();
            hideTimer = 2;
            StartCoroutine(ShowResult());
            ResultMessage.color = Color.green;
            ResultMessage.text = "Correct";
            decryptedMessagesLevelThree++;
            if (decryptedMessagesLevelThree <= goal)
            {
                ProgressBar.value = (float)decryptedMessagesLevelThree / (float)goal;
                ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

                if (decryptedMessagesLevelThree == goal)
                {
                    PlainWords.MorseCodes = 6;
                    PlainWords.XORCodes = 6;
                    Morse_XOR_EncryptFunction();
                }
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
    
    /*        string a = "abcde";
            byte b = (byte)(a[0] ^ 50);
            int c = (int)b;
            Debug.Log(b);
            Debug.Log(c);*/
}
