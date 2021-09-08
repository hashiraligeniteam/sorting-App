using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public bool Dargging = false;
    public List<GameObject> Apps;
    public GameObject AppsMainParent;
    public Vector3 finalAppPosition;
    public float addScore = 1f;
    public float subScore = 0.5f;
    public Text scoreText;

    private void Awake()
    {
        Instance = this;
    }
  
    int DrageObjectIndex;
    int ColliedObjectindex;

    private void Start()
    {
        for (int i=0;i<AppsMainParent.transform.childCount;i++)
        {
            AppsMainParent.transform.GetChild(i).GetComponent<AppAttriutes>().AppName = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].AppName;
            AppsMainParent.transform.GetChild(i).GetComponent<AppAttriutes>().ColorName = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].AppColor;
            AppsMainParent.transform.GetChild(i).GetComponent<AppAttriutes>().AppCatagory = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].Catagory;
            AppsMainParent.transform.GetChild(i).GetComponent<AppAttriutes>().ShowText();
        }
        GameManager.Instance.score = 0;
    }

    public void DisableTriggers(GameObject clickedApp, GameObject collidedApp)
    {
        foreach (GameObject app in Apps)
        {
            if (app != clickedApp || app != collidedApp)
            {
                app.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
    public void EnableTriggers()
    {
        foreach (GameObject app in Apps)
        {
            app.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public void UpdateChildList(GameObject DrageObject, GameObject ColliedObject) 
    {
        ColliedObjectindex =  Apps.IndexOf(ColliedObject);
        DrageObjectIndex = Apps.IndexOf(DrageObject);
        ColliedObject.transform.SetSiblingIndex(DrageObjectIndex);
        DrageObject.transform.SetSiblingIndex(ColliedObjectindex);

        finalAppPosition = ColliedObject.transform.localPosition;
        
        UpdateListIndex();
    }
    public void UpdateListIndex()
    {
        Apps.Clear();
        for (int i=0;i<AppsMainParent.transform.childCount;i++)
        {
            Apps.Add(AppsMainParent.transform.GetChild(i).gameObject);
        }
        CheckScore();
    }

    private void AddScore()
    {
        GameManager.Instance.score += addScore;
    }
    private void DeductScore()
    {
//        GameManager.Instance.score -= subScore;
    }

    public void CheckScore()
    {
        for (int i = 0; i <Apps.Count; i++)
        {
            foreach (ScoreState scoreState in LevelHandler.Instance._levels[GameManager.Instance.LevelNo].scoreSequence)
            {
                switch (scoreState)
                {
                    case ScoreState.Name: 
                        ScoreByName(i);
                        break;
                    case ScoreState.Color:
                        ScoreByColor(i);
                        break;
                    case ScoreState.Category:
                        ScoreByCategory(i);
                        break;
                    case ScoreState.Creator:
                        ScoreByCreator(i);
                        break;
                }
            }
        }

        scoreText.text = GameManager.Instance.score.ToString();
    }

    public void ScoreByName(int index)
    {
        string currentColor = Apps[index].GetComponent<AppAttriutes>().AppName;
        char[] currentTemp = currentColor.ToCharArray();
        Debug.Log("---------------------Current :" + currentColor);
        
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            string nextName = Apps[index - 1].GetComponent<AppAttriutes>().AppName;
            char[] nextTemp = nextName.ToCharArray();
            Debug.Log("---------------------Previous :" + nextName);

            for (int i = 0; i < currentTemp.Length; i++)
            {
                //Debug.Log("-----CurrentFirst :" + currentTemp[i] + "-----NextFirst" + nextTemp[i]);
                if (currentTemp[i] > nextTemp[i])
                {
                    //Okay boss
                    AddScore();
                    Debug.Log("ScoreAdded");
                    break;
                }
                else if (currentTemp[i] < nextTemp[i])
                {
                    //Score boss 
                    DeductScore();
                    Debug.Log("ScoreDeducted");
                    break;
                }
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            string nextName = Apps[index + 1].GetComponent<AppAttriutes>().AppName;
            char[] nextTemp = nextName.ToCharArray();
            Debug.Log("---------------------Next :" + nextName);

            for (int i = 0; i < currentTemp.Length; i++)
            {
                if (currentTemp[i] > nextTemp[i])
                {
                    //score deducted
                    DeductScore();
                    Debug.Log("ScoreDeducted");
                    break;
                }
                else if (currentTemp[i] < nextTemp[i])
                {
                    //score added
                    AddScore();
                    Debug.Log("ScoreAdded");
                    break;
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            string nextName = Apps[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().AppName;
            char[] nextTemp = nextName.ToCharArray();
            Debug.Log("---------------------Top :" + nextName);

            for (int i = 0; i < currentTemp.Length; i++)
            {
                if (currentTemp[i] > nextTemp[i])
                {
                    //score added
                    AddScore();
                    Debug.Log("ScoreAdded");
                    break;
                }
                else if (currentTemp[i] < nextTemp[i])
                {
                    //add deducted
                    DeductScore();
                    Debug.Log("ScoreDeducted");
                    break;
                }
            }
        }
        
        //Checking Bottom
        if (index < (Apps.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            string nextName = Apps[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().AppName;
            char[] nextTemp = nextName.ToCharArray();
            Debug.Log("---------------------Bottom :" + nextName);

            for (int i = 0; i < currentTemp.Length; i++)
            {
                if (currentTemp[i] > nextTemp[i])
                {
                    //score deducted
                    DeductScore();
                    Debug.Log("ScoreDeducted");
                    break;
                }
                else if (currentTemp[i] < nextTemp[i])
                {
                    //score added
                    AddScore();
                    Debug.Log("ScoreAdded");
                    break;
                }
            }
        }
    }

    public void ScoreByColor(int index)
    {
        string currentColor = Apps[index].GetComponent<AppAttriutes>().ColorName;
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            string nextColor = Apps[index - 1].GetComponent<AppAttriutes>().ColorName;
            Debug.Log("---------------------Previous :" + nextColor);

            if (currentColor==nextColor)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            string nextColor = Apps[index + 1].GetComponent<AppAttriutes>().ColorName;
            char[] nextTemp = nextColor.ToCharArray();
            Debug.Log("---------------------Next :" + nextColor);

            if (currentColor==nextColor)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            string nextColor = Apps[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().ColorName;
            Debug.Log("---------------------Top :" + nextColor);

            if (currentColor==nextColor)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }
        
        //Checking Bottom
        if (index < (Apps.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            string nextColor = Apps[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().ColorName;
            Debug.Log("---------------------Bottom :" + nextColor);

            if (currentColor==nextColor)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }
    }
    
    public void ScoreByCreator(int index)
    {
        string currentCreator = Apps[index].GetComponent<AppAttriutes>().Creator;
        Debug.Log("---------------------Current :" + currentCreator);
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            string nextCreator = Apps[index - 1].GetComponent<AppAttriutes>().Creator;
            Debug.Log("---------------------Previous :" + nextCreator);

            if (currentCreator==nextCreator)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            string nextCreator = Apps[index + 1].GetComponent<AppAttriutes>().Creator;
            Debug.Log("---------------------Next :" + nextCreator);

            if (currentCreator==nextCreator)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            string nextCreator = Apps[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().Creator;
            Debug.Log("---------------------Top :" + nextCreator);

            if (currentCreator==nextCreator)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }
        
        //Checking Bottom
        if (index < (Apps.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            string nextCreator = Apps[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().Creator;
            Debug.Log("---------------------Bottom :" + nextCreator);

            if (currentCreator==nextCreator)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }
    }
    
    public void ScoreByCategory(int index)
    {
        string currentCategory = Apps[index].GetComponent<AppAttriutes>().AppCatagory;
        Debug.Log("---------------------Current :" + currentCategory);
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            string nextCategory = Apps[index - 1].GetComponent<AppAttriutes>().AppCatagory;
            Debug.Log("---------------------Previous :" + nextCategory);

            if (currentCategory==nextCategory)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            string nextCategory = Apps[index + 1].GetComponent<AppAttriutes>().AppCatagory;
            char[] nextTemp = nextCategory.ToCharArray();
            Debug.Log("---------------------Next :" + nextCategory);

            if (currentCategory==nextCategory)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            string nextCategory = Apps[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().AppCatagory;
            Debug.Log("---------------------Top :" + nextCategory);

            if (currentCategory==nextCategory)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }
        
        //Checking Bottom
        if (index < (Apps.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            string nextCategory = Apps[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().AppCatagory;
            Debug.Log("---------------------Bottom :" + nextCategory);

            if (currentCategory==nextCategory)
            {
                //Okay boss
                AddScore();
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                DeductScore();
                Debug.Log("ScoreDeducted");
            }
        }
    }
}
