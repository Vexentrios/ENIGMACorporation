using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PlainWords;
using static AccessEnigmaScript;

public class FenceUserSystem : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject FenceApp;
    [SerializeField] private GameObject TimePanel;
    [Header("-------------------------------------------------")]
    [Header("Buttons")]
    [SerializeField] private Button[] FenceLevelsButton = new Button[4];
    [SerializeField] private Button BuildFenceButton;
    [SerializeField] private Button CloseFenceAppButton;
    [SerializeField] private Button SolveReportButton;
    [SerializeField] private Button LogoutButton;
    [Header("-------------------------------------------------")]
    [Header("InputFields")]
    [SerializeField] private InputField EncryptedMessage;
    [SerializeField] private InputField Solution;
    [Header("-------------------------------------------------")]
    [Header("Texts")]
    [SerializeField] private Text ProgressValue;
    [SerializeField] private Text ResultMessage;
    [SerializeField] private Text Fence;
    [SerializeField] private Text CipheredText;
    [SerializeField] private Text AccountPassword;
    [Header("-------------------------------------------------")]
    [Header("Other")]
    [SerializeField] private Slider ProgressBar;
    [SerializeField] private Image FenceAppIcon;

    int setFenceLevel;
    public static string Level1Answer;         //Answer which need to be given
    public static string Level1EncryptedWord;        //Encrypted answer word container
    int goal = 7;
    int hideTimer;

    void Start()
    {
        if (AccessEnigmaScript.RookieAccessGranted != true)
            AccessEnigmaScript.RookieAccessGranted = true;

        if (AccessEnigmaScript.AmateurAccessGranted != true && AccessEnigmaScript.Level1Completed == true)
            AccountPassword.gameObject.SetActive(true);

        FenceAppIcon.GetComponent<Button>().enabled = true;
        FenceApp.SetActive(false);
        TimePanel.SetActive(false);
        ProgressBar.value = (float)AccessEnigmaScript.decryptedMessagesLevelOne / (float)goal; 
        ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

        if (Level1Answer == null || Level1Answer == "")
            FenceEncryptFunction();
        else
            CipheredText.text = Level1EncryptedWord.ToUpper();
    }

    public void OpenFenceBreaker()
    {
        FenceAppIcon.GetComponent<Button>().enabled = false;
        FenceApp.SetActive(true);
        ChangeFenceLevel(0);
        EncryptedMessage.text = "";
        Fence.text = "";
    }

    public void CloseFenceBreaker()
    {
        FenceApp.SetActive(false);
        FenceAppIcon.GetComponent<Button>().enabled = true;
    }

    public void ChangeFenceLevel(int levels)
    {
        setFenceLevel = levels + 3;
        for(int i = 0; i < 4; i++)
            FenceLevelsButton[i].interactable = (i == levels ? false : true);
    }

    public void FenceEncryptFunction()
    {
        ///THIS CONTENT IS MOVED TO FINAL VERSION
        //if(PlainWords.FenceWords.Count > 0)
        if (PlainWords.FenceWords.Count > 0 && AccessEnigmaScript.decryptedMessagesLevelOne != goal)
        {
            int number = Random.Range(0, PlainWords.FenceWords.Count);
            Level1Answer = PlainWords.FenceWords[number];
            PlainWords.FenceWords.RemoveAt(number);
            int randomLevel = Random.Range(3, 7);
            Level1EncryptedWord = "";

            int index;
            int[] jump = { (randomLevel - 3) + randomLevel, -1 };
            int jumpOrder = 0;
            bool swap = false;

            for (int f = 0; f < randomLevel; f++)
            {
                index = f;
                while (index <= Level1Answer.Length - 1)
                {
                    Level1EncryptedWord += Level1Answer[index];
                    index += jump[jumpOrder] + 1;
                    if (swap)
                        jumpOrder = (jumpOrder == 0 ? 1 : 0);
                }
                jump[0] -= 2;
                jump[1] += 2;
                jumpOrder = 0;
                if (jump[0] > 0 && jump[1] > 0 && swap == false)
                    swap = true;
                else if (jump[0] < 0 && jump[1] > 0)
                {
                    swap = false;
                    jumpOrder = 1;
                }
            }
            CipheredText.text = Level1EncryptedWord.ToUpper();
        }
        else
        {
            CipheredText.text = "All Solved";
            Solution.enabled = false;
            SolveReportButton.enabled = false;
        }
    }

    public void FenceDraw()
    {
        if(EncryptedMessage.text.Length > 0)
        {
            string fenceVisual = "";
            string drawForWord = EncryptedMessage.text.ToUpper();
            int index;
            int pointer = 0;
            int[] jump = {(setFenceLevel - 3) + setFenceLevel, -1};
            int jumpOrder = 0;
            bool swap = false;

            for (int f = 0; f < setFenceLevel; f++)
            {
                for (int tab = 0; tab < f; tab++)
                    fenceVisual += "  ";
                index = f;
                while (index <= drawForWord.Length - 1)
                {
                    fenceVisual += drawForWord[pointer];
                    pointer++;
                    index += jump[jumpOrder] + 1;
                    if(index <= drawForWord.Length - 1)
                        for (int tab = 0; tab < jump[jumpOrder] + 1; tab++)
                            fenceVisual += tab == 0 ? " " : "  ";
                    if (swap)
                        jumpOrder = (jumpOrder == 0 ? 1 : 0);
                }
                fenceVisual += "\n";
                jump[0] -= 2;
                jump[1] += 2;
                jumpOrder = 0;
                if (jump[0] > 0 && jump[1] > 0 && swap == false)
                    swap = true;
                else if (jump[0] < 0 && jump[1] > 0)
                {
                    swap = false;
                    jumpOrder = 1;
                }
            }
            for (int missingLines = setFenceLevel; missingLines <= 6; missingLines++)
                fenceVisual += "\n";
            Fence.text = fenceVisual;
        }
    }

    public void SendSolutionReport()
    {
        if (Solution.text.ToLower() == Level1Answer)
        {
            hideTimer = 2;
            StartCoroutine(ShowResult());
            ResultMessage.color = Color.green;
            ResultMessage.text = "Correct";
            AccessEnigmaScript.decryptedMessagesLevelOne++;
            FenceEncryptFunction();
            if (AccessEnigmaScript.decryptedMessagesLevelOne <= goal)
            {
                ProgressBar.value = (float)AccessEnigmaScript.decryptedMessagesLevelOne / (float)goal;
                ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

                if (AccessEnigmaScript.decryptedMessagesLevelOne == goal)
                {
                    AccessEnigmaScript.Level1Completed = true;
                    AccountPassword.gameObject.SetActive(true);
                }

                ///THIS CONTENT IS MOVED TO FINAL VERSION
                //if (decryptedMessagesLevelThree == goal)
                //{
                //    goal = 11;
                //    FenceEncryptFunction();
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