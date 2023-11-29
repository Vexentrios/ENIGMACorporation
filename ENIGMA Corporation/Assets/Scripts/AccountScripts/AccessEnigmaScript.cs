using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessEnigmaScript : MonoBehaviour
{
    public static int decryptedMessagesLevelOne;
    public static bool Level1Completed;
    public static int decryptedMessagesLevelTwo;
    public static bool Level2Completed;
    public static int decryptedMessagesLevelThree;
    public static bool Level3Completed;
    public static int decryptedMessagesLevelFour;
    public static bool Level4Completed;

    public static bool RookieAccessGranted;
    public static bool AmateurAccessGranted;
    public static bool ProfessionalAccessGranted;
    public static bool EliteAccessGranted;

    //Those Passwords for Prototype Version are pre-made
    //In final version, probably they will be randomized in their own ways
    public static string RookiePassword = "FenceCipher";
    public static string AmateurPassword = "BelovedCaesar753";
    public static string ProfessionalPassword = "X@12_M0r5e";
    public static string ElitePassword = "V1@en#PL47!AdvX";
}
