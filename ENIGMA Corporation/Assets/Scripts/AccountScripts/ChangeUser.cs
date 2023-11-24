using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeUser : MonoBehaviour
{
    string DevModePasswordNoobie = "admin123@";

    [SerializeField] private GameObject AllUsersPanel;
    [SerializeField] private GameObject ChosenUserPanel;

    [SerializeField] private Image[] User = new Image[5];
    [SerializeField] private Image ActiveUser;
    [SerializeField] private InputField Password;

    [SerializeField] private Text UserName;
    [SerializeField] private Text PasswordMessage;

    public int hideTimer = 0;

    void Start()
    {
        AllUsersPanel.SetActive(true);
        ChosenUserPanel.SetActive(false);
    }

    public void UserScreen(GameObject PressedAvatar)
    {
        AllUsersPanel.SetActive(false);
        ChosenUserPanel.SetActive(true);
        UserName.text = PressedAvatar.name;
        ActiveUser.sprite = PressedAvatar.GetComponent<Image>().sprite;
    }

    public void LogInButton()
    {
        //In Following Version here need to be added verification if password
        //entered by user is correct
        //For now it's only testing version for prototype
        if (DevModePasswordNoobie == Password.text)
            SceneManager.LoadSceneAsync("Level1");
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
