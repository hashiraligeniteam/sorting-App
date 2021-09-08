using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public bool Dargging = false;
    public List<GameObject> Apps;
    public GameObject AppsMainParent;
    public Vector3 finalAppPosition;

    private void Awake()
    {
        Instance = this;
    }
  
    int DrageObjectIndex;
    int ColliedObjectindex;

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

    public void CheckScore()
    {
        for (int i = 0; i <Apps.Count; i++)
        {
            ScoreByName(i);
        }
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
                    Debug.Log("ScoreAdded");
                    break;
                }
                else if (currentTemp[i] < nextTemp[i])
                {
                    //Score boss 
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
                    Debug.Log("ScoreDeducted");
                    break;
                }
                else if (currentTemp[i] < nextTemp[i])
                {
                    //score added
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
                    Debug.Log("ScoreAdded");
                    break;
                }
                else if (currentTemp[i] < nextTemp[i])
                {
                    //add deducted
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
                    Debug.Log("ScoreDeducted");
                    break;
                }
                else if (currentTemp[i] < nextTemp[i])
                {
                    //score added
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
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                Debug.Log("ScoreDeducted");
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            string nextColor = Apps[index + 1].GetComponent<AppAttriutes>().AppName;
            char[] nextTemp = nextColor.ToCharArray();
            Debug.Log("---------------------Next :" + nextColor);

            if (currentColor==nextColor)
            {
                //Okay boss
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                Debug.Log("ScoreDeducted");
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            string nextColor = Apps[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().AppName;
            Debug.Log("---------------------Top :" + nextColor);

            if (currentColor==nextColor)
            {
                //Okay boss
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                Debug.Log("ScoreDeducted");
            }
        }
        
        //Checking Bottom
        if (index < (Apps.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            string nextColor = Apps[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().AppName;
            Debug.Log("---------------------Bottom :" + nextColor);

            if (currentColor==nextColor)
            {
                //Okay boss
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                Debug.Log("ScoreDeducted");
            }
        }
    }
    
    public void ScoreByCreator(int index)
    {
        string currentCreator = Apps[index].GetComponent<AppAttriutes>().Creator;
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            string nextCreator = Apps[index - 1].GetComponent<AppAttriutes>().Creator;
            Debug.Log("---------------------Previous :" + nextCreator);

            if (currentCreator==nextCreator)
            {
                //Okay boss
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
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
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
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
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
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
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                Debug.Log("ScoreDeducted");
            }
        }
    }
    
    public void ScoreByCategory(int index)
    {
        string currentCategory = Apps[index].GetComponent<AppAttriutes>().AppCatagory;
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            string nextCategory = Apps[index - 1].GetComponent<AppAttriutes>().Creator;
            Debug.Log("---------------------Previous :" + nextCategory);

            if (currentCategory==nextCategory)
            {
                //Okay boss
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                Debug.Log("ScoreDeducted");
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            string nextCategory = Apps[index + 1].GetComponent<AppAttriutes>().Creator;
            char[] nextTemp = nextCategory.ToCharArray();
            Debug.Log("---------------------Next :" + nextCategory);

            if (currentCategory==nextCategory)
            {
                //Okay boss
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                Debug.Log("ScoreDeducted");
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            string nextCategory = Apps[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().Creator;
            Debug.Log("---------------------Top :" + nextCategory);

            if (currentCategory==nextCategory)
            {
                //Okay boss
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                Debug.Log("ScoreDeducted");
            }
        }
        
        //Checking Bottom
        if (index < (Apps.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            string nextCategory = Apps[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>().Creator;
            Debug.Log("---------------------Bottom :" + nextCategory);

            if (currentCategory==nextCategory)
            {
                //Okay boss
                Debug.Log("ScoreAdded");
            }
            else 
            {
                //Score boss 
                Debug.Log("ScoreDeducted");
            }
        }
    }
}
