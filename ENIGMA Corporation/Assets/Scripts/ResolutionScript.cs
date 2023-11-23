using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionScript : MonoBehaviour
{
    int[] widthValues = { 1280, 1600, 1920 };
    int[] heightValues = { 720, 900, 1080 };
    [SerializeField] private Dropdown ResolutionsSelector;
    [SerializeField] private Toggle FullscreenChecker;

    public void ChangeResolution() =>
        Screen.SetResolution(widthValues[ResolutionsSelector.value], heightValues[ResolutionsSelector.value], FullscreenChecker.isOn);

    public void FullScreenModeSwap() =>
        Screen.SetResolution(Screen.width, Screen.height, FullscreenChecker.isOn);
}
