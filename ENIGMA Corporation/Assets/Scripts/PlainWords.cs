using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlainWords : MonoBehaviour
{
    [Header("Fence Words - Level 1")]
    [Header("------------------------------------------------------")]
    
    public static List<string> FenceWordDataBase = new List<string> {"corporation","university", "security", "technophobia", "scopophobia", "administration", "disagreement", "acknowledgements", "disqualification", "ciphers", "technology"};

    public static List<string> FenceWords = new List<string> {};
    
    [Header("------------------------------------------------------")]
    [Header("AtBash and Caesar Words - Level 2")]
    [Header("------------------------------------------------------")]
    
    public static int AtBashCodes;
    public static int CaesarCodes;
    
    public static List<string> Caesar_AtBash_WordDataBase = new List<string> {"message", "rome", "history", "secret", "information", "past", "forum", "heroes", "chariot", "horses", "apollo", "ceres", "diana", "egeria", "hercules", "jupiter", "luna", "nemesis", "trivia", "voluptas"};
    
    public static List<string> Caesar_AtBash_Words = new List<string> {};
    
    [Header("------------------------------------------------------")]
    [Header("XOR and Morse Words - Level 3")]
    [Header("------------------------------------------------------")]
    
    public static int XORCodes;
    public static int MorseCodes;
    
    public static List<string> XOR_Morse_WordDataBase = new List<string> {};
    
    public static List<string> XOR_Morse_Words = new List<string> {};

    //public static List<string> PLACEHOLDER = new List<string> { };
}
