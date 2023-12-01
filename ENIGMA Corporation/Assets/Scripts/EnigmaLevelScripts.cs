using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Sprite[] RotorsSprites = new Sprite[6];

    int[] index = new int[2];
    List<char> switchBoard = new List<char> {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    
    int[] switcherLetterIndex = new int[2];

    List<char> revertRoller = new List<char> { 'Y', 'R', 'U', 'H', 'Q', 'S', 'L', 'D', 'P', 'X', 'N', 'G', 'O', 'K', 'M', 'I', 'E', 'B', 'F', 'Z', 'C', 'W', 'V', 'J', 'A', 'T' };

    string[] RotorsAlphabets = { "EKMFLGDQVZNTOWYHXUSPAIBRCJ", "AJDKSIRUXBLHWTMCQGZNPYFVOE", "BDFHJLCPRTXVZNYEIWGAKMUSQO", "ESOVPZJAYQUIRHXLNFTGKDCMWB", "VZBRGITYUPSDNHLXAWMJQOFECK" };

    int[] rotateRotor = { 17, 5, 22, 10, 0 };

    string alphabetDefault = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string alphabetRemains;

    int[] chosenRotor = new int[3];
    int[] currentRotorPosition = new int[3];
    bool locked;
    bool instructionsOnScreen;
    int hideTimer;

    void Start()
    {
        EnigmaPanel.SetActive(true);
        InstructionsPanel.SetActive(false);
        instructionsOnScreen = false;
        RotorsOpened.SetActive(false);
        RotorsClosed.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            RotorsChoiceButtons[i].interactable = false;
            chosenRotor[i] = -1;
            ActiveRotors[i].sprite = RotorsSprites[0];
        }
        CheckKeyboardBlockade();
        ClearSwitchboard();
        EncryptedFinalWord.text = "MUAKVJFWEVDNTAUAQ";
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
            int swapLine = chosenRotor[rotorPosition] + 1;
            for (int o = 0; o < 3; o++)
            {
                RotorsChoiceButtons[swapLine * 3 + o].gameObject.SetActive(true);
                RotorsChoiceButtons[swapLine * 3 + o].interactable = true;
            }
        }
        for (int o = 0; o < 3; o++)
            RotorsChoiceButtons[rotorLine * 3 + o].gameObject.SetActive(false);

        ActiveRotors[rotorPosition].sprite = RotorsSprites[rotorLine];
        RotorsChoiceButtons[rotorPosition].interactable = true;
        RotorsChoiceButtons[pressed].interactable = false;
        RotorsChoiceButtons[pressed].gameObject.SetActive(true);
        chosenRotor[rotorPosition] = rotorLine - 1;

        CheckKeyboardBlockade();
    }

    public void RemoveRotor(int pressed)
    {
        int rotorLine = chosenRotor[pressed] + 1;
        RotorsChoiceButtons[pressed].interactable = false;
        chosenRotor[pressed] = -1;
        ActiveRotors[pressed].sprite = RotorsSprites[0];
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
        SwitchBoard.text += alphabetRemains[switcherLetterIndex[0]].ToString() + " <-> " + alphabetRemains[switcherLetterIndex[1]].ToString() + "\n";

        char left = (char)(alphabetRemains[switcherLetterIndex[0]]);
        char right = (char)(alphabetRemains[switcherLetterIndex[1]]);
        alphabetRemains = alphabetRemains.Replace(left, '_').Replace(right, '_');
        alphabetRemains = alphabetRemains.Replace("_", "");

        index[0] = switchBoard.IndexOf(left);
        index[1] = switchBoard.IndexOf(right);
        switchBoard[index[0]] = right;
        switchBoard[index[1]] = left;

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
        switchBoard.Sort();
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
    //############################[ENIGMA MACHINE Part]#######################################
    //########################################################################################

    public void EnigmaMachine(string pressedKey)
    {
        currentRotorPosition[2] = (currentRotorPosition[2] + 1) > 25 ? 0 : currentRotorPosition[2] + 1;
        if (currentRotorPosition[2] == rotateRotor[chosenRotor[2]])
        {
            currentRotorPosition[1] = (currentRotorPosition[1] + 1) > 25 ? 0 : currentRotorPosition[1] + 1;
            if (currentRotorPosition[1] == rotateRotor[chosenRotor[1]])
            {
                currentRotorPosition[0]++;
                RotorPosition[0].text = (currentRotorPosition[0] + 1).ToString();
            }
            RotorPosition[1].text = (currentRotorPosition[1] + 1).ToString();
        }
        RotorPosition[2].text = (currentRotorPosition[2] + 1).ToString();

        char letter = pressedKey[0];
        int posIndexer;
        char result = (char)(switchBoard.IndexOf(letter) + 65);

        for (int ent = 2; ent >= 0; ent--)
        {
            //Enter Rotor from Right to Left
            result = RotorsAlphabets[chosenRotor[ent]][(result - 65 + currentRotorPosition[ent]) % 26];
            //Leave Rotor from Right to Left
            posIndexer = (result - 65 - currentRotorPosition[ent]) % 26;
            if (posIndexer < 0)
                posIndexer = 26 + posIndexer;
            result = alphabetDefault[posIndexer];
        }

        //(Reflector) Revert Enter and Leave
        posIndexer = (revertRoller[result - 65] - 65);
        //result = revertRoller[result - 65];

        for (int leav = 0; leav < 3; leav++)
        {
            //Enter Rotor from Left to Right
            result = alphabetDefault[(posIndexer + currentRotorPosition[leav]) % 26];
            //Leave Rotor from Left to Right
            posIndexer = (RotorsAlphabets[chosenRotor[leav]].IndexOf(result) - currentRotorPosition[leav]) % 26;
            if (posIndexer < 0)
                posIndexer = 26 + posIndexer;
        }

        result = switchBoard[posIndexer];
        EnigmaScreen.text += result.ToString();
    }
    
    public void SendSolutionReport()
    {
        if (AnswerField.text.ToUpper() == "ENIGMACORPORATION")
        {
            hideTimer = 2;
            StartCoroutine(ShowResult());
            ResultMessage.color = Color.green;
            ResultMessage.text = "Correct";
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
        AnswerField.text = "";
    }

    private IEnumerator ShowResult()
    {
        while (hideTimer > 0)
        {
            yield return new WaitForSeconds(1);
            hideTimer--;
        }

        ResultMessage.gameObject.SetActive(false);
        SceneManager.LoadSceneAsync("EndGameScreen");
        yield break;
    }

    public void SwapPanel()
    {
        if(instructionsOnScreen)
        {
            EnigmaPanel.SetActive(true);
            InstructionsPanel.SetActive(false);
            instructionsOnScreen = false;
        }
        else
        {
            EnigmaPanel.SetActive(false);
            InstructionsPanel.SetActive(true);
            instructionsOnScreen = true;
        }
    }
}
