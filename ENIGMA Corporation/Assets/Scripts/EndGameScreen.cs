using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScreen : MonoBehaviour
{
    public void BacktoMenu()
    {
        PlainWords.FenceWords.Clear();
        PlainWords.Caesar_AtBash_Words.Clear();
        PlainWords.XOR_Morse_Words.Clear();
        PlainWords.ADFVGX_PlayFair_Vigenere_Words.Clear();
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
