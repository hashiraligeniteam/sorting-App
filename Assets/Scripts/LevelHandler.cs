using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreState
{
    None,
    Name,
    Color,
    Category,
    Creator
}

[System.Serializable]
public class AppsAttributes 
{
    public Sprite AppImage;
    public string AppName;
    public string CompanyName;
    public string AppColor;
    public string Catagory;
   
}


[System.Serializable]
public class Levels 
{
    public int LevelNo;
    public string Description;
    public AppsAttributes[] Apps;
    public bool MakeFolder;
    public int row;
    public int col;
    public ScoreState[] scoreSequence;
}
public class LevelHandler : MonoBehaviour
{
    public Levels[] _levels;
    public static LevelHandler Instance;

    public void Awake()
    {
        Instance = this;
    }
}
