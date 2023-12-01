using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static AccessEnigmaScript;

public class ChangeUser : MonoBehaviour
{
    [SerializeField] private GameObject AllUsersPanel;
    [SerializeField] private GameObject ChosenUserPanel;

    [SerializeField] private Image[] User = new Image[5];
    [SerializeField] private Image ActiveUser;
    [SerializeField] private InputField Password;

    [SerializeField] private Text UserName;
    [SerializeField] private Text PasswordMessage;
    [SerializeField] private Text RookiePasswordMessage;

    [SerializeField] private Button EnigmaProtocolButton;

    int hideTimer = 0;
    string user;

    void Start()
    {
        if (AccessEnigmaScript.RookieAccessGranted != true)
            RookiePasswordMessage.gameObject.SetActive(true);
        if (AccessEnigmaScript.Level4Completed == true)
            EnigmaProtocolButton.gameObject.SetActive(true);

        AllUsersPanel.SetActive(true);
        ChosenUserPanel.SetActive(false);
    }

    public void AccountScreen(GameObject PressedAvatar)
    {
        user = PressedAvatar.name;
        switch (user)
        {
            case "Rookie":
                if (AccessEnigmaScript.RookieAccessGranted == true)
                    SceneManager.LoadSceneAsync("Level1");
                else
                    ShowLoginScreen(PressedAvatar);
                break;

            case "Amateur":
                if (AccessEnigmaScript.AmateurAccessGranted == true)
                    SceneManager.LoadSceneAsync("Level2");
                else
                    ShowLoginScreen(PressedAvatar);
                break;
            
            case "Professional":
                if (AccessEnigmaScript.ProfessionalAccessGranted == true)
                    SceneManager.LoadSceneAsync("Level3");
                else
                    ShowLoginScreen(PressedAvatar);
                break;

            case "Elite":
                if (AccessEnigmaScript.EliteAccessGranted == true)
                    SceneManager.LoadSceneAsync("Level4");
                else
                    ShowLoginScreen(PressedAvatar);
                break;
            case "CEO":
                ShowLoginScreen(PressedAvatar);
                break;
        }
    }

    public void ShowLoginScreen(GameObject PressedAvatar)
    {
        AllUsersPanel.SetActive(false);
        ChosenUserPanel.SetActive(true);
        UserName.text = PressedAvatar.name;
        ActiveUser.sprite = PressedAvatar.GetComponent<Image>().sprite;
    }

    public void LogInButton()
    {
        //This is for now solution (as long as I won't find a better way to do it)
        
        //LEVEL1
        if (UserName.text == "Rookie" && AccessEnigmaScript.RookiePassword == Password.text)
            SceneManager.LoadSceneAsync("Level1");
        
        //LEVEL2
        else if(UserName.text == "Amateur" && AccessEnigmaScript.Level1Completed == true && AccessEnigmaScript.AmateurPassword == Password.text)
            SceneManager.LoadSceneAsync("Level2");

        //LEVEL3
        else if(UserName.text == "Professional" && AccessEnigmaScript.Level2Completed == true && AccessEnigmaScript.ProfessionalPassword == Password.text)
            SceneManager.LoadSceneAsync("Level3");
        
        //LEVEL4
        else if (UserName.text == "Elite" && AccessEnigmaScript.Level3Completed == true && AccessEnigmaScript.ElitePassword == Password.text)
            SceneManager.LoadSceneAsync("Level4");
        
        //WRONG PASSWORD
        else
        {
            Password.text = "";
            hideTimer = 3;
            if (!PasswordMessage.IsActive())
            {
                PasswordMessage.gameObject.SetActive(true);
                StartCoroutine(ShowWarning());
            }
        }
    }

    public void ReturnButton()
    {
        AllUsersPanel.SetActive(true);
        ChosenUserPanel.SetActive(false);
        if (!PasswordMessage.IsActive())
        {
            PasswordMessage.gameObject.SetActive(false);
            StopCoroutine(ShowWarning());
        }
    }

    public void FinalLevelButton() =>
        SceneManager.LoadSceneAsync("ENIGMA_Scene");

    private IEnumerator ShowWarning()
    {
        while(hideTimer > 0)
        {
            yield return new WaitForSeconds(1);
            hideTimer--;
        }

        PasswordMessage.gameObject.SetActive(false);
        yield break;
    }
}
