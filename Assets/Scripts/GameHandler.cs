using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    [Header ("Apps List")]
    public List<GameObject> Apps;
    [Header("Phone Object")]
    public GameObject AppsMainParent;
    [HideInInspector]
    public Vector3 finalAppPosition;
    [Header("Score")]
    public float addScore = 1f;
    public float subScore = 0.5f;
    public Text scoreText;

    [Header("Prefab")]
    public GameObject folderPrefab;


    [Header("Grid Layout Multiplier")]
    public float GridMultiplier;

    public GameObject OpenFolderRef;
    public GameObject FolderScreen;

    [Header("Inside Folder List")]
    public List<GameObject> InsideFolderApps;
    public GameObject currentFolderGridClosed;
    [HideInInspector]
    public GameObject appPrefab;

    public bool appBeingused;
    private GameObject app;

    [Header("Screens")]
    public GameObject LevelCompleteScreen;
    public GameObject ProfileScreen;
    public GameObject MainScreen;
    [Header("Text")]
    public Text ObjectiveText;

    //public List<GameObject> Folders;


    int DrageObjectIndex;
    int ColliedObjectindex;
    
    public GameObject SwapableObject;
    public GameObject DraggedObject;

 

    public float MoveSpeed;
    public int MaxAppsInFolder;

    [Header("Profile")]
    public Sprite[] ProfileImages;
    public Image ProfileImage;
    public Image ProgressBar;

    [Header("InfoScreen")]
    public GameObject InfoScreen;
    public Text InfoScreenAppName;
    public Text InfoScreenColor;
    public Text InfoScreenCreator;
    public Text InfoScreenCategory;
    public Image APPImage;



    private void Awake()
    {
        Instance = this;
    }
  
    

    private void Start()
    {
        Debug.Log(GameManager.Instance.LevelNo + " MF level");
        if (GameManager.Instance.Next)
        {
            MainScreen.SetActive(false);
            ProfileScreen.SetActive(true);
        }
        if (GameManager.Instance.Restart)
        {
            Debug.Log("here Restart");
            MainScreen.SetActive(false);
            ProfileScreen.SetActive(false);
            Initiate();
        }
        GameManager.Instance.Next = false;
        GameManager.Instance.Restart = false;
    }
    public void Initiate()
    {
        ObjectiveText.text = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].ObjectiveText + " " +LevelHandler.Instance._levels[GameManager.Instance.LevelNo].ScoreToComplete;
       
        for (int i = 0; i < LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps.Length; i++)
        {
            app = Instantiate(appPrefab);
            app.transform.SetParent(AppsMainParent.transform);
            if (app.GetComponent<AppAttriutes>()) 
            {
                app.GetComponent<AppAttriutes>().AppName = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].AppName;
                app.GetComponent<AppAttriutes>().ColorName = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].AppColor;
                app.GetComponent<AppAttriutes>().AppCatagory = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].Catagory;
                app.GetComponent<AppAttriutes>().Creator = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].CompanyName;
                app.GetComponent<Image>().sprite = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].AppImage;
                app.GetComponent<AppAttriutes>().ShowText();
                app.GetComponent<AppAttriutes>().AppSprite = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].AppImage;
            }
            
            app.transform.localScale = Vector3.one;
            Apps.Add(app);
        }
        //Empty Cells Logic
        //if (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].DummyApps)
        //{
        //    int TotalRemainingApps = LevelHandler.Instance.TotalNumberAppsExitInScreen - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps.Length;
        //    Debug.Log(TotalRemainingApps + " Dummy Apps Count");
        //    for (int i = 0; i < TotalRemainingApps; i++)
        //    {
        //        app = Instantiate(appPrefab);
        //        app.transform.SetParent(AppsMainParent.transform);
        //        app.GetComponent<UIDrag>().IsEmpty = true;
        //        app.GetComponent<Image>().enabled = false;
                
        //        app.transform.localScale = Vector3.one;
        //        Apps.Add(app);
        //    }
        //}
        GameManager.Instance.score = 0;
    }

    public void DisableTriggers(GameObject clickedApp, GameObject collidedApp)
    {
        foreach (GameObject app in Apps)
        {
            if (app != clickedApp || app != collidedApp)
            {
                app.GetComponent<BoxCollider2D>().enabled = false;
                app.GetComponentInChildren<BoxCollider2D>().enabled = false;
            }
        }
    }
    public void EnableTriggers()
    {
        foreach (GameObject app in Apps)
        {
            app.GetComponent<BoxCollider2D>().enabled = true;
            app.GetComponentInChildren<BoxCollider2D>().enabled = true;
        }
    }

    public void UpdateChildList(GameObject DrageObject, GameObject ColliedObject) 
    {
      
        if (ColliedObject.GetComponent<TriggerCheck>().InsideFolder)
        {
          
            ColliedObjectindex = InsideFolderApps.IndexOf(ColliedObject);
            DrageObjectIndex = InsideFolderApps.IndexOf(DrageObject);
            for (int i = 0; i < OpenFolderRef.transform.childCount; i++)
            {
                DisableTriggers(OpenFolderRef.transform.GetChild(i).gameObject);
            }
        }
        else
        {

            ColliedObjectindex = Apps.IndexOf(ColliedObject);
            DrageObjectIndex = Apps.IndexOf(DrageObject);
            for (int i = 0; i < AppsMainParent.transform.childCount; i++)
            {
                DisableTriggers(AppsMainParent.transform.GetChild(i).gameObject);
            }
        }
    
    

        OpenFolderRef.GetComponent<GridLayoutGroup>().enabled = false;
        AppsMainParent.GetComponent<GridLayoutGroup>().enabled = false;
      
        int Diff = DrageObjectIndex - ColliedObjectindex;
        Vector3 SwapPos = SwapableObject.transform.localPosition;
        //DrageObject.transform.DOLocalMove(SwapableObject.transform.localPosition, MoveSpeed);
        ColliedObject.transform.SetSiblingIndex(DrageObjectIndex);
       
        //int DummyIndex = DrageObjectIndex;
        if (Mathf.Abs(Diff) != 1)
        {
            if (Diff < 0)
            {
                for (int i = DrageObjectIndex; i < ColliedObjectindex; i++)
                {
                    if (!ColliedObject.GetComponent<TriggerCheck>().InsideFolder)
                    {
                        AppsMainParent.transform.GetChild(i).transform.DOLocalMove(AppsMainParent.transform.GetChild(i + 1).transform.localPosition, MoveSpeed);
                    }
                    else
                    {
                        OpenFolderRef.transform.GetChild(i).transform.DOLocalMove(OpenFolderRef.transform.GetChild(i + 1).transform.localPosition, MoveSpeed);
                    }
                }
            }
            else
            {
                if (!ColliedObject.GetComponent<TriggerCheck>().InsideFolder)
                {
                    AppsMainParent.transform.GetChild(ColliedObjectindex).transform.DOLocalMove(finalAppPosition, MoveSpeed);
                    for (int i = ColliedObjectindex + 1; i < DrageObjectIndex; i++)
                    {

                        AppsMainParent.transform.GetChild(i).transform.DOLocalMove(AppsMainParent.transform.GetChild(i - 1).transform.localPosition, MoveSpeed);
                    }
                }
                else 
                {
                    if (OpenFolderRef.transform.childCount>0) {
                        OpenFolderRef.transform.GetChild(ColliedObjectindex).transform.DOLocalMove(finalAppPosition, MoveSpeed);
                        for (int i = ColliedObjectindex + 1; i < DrageObjectIndex; i++)
                        {

                            OpenFolderRef.transform.GetChild(i).transform.DOLocalMove(OpenFolderRef.transform.GetChild(i - 1).transform.localPosition, MoveSpeed);
                        }
                    }
                }
            }
            OpenFolderRef.GetComponent<GridLayoutGroup>().enabled = true;
            AppsMainParent.GetComponent<GridLayoutGroup>().enabled = true;
        }
        else 
        {
            
            // DrageObject.transform.SetSiblingIndex(ColliedObjectindex);
            DrageObject.transform.DOLocalMove(SwapPos, MoveSpeed);
            ColliedObject.transform.DOLocalMove(finalAppPosition,MoveSpeed).OnComplete( ()=> {
                OpenFolderRef.GetComponent<GridLayoutGroup>().enabled = true;
                AppsMainParent.GetComponent<GridLayoutGroup>().enabled = true;
            });
        }
        if (!ColliedObject.GetComponent<TriggerCheck>().InsideFolder)
        {
            for (int i = 0; i < OpenFolderRef.transform.childCount; i++)
            {
                DisableTriggers(OpenFolderRef.transform.GetChild(i).gameObject);
            }
            Debug.Log("here");
            for (int i = 0; i < AppsMainParent.transform.childCount; i++)
            {
                ActivateTriggers(AppsMainParent.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            StartCoroutine(Waiting());
            for (int i = 0; i < OpenFolderRef.transform.childCount; i++)
            {
                ActivateTriggers(OpenFolderRef.transform.GetChild(i).gameObject);
            }
        }
        UpdateListIndex(ColliedObject.GetComponent<TriggerCheck>().InsideFolder);
        ColliedObject.GetComponent<UIDrag>().Moving = false;
        DrageObject.GetComponent<UIDrag>().Moving = false;
    }
    IEnumerator Waiting() 
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < AppsMainParent.transform.childCount; i++)
        {
            DisableTriggers(AppsMainParent.transform.GetChild(i).gameObject);
        }
    }

    public void UpdateListIndex(bool InsideFolder)
    {
        Debug.Log(InsideFolder + "  Inside Folder Bool");
        if (InsideFolder)
        {
            InsideFolderApps.Clear();
            for (int i = 0; i < OpenFolderRef.transform.childCount; i++)
            {

                InsideFolderApps.Add(OpenFolderRef.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            Apps.Clear();
            for (int i = 0; i < AppsMainParent.transform.childCount; i++)
            {
                Apps.Add(AppsMainParent.transform.GetChild(i).gameObject);
            }
        }
       // CheckScore(InsideFolder);
    }

    public void CreateFolder(GameObject collidedApp,GameObject draggedApp, TriggerCheck tCheck)
    {
        GameObject folder = Instantiate(folderPrefab, AppsMainParent.transform);
        Debug.Log(Apps.IndexOf(draggedApp) + " Dragged "+ Apps.IndexOf(collidedApp));
        folder.transform.SetSiblingIndex(Apps.IndexOf(collidedApp));
        collidedApp.SetActive(false);
        draggedApp.SetActive(false);
        Apps[Apps.IndexOf(collidedApp)] = folder;
        Apps.Remove(draggedApp);

        DisableTriggers(collidedApp);
        DisableTriggers(draggedApp);
        collidedApp.transform.SetParent(folder.GetComponentInChildren<GridLayoutGroup>().transform);
        Destroy(collidedApp.GetComponent<UIDrag>());
        draggedApp.transform.SetParent(folder.GetComponentInChildren<GridLayoutGroup>().transform);
        Destroy(draggedApp.GetComponent<UIDrag>());

        collidedApp.SetActive(true);
        draggedApp.SetActive(true);
       // AssignGridSize(folder.GetComponentInChildren<GridLayoutGroup>(), true);
        tCheck.once = false;
      
    }

    public void AddInFolder(GameObject app,GameObject folder)
    {
        Apps.Remove(app);
        app.transform.SetParent(folder.GetComponent<TriggerCheck>().LayoutGroup.transform);
        Destroy(app.GetComponent<UIDrag>());
        DisableTriggers(app);

        if (folder.GetComponent<TriggerCheck>().LayoutGroup.transform.childCount > MaxAppsInFolder) 
        {
            app.SetActive(false);
            GameObject Obj = folder.GetComponent<TriggerCheck>().LayoutGroup.transform.GetChild(MaxAppsInFolder - 1).gameObject;
            Obj.GetComponent<Image>().enabled = false;
            Obj.GetComponent<AppAttriutes>().AppsCountImage.SetActive(true);
            Obj.GetComponent<AppAttriutes>().AppsCountText.text = "+ " + (folder.GetComponent<TriggerCheck>().LayoutGroup.transform.childCount - MaxAppsInFolder);
        }

        //if (folder.GetComponent<TriggerCheck>().LayoutGroup.transform.childCount >= MaxAppsInFolder)
        //{
        //    Debug.Log("Here");
        
        //}

        UpdateListIndex(true);
    }

    #region ScoreLogic
    
    private void AddScore()
    {
        GameManager.Instance.score += addScore;
    }
    private void DeductScore()
    {
//        GameManager.Instance.score -= subScore;
    }

    public void CheckScore(bool insideFolder)
    {
        GameManager.Instance.score = 0;
        List<GameObject> appList=new List<GameObject>();
        if (insideFolder)
        {
            appList = InsideFolderApps;
        }
        else
        {
            appList = Apps;
        }
        
        for (int i = 0; i < appList.Count; i++)
        {
            if (!appList[i].GetComponent<TriggerCheck>().Folder)
            {
                foreach (ScoreState scoreState in LevelHandler.Instance._levels[GameManager.Instance.LevelNo]
                    .scoreSequence)
                {
                    switch (scoreState)
                    {
                        case ScoreState.Name:
                            ScoreByName(i, appList);
                            break;
                        case ScoreState.Color:
                            ScoreByColor(i, appList);
                            break;
                        case ScoreState.Category:
                            ScoreByCategory(i, appList);
                            break;
                        case ScoreState.Creator:
                            ScoreByCreator(i, appList);
                            break;
                    }
                }
            }
        }
        GameManager.Instance.score = GameManager.Instance.score / 2;
        scoreText.text = GameManager.Instance.score.ToString();
        ProgressBar.fillAmount = (GameManager.Instance.score / LevelHandler.Instance._levels[GameManager.Instance.LevelNo].ScoreToComplete);
        StartCoroutine(Wait());
    }
    IEnumerator Wait() 
    {
        yield return new WaitForSeconds(0.5f);
        if (GameManager.Instance.score >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].ScoreToComplete)
        {
            LevelCompleteScreen.SetActive(true);
        }
    }
    //appList[index].GetComponent<LinkingRelation>().OnLeft(index);
    public void ScoreByName(int index,List<GameObject> appList)
    {
        string currentColor = appList[index].GetComponent<AppAttriutes>().AppName;
        char[] currentTemp = currentColor.ToCharArray();
      
        
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string previousName = appList[index - 1].GetComponent<AppAttriutes>().AppName;
                char[] nextTemp = previousName.ToCharArray();
              

                for (int i = 0; i < currentTemp.Length; i++)
                {
                  //  Debug.Log("-----CurrentFirst :" + currentTemp[i] + "-----NextFirst" + nextTemp[i]);
                    if (currentTemp[i] > nextTemp[i])
                    {
                     //   Debug.Log("Score By Name Added");
                        //Okay boss
                        AddScore();
                        appList[index].GetComponent<LinkingRelation>().OnLeft(index);
                      
                        break;
                    }
                    else if (currentTemp[i] < nextTemp[i])
                    {
                        //Score boss 
                        DeductScore();
                       
                        break;
                    }
                }
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            if ((index + 1 < appList.Count) && appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextName = appList[index + 1].GetComponent<AppAttriutes>().AppName;
                char[] nextTemp = nextName.ToCharArray();
               

                for (int i = 0; i < currentTemp.Length; i++)
                {
                    if (currentTemp[i] > nextTemp[i])
                    {
                        //score deducted
                        DeductScore();
                       
                        break;
                    }
                    else if (currentTemp[i] < nextTemp[i])
                    {
                        //score added
                        AddScore();
                        appList[index].GetComponent<LinkingRelation>().OnRight(index);
                        // Debug.Log("Score By Name Added");
                        break;
                    }
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            if (appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>())
            {
                string topName = appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().AppName;
                char[] nextTemp = topName.ToCharArray();
               

                for (int i = 0; i < currentTemp.Length; i++)
                {
                    if (currentTemp[i] > nextTemp[i])
                    {
                       // Debug.Log("Score By Name Added");
                        //score added
                        AddScore();
                        appList[index].GetComponent<LinkingRelation>().OnUp(index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col);
                        break;
                    }
                    else if (currentTemp[i] < nextTemp[i])
                    {
                        //add deducted
                        DeductScore();
                      
                        break;
                    }
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            if (appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string bottomName = appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().AppName;
                char[] nextTemp = bottomName.ToCharArray();
          

                for (int i = 0; i < currentTemp.Length; i++)
                {
                    if (currentTemp[i] > nextTemp[i])
                    {
                     //   Debug.Log("Score By Name Added");
                        //score deducted
                        DeductScore();
                     
                        break;
                    }
                    else if (currentTemp[i] < nextTemp[i])
                    {
                        //score added
                        AddScore();
                        appList[index].GetComponent<LinkingRelation>().OnDown(index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col);
                        break;
                    }
                }
            }
        }
    }

    public void ScoreByColor(int index,List<GameObject> appList)
    {
        string currentColor = appList[index].GetComponent<AppAttriutes>().ColorName;
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index - 1].GetComponent<AppAttriutes>().ColorName;
             

                if (currentColor == nextColor)
                {
                   // Debug.Log("Score By Name Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnLeft(index);
                }
                else
                {
                    //Score boss
                    DeductScore();
                    
                }
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            if (appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index + 1].GetComponent<AppAttriutes>().ColorName;
                char[] nextTemp = nextColor.ToCharArray();
            

                if (currentColor == nextColor)
                {
                   // Debug.Log("Score By Name Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnRight(index);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            if (appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().ColorName;
              

                if (currentColor == nextColor)
                {
                  //  Debug.Log("Score By Name Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnUp(index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                   
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            if (appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().ColorName;
              

                if (currentColor == nextColor)
                {
                   // Debug.Log("Score By Name Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnDown(index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col);
                }
                else
                {
                    //Score boss 
                    DeductScore();
               
                }
            }
        }
    }
    
    public void ScoreByCreator(int index,List<GameObject> appList)
    {
        string currentCreator = appList[index].GetComponent<AppAttriutes>().Creator;
       
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index - 1].GetComponent<AppAttriutes>().Creator;
               

                if (currentCreator == nextCreator)
                {
                   // Debug.Log("Score By Creator Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnLeft(index);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                   
                }
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            if (appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index + 1].GetComponent<AppAttriutes>().Creator;
             

                if (currentCreator == nextCreator)
                {
                   // Debug.Log("Score By Creator Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnRight(index);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                  
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            if (appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().Creator;
            

                if (currentCreator == nextCreator)
                {
                   // Debug.Log("Score By Creator Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnUp(index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                    
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            if (appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().Creator;
              

                if (currentCreator == nextCreator)
                {
                  //  Debug.Log("Score By Creator Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnDown(index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                    
                }
            }
        }
    }
    
    public void ScoreByCategory(int index,List<GameObject> appList)
    {
        string currentCategory = appList[index].GetComponent<AppAttriutes>().AppCatagory;
      
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index - 1].GetComponent<AppAttriutes>().AppCatagory;
              

                if (currentCategory == nextCategory)
                {
                   // Debug.Log("Score By Category Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnLeft(index);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                    
                }
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            if (appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index + 1].GetComponent<AppAttriutes>().AppCatagory;
                char[] nextTemp = nextCategory.ToCharArray();
             

                if (currentCategory == nextCategory)
                {
                   // Debug.Log("Score By Category Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnRight(index);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                 
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            if (appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().AppCatagory;
              

                if (currentCategory == nextCategory)
                {
                   // Debug.Log("Score By Category Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnUp(index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                  
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            if (appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().AppCatagory;
               

                if (currentCategory == nextCategory)
                {
                    //Debug.Log("Score By Category Added");
                    //Okay boss
                    AddScore();
                    appList[index].GetComponent<LinkingRelation>().OnDown(index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                   
                }
            }
        }
    }
    
    #endregion
    //public void AssignGridSize(GridLayoutGroup Grid,bool folder) 
    //{
    //    if (folder)
    //    {
    //        //Grid.cellSize = new Vector2(Grid.cellSize.x / GridMultiplier, Grid.cellSize.y / GridMultiplier);
    //        //Grid.padding.left = ( Grid.padding.left / (int)GridMultiplier);
    //        //Grid.padding.top = (Grid.padding.top / (int)GridMultiplier);
    //        //Grid.spacing = new Vector2(Grid.spacing.x / GridMultiplier, Grid.spacing.y / GridMultiplier);
    //    }
    //    //else 
    //    //{
    //    //    Grid.cellSize = new Vector2(Grid.cellSize.x * GridMultiplier, Grid.cellSize.y * GridMultiplier);
    //    //    Grid.padding.left = (Grid.padding.left * (int)GridMultiplier);
    //    //    Grid.padding.top = (Grid.padding.top * (int)GridMultiplier);
    //    //    Grid.spacing = new Vector2(Grid.spacing.x * GridMultiplier, Grid.spacing.y * GridMultiplier);
    //    //}
    //}
    public void ActivateTriggers(GameObject Obj) 
    {
        Debug.Log("activating Triggers");
        Obj.GetComponent<Collider2D>().enabled = true;
        Collider2D[] Col = Obj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in Col) 
        {
            c.enabled = true;
        }
    }

    public void DisableTriggers(GameObject Obj)
    {
        Obj.GetComponent<Collider2D>().enabled = false;
        BoxCollider2D[] Col = Obj.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D c in Col)
        {
            c.enabled = false;
        }
    }
    public void OnPlay()
    {
        MainScreen.SetActive(false);
        ProfileScreen.SetActive(true);
    }
    public void OnSelectProfile(int LeveLNo) 
    {
        GameManager.Instance.LevelNo = LeveLNo;
        ProfileImage.sprite = ProfileImages[LeveLNo];
        ProfileScreen.SetActive(false);
        Initiate();
    }
    public void OnRestart()
    {
        GameManager.Instance.Restart = true;
        SceneManager.LoadScene("Gameplay");
    }
    public void OnNext()
    {
        GameManager.Instance.Next = true;
        GameManager.Instance.LevelNo +=1;
        SceneManager.LoadScene("Gameplay");
    }

   public IEnumerator DestroyingObjects(GraphicRaycaster _RayCaster,Canvas _Canvas)
    {
        Destroy(_RayCaster);
        yield return new WaitForSeconds(0.15f);
        Destroy(_Canvas);
    }
    public void CloseRealtion() 
    {
        for (int i =0;i<Apps.Count;i++)
        {
            Apps[i].GetComponent<LinkingRelation>().Close();
        }
    }
    
}
