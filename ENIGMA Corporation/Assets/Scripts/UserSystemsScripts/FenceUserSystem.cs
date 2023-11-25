using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PlainWords;

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
    [Header("-------------------------------------------------")]
    [Header("Other")]
    [SerializeField] private Slider ProgressBar;
    [SerializeField] private Image FenceAppIcon;

    int setFenceLevel;
    public static string wordFence;         //Answer which need to be given
    public static string fencedWord;        //Encrypted answer word container
    public static int decryptedMessagesLevelOne = 0;
    int goal = 7;
    int hideTimer;

    void Start()
    {
        FenceAppIcon.GetComponent<Button>().enabled = true;
        FenceApp.SetActive(false);
        TimePanel.SetActive(false);
        ProgressBar.value = (float)decryptedMessagesLevelOne / (float)goal; 
        ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";

        if (wordFence == null || wordFence == "")
            FenceEncryptFunction();
        else
            CipheredText.text = fencedWord.ToUpper();
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
        if(PlainWords.FenceWords.Count > 0)
        {
            int number = Random.Range(0, PlainWords.FenceWords.Count - 1);
            wordFence = PlainWords.FenceWords[number];
            PlainWords.FenceWords.RemoveAt(number);
            int randomLevel = Random.Range(3, 6);
            fencedWord = "";

            int index;
            int[] jump = { (randomLevel - 3) + randomLevel, -1 };
            int jumpOrder = 0;
            bool swap = false;

            for (int f = 0; f < randomLevel; f++)
            {
                index = f;
                while (index <= wordFence.Length - 1)
                {
                    fencedWord += wordFence[index];
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
            CipheredText.text = fencedWord.ToUpper();
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
                    fenceVisual += "\t\t";
                index = f;
                while (index <= drawForWord.Length - 1)
                {
                    fenceVisual += drawForWord[pointer];
                    pointer++;
                    index += jump[jumpOrder] + 1;
                    if(index <= drawForWord.Length - 1)
                        for (int tab = 0; tab < jump[jumpOrder] + 1; tab++)
                            fenceVisual += "\t\t";
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
        if (Solution.text.ToLower() == wordFence)
        {
            FenceEncryptFunction();
            hideTimer = 2;
            StartCoroutine(ShowResult());
            ResultMessage.color = Color.green;
            ResultMessage.text = "Correct";
            decryptedMessagesLevelOne++;
            if (decryptedMessagesLevelOne <= goal)
            {
                ProgressBar.value = (float)decryptedMessagesLevelOne / (float)goal;
                ProgressValue.text = Mathf.RoundToInt(ProgressBar.value * 100).ToString() + "%";
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