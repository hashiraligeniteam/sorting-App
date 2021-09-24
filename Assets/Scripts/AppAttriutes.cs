using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppAttriutes : MonoBehaviour
{
    public string ColorName;
    public string AppCatagory;
    public string Creator;
    public string AppName;
    public Text nameText;

    public GameObject HighLighter;
    public Text AppsCountText;
    public GameObject AppsCountImage;
    public Sprite MainSPrite;
    public Sprite HighliterSprite;
    public void ShowText()
    {
        nameText.text = AppName;
    }
}
