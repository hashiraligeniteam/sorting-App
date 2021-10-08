using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using GameAnalyticsSDK;


public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    [Header("Level Buttons")]
    public Button[] LevelButtons;
    public GameObject[] HighlighterImages;
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
    public Text BarObjectiveText;
    public Text ObjectiveRemainingText;
    public Text TotalMovesTaken;
    public Text TimeTaken;
    public Text MiniCompleteObjectivePanelText;

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

    public GameObject HintText;

    public Canvas _Canvas;
    public GameObject Canfetti;
    public GameObject MiniCompleteObjectivePanel;

    ScoreState scoreState;
    int stateIndex = 0;
    Coroutine _coroutine;
    bool once;
    Coroutine TotalTime;
    int timer;

    public GameObject[] Stars;
    public int CurrentMoves;
    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }
  
    

    private void Start()
    {
      
        if (GameManager.Instance.Next)
        {
            MainScreen.SetActive(false);
            ProfileScreen.SetActive(true);
        }
        if (GameManager.Instance.Restart)
        {
           
            MainScreen.SetActive(false);
            ProfileScreen.SetActive(false);
            Initiate();
        }
        GameManager.Instance.Next = false;
        GameManager.Instance.Restart = false;
      
        for (int i = 0; i<=PlayerPrefs.GetInt("UnlockLevel"); i++)
        {
            LevelButtons[i].interactable = true;
        }
        HighlighterImages[PlayerPrefs.GetInt("UnlockLevel")].SetActive(true);
    
    }
    public void Initiate()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start,GameManager.Instance.LevelNo.ToString());
        AnalyticsAdsManager.instance.RequestIronsourceInterstitialAd();
        if (GameManager.Instance.LevelNo==0)
        {
            HintText.SetActive(true);
        }
        ObjectiveText.text = LevelHandler.Instance.Config.GetObjectiveByLevel(GameManager.Instance.LevelNo);
        for (int i =0;i< LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing.Length; i++)
        {
            LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[i].COmpleted = false;
        }
        LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[0].COmpleted = false;
        for (int i = 0; i < LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].AppsName.Count; i++)
        {
            AppsAttributes AA = LevelHandler.Instance.Config.GetAppByName(LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].AppsName[i]);
            app = Instantiate(appPrefab);
            app.transform.SetParent(AppsMainParent.transform);
            AppAttriutes AAA = app.GetComponent<AppAttriutes>();
            if (AA !=null && AAA != null ) 
            {
                AAA.AppName = AA.AppName;
                AAA.ColorName = AA.AppColor;
                AAA.AppCatagory = AA.Catagory;
                AAA.Creator = AA.CompanyName;
                AAA.GetComponent<Image>().sprite = AA.AppImage;
                AAA.ShowText();
                AAA.AppSprite = AA.AppImage;
            }
            
            app.transform.localScale = Vector3.one;
            Apps.Add(app);
        }
        #region Epmty Cell
        //Empty Cells Logic
        //if (LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].DummyApps)
        //{
        //    int TotalRemainingApps = LevelHandler.Instance.TotalNumberAppsExitInScreen - LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Apps.Length;
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
        #endregion
        GameManager.Instance.score = 0;
        BarObjectiveText.text = (stateIndex + 1) + " / " + LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing.Length;
        ObjectiveRemainingText.text = LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[stateIndex].GetMiniObjective();
        MiniCompleteObjectivePanelText.text = LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[stateIndex].GetMiniObjective();



        CheckScore(false);
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
        CurrentMoves++;
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
 
    private void AddScore(ScoreState State)
    {
        if (State.ToString() == ScoreState.Name.ToString())
        {
            GameManager.Instance.score += addScore;
        }
        else 
        {
            GameManager.Instance.score += (addScore/2);
        }
        if (LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[stateIndex].ScoreToComplete <= GameManager.Instance.score)
        {
            if (!once)
            {
                once = true;
                LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[stateIndex].COmpleted = true;
            }
        }
        #region Comment Code
        //switch (State) 
        //{
        //    case ScoreState.Name:
        //     GameManager.Instance.score += addScore; 
        //        if (LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[stateIndex].ScoreToComplete <= GameManager.Instance.score)
        //        {

        //        }
        //        break;
        //    case ScoreState.Category:
        //        if (!LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].ByCatagoryAchieved)
        //        {
        //            CurrentScoreToCompleteByCatagory += (addScore / 2);
        //            if (CurrentScoreToCompleteByCatagory >= LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].ScoreToCompleteByCatagory)
        //            {
        //                LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].ByCatagoryAchieved = true;
        //            }
        //        }

        //        break;
        //    case ScoreState.Color:
        //        if (!LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].ByColorAchieved)
        //        {
        //            CurrentScoreToCompleteByColor += (addScore / 2);
        //            if (CurrentScoreToCompleteByColor >= LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].ScoreToCompleteByColor)
        //            {
        //                LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].ByColorAchieved = true;
        //            }
        //        }


        //        break;
        //    case ScoreState.Creator:
        //        if (!LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].ByCompanyNameAchieved)
        //        {
        //            CurrentScoreToCompleteByCompanyName += (addScore / 2);
        //            if (CurrentScoreToCompleteByCompanyName >= LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].ScoreToCompleteByCompanyName)
        //            {
        //                LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].ByCompanyNameAchieved = true;
        //            }
        //        }

        //        break;
        //}
        #endregion

    }
    private void DeductScore()
    {
//        GameManager.Instance.score -= subScore; 
    }

    public void CheckScore(bool insideFolder)
    {
        GameManager.Instance.score = 0;
        once = false;
        List<GameObject> appList = new List<GameObject>();
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
            Debug.Log("here");
            if (!appList[i].GetComponent<TriggerCheck>().Folder)
            {
                
                //foreach (ScoreState scoreState in LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo]
                //    .scoreSequence)
                for (int j=0;j<LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing.Length;j++ )
                {
                   
                    if (!LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[j].COmpleted) 
                    {
                        scoreState = LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[j].State;
                        stateIndex = j;
                        
                        break;
                    }
                }
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
     
        ProgressBar.fillAmount = GameManager.Instance.score / LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[stateIndex].ScoreToComplete;
        if (ProgressBar.fillAmount==1)
        {
            
            UpdateBarAndText();
        }
        if (GameManager.Instance.score >= LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[stateIndex].ScoreToComplete)
        {
            _coroutine = StartCoroutine(Wait());
        }
    }

    void UpdateBarAndText() 
    {
        MiniCompleteObjectivePanel.SetActive(true);
        if ((stateIndex) < LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing.Length)
        {
            ProgressBar.fillAmount = 0.0f;
            //BarObjectiveText.text = (stateIndex + 1) + " / " +LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing.Length;
            ObjectiveRemainingText.text = LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[stateIndex ].GetMiniObjective();
            StartCoroutine(ChangeMiniText());
        }
    }
    IEnumerator ChangeMiniText() 
    {
        yield return new WaitForSeconds(3f);
        MiniCompleteObjectivePanel.SetActive(false);
        MiniCompleteObjectivePanelText.text = LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[stateIndex].GetMiniObjective();
    }
    IEnumerator Wait() 
    {
       
        yield return new WaitForSeconds(0.5f);
        for (int i=0;i< LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing.Length;i++) 
        {
            if (!LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].Sequencing[i].COmpleted)
                StopCoroutine(_coroutine);
        }
        for (int i = 0; i < Apps.Count; i++)
        {
            yield return new WaitForSeconds(0.05f);
            Apps[i].GetComponent<LinkingRelation>().ChangeColor();
        }
        StopCoroutine(TotalTime);
        yield return new WaitForSeconds(0.5f);
        LevelCompleteScreen.SetActive(true);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, GameManager.Instance.LevelNo.ToString());
        //float TotalMove = (LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].CompleteMoves * 2f);
        float mid = (LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].CompleteMoves * 1.5f);
        if (LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].CompleteMoves >= CurrentMoves)
        {
            StartCoroutine(FillStars(3));
            UnlockLevels();
        }
        else if (LevelHandler.Instance.Config.LC[GameManager.Instance.LevelNo].CompleteMoves < CurrentMoves && CurrentMoves < mid)
        {
            StartCoroutine(FillStars(2));
            UnlockLevels();
        }
        else 
        {
            StartCoroutine(FillStars(1));
        }
        TotalMovesTaken.text = CurrentMoves.ToString();
        TimeTaken.text = timer + " Sec";
        _Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        Canfetti.SetActive(true);
    }
    void UnlockLevels() 
    {
        if (PlayerPrefs.GetInt("UnlockLevel") <= GameManager.Instance.LevelNo && PlayerPrefs.GetInt("UnlockLevel") < 6)
        {
            Debug.Log("here i am addding one into it");
            PlayerPrefs.SetInt("UnlockLevel", PlayerPrefs.GetInt("UnlockLevel") +1);
        }
    }
    //appList[index].GetComponent<LinkingRelation>().OnLeft(index);
    public void ScoreByName(int index,List<GameObject> appList)
    {
        string currentColor = appList[index].GetComponent<AppAttriutes>().AppName;
        char[] currentTemp = currentColor.ToCharArray();


        #region Code Commited
        //Checking Previous 
        //if ((index % (LevelHandler.Instance.Config.Col)) != 0)
        //{
        //    if (appList[index - 1].GetComponent<AppAttriutes>())
        //    {
        //        string previousName = appList[index - 1].GetComponent<AppAttriutes>().AppName;
        //        char[] nextTemp = previousName.ToCharArray();


        //        for (int i = 0; i < currentTemp.Length; i++)
        //        {
        //          //  Debug.Log("-----CurrentFirst :" + currentTemp[i] + "-----NextFirst" + nextTemp[i]);
        //            if (currentTemp[i] > nextTemp[i])
        //            {
        //             //   Debug.Log("Score By Name Added");
        //                //Okay boss
        //                AddScore();
        //                appList[index].GetComponent<LinkingRelation>().OnLeft(index);

        //                break;
        //            }
        //            else if (currentTemp[i] < nextTemp[i])
        //            {
        //                //Score boss 
        //                DeductScore();

        //                break;
        //            }
        //        }
        //    }
        //}
        #endregion

        // Checking Next
        if ((index % (LevelHandler.Instance.Config.Col)) != LevelHandler.Instance.Config.Col-1)
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
                        AddScore(ScoreState.Name);
                        StartCoroutine(LineWait(index,appList));
                        // Debug.Log("Score By Name Added");
                        break;
                    }
                }
            }
        }
        #region Code Commited
        //Checking Top
        //if (index >= LevelHandler.Instance.Config.Col)
        //{
        //    if (appList[index - LevelHandler.Instance.Config.Col].GetComponent<AppAttriutes>())
        //    {
        //        string topName = appList[index - LevelHandler.Instance.Config.Col]
        //            .GetComponent<AppAttriutes>().AppName;
        //        char[] nextTemp = topName.ToCharArray();


        //        for (int i = 0; i < currentTemp.Length; i++)
        //        {
        //            if (currentTemp[i] > nextTemp[i])
        //            {
        //               // Debug.Log("Score By Name Added");
        //                //score added
        //                AddScore();
        //                appList[index].GetComponent<LinkingRelation>().OnUp(index - LevelHandler.Instance.Config.Col);
        //                break;
        //            }
        //            else if (currentTemp[i] < nextTemp[i])
        //            {
        //                //add deducted
        //                DeductScore();

        //                break;
        //            }
        //        }
        //    }
        //}

        ////Checking Bottom
        //if (index < (appList.Count-LevelHandler.Instance.Config.Col))
        //{
        //    if (appList[index + LevelHandler.Instance.Config.Col]
        //        .GetComponent<AppAttriutes>())
        //    {
        //        string bottomName = appList[index + LevelHandler.Instance.Config.Col]
        //            .GetComponent<AppAttriutes>().AppName;
        //        char[] nextTemp = bottomName.ToCharArray();


        //        for (int i = 0; i < currentTemp.Length; i++)
        //        {
        //            if (currentTemp[i] > nextTemp[i])
        //            {
        //             //   Debug.Log("Score By Name Added");
        //                //score deducted
        //                DeductScore();

        //                break;
        //            }
        //            else if (currentTemp[i] < nextTemp[i])
        //            {
        //                //score added
        //                AddScore();
        //                appList[index].GetComponent<LinkingRelation>().OnDown(index + LevelHandler.Instance.Config.Col);
        //                break;
        //            }
        //        }
        //    }
        //}
        #endregion
    }
    IEnumerator LineWait(int Index,List<GameObject> appList) 
    {
        yield return new WaitForSeconds(0.2f);
        appList[Index].GetComponent<LinkingRelation>().OnRight(Index);
    }
    public void ScoreByColor(int index,List<GameObject> appList)
    {
        string currentColor = appList[index].GetComponent<AppAttriutes>().ColorName;
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance.Config.Col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index - 1].GetComponent<AppAttriutes>().ColorName;
             

                if (currentColor == nextColor)
                {
                    // Debug.Log("Score By Name Added");
                    //Okay boss
                    AddScore(ScoreState.Color);
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
        if ((index % (LevelHandler.Instance.Config.Col)) != LevelHandler.Instance.Config.Col-1)
        {
            if ((index + 1)<appList.Count) {
                if (appList[index + 1].GetComponent<AppAttriutes>())
                {

                    string nextColor = appList[index + 1].GetComponent<AppAttriutes>().ColorName;
                    char[] nextTemp = nextColor.ToCharArray();


                    if (currentColor == nextColor)
                    {
                        Debug.Log("Score By Name Added");
                        //Okay boss
                        AddScore(ScoreState.Color);
                        appList[index].GetComponent<LinkingRelation>().OnRight(index);
                    }
                    else
                    {
                        //Score boss 
                        DeductScore();

                    }
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance.Config.Col)
        {
            if (appList[index - LevelHandler.Instance.Config.Col]
                .GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index - LevelHandler.Instance.Config.Col]
                    .GetComponent<AppAttriutes>().ColorName;
              

                if (currentColor == nextColor)
                {
                    //  Debug.Log("Score By Name Added");
                    //Okay boss
                    AddScore(ScoreState.Color);
                    appList[index].GetComponent<LinkingRelation>().OnUp(index - LevelHandler.Instance.Config.Col);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                   
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count - LevelHandler.Instance.Config.Col))
        {
            if (appList[index + LevelHandler.Instance.Config.Col]
                .GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index + LevelHandler.Instance.Config.Col]
                    .GetComponent<AppAttriutes>().ColorName;
              

                if (currentColor == nextColor)
                {
                    // Debug.Log("Score By Name Added");
                    //Okay boss
                    AddScore(ScoreState.Color);
                    appList[index].GetComponent<LinkingRelation>().OnDown(index + LevelHandler.Instance.Config.Col);
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
        if ((index % (LevelHandler.Instance.Config.Col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index - 1].GetComponent<AppAttriutes>().Creator;
               

                if (currentCreator == nextCreator)
                {
                    // Debug.Log("Score By Creator Added");
                    //Okay boss
                    AddScore(ScoreState.Creator);
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
        if ((index % (LevelHandler.Instance.Config.Col)) != LevelHandler.Instance.Config.Col-1)
        {
            if (appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index + 1].GetComponent<AppAttriutes>().Creator;
             

                if (currentCreator == nextCreator)
                {
                    // Debug.Log("Score By Creator Added");
                    //Okay boss
                    AddScore(ScoreState.Creator);
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
        if (index >= LevelHandler.Instance.Config.Col)
        {
            if (appList[index - LevelHandler.Instance.Config.Col].GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index - LevelHandler.Instance.Config.Col]
                    .GetComponent<AppAttriutes>().Creator;
            

                if (currentCreator == nextCreator)
                {
                    // Debug.Log("Score By Creator Added");
                    //Okay boss
                    AddScore(ScoreState.Creator);
                    appList[index].GetComponent<LinkingRelation>().OnUp(index - LevelHandler.Instance.Config.Col);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                    
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count-LevelHandler.Instance.Config.Col))
        {
            if (appList[index + LevelHandler.Instance.Config.Col]
                .GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index + LevelHandler.Instance.Config.Col]
                    .GetComponent<AppAttriutes>().Creator;
              

                if (currentCreator == nextCreator)
                {
                    //  Debug.Log("Score By Creator Added");
                    //Okay boss
                    AddScore(ScoreState.Creator);
                    appList[index].GetComponent<LinkingRelation>().OnDown(index + LevelHandler.Instance.Config.Col);
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
        if ((index % (LevelHandler.Instance.Config.Col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index - 1].GetComponent<AppAttriutes>().AppCatagory;
              

                if (currentCategory == nextCategory)
                {
                    // Debug.Log("Score By Category Added");
                    //Okay boss
                    AddScore(ScoreState.Category);
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
        if ((index % (LevelHandler.Instance.Config.Col)) != LevelHandler.Instance.Config.Col-1)
        {
            if (appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index + 1].GetComponent<AppAttriutes>().AppCatagory;
                char[] nextTemp = nextCategory.ToCharArray();
             

                if (currentCategory == nextCategory)
                {
                    // Debug.Log("Score By Category Added");
                    //Okay boss
                    AddScore(ScoreState.Category);
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
        if (index >= LevelHandler.Instance.Config.Col)
        {
            if (appList[index - LevelHandler.Instance.Config.Col].GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index - LevelHandler.Instance.Config.Col]
                    .GetComponent<AppAttriutes>().AppCatagory;
              

                if (currentCategory == nextCategory)
                {
                    // Debug.Log("Score By Category Added");
                    //Okay boss
                    AddScore(ScoreState.Category);
                    appList[index].GetComponent<LinkingRelation>().OnUp(index - LevelHandler.Instance.Config.Col);
                }
                else
                {
                    //Score boss 
                    DeductScore();
                  
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count - LevelHandler.Instance.Config.Col))
        {
            if (appList[index + LevelHandler.Instance.Config.Col]
                .GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index + LevelHandler.Instance.Config.Col]
                    .GetComponent<AppAttriutes>().AppCatagory;
               

                if (currentCategory == nextCategory)
                {
                    //Debug.Log("Score By Category Added");
                    //Okay boss
                    AddScore(ScoreState.Category);
                    appList[index].GetComponent<LinkingRelation>().OnDown(index + LevelHandler.Instance.Config.Col);
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
    #region Commneted Region
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
    #endregion
    public void ActivateTriggers(GameObject Obj) 
    {
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
        AnalyticsAdsManager.instance.ShowIronsourceInterstitialAd();
        GameManager.Instance.Restart = true;
        SceneManager.LoadScene("Gameplay");
    }
    public void OnNext()
    {
        AnalyticsAdsManager.instance.ShowIronsourceInterstitialAd();
        GameManager.Instance.Next = true;
       // GameManager.Instance.LevelNo +=1;
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
    IEnumerator FillStars(int num) 
    {
        for (int i=0;i<num;i++)
        {
            yield return new WaitForSeconds(0.15f);
            Stars[i].SetActive(true);
            Stars[i].GetComponent<Image>().DOFillAmount(1f,1f);
        }
    }
    public void OnPause() { Time.timeScale = 0; }
    public void OnResume() { Time.timeScale = 1; }
    public void OnObjectiveClose() 
    {
        TotalTime = StartCoroutine(startTimer());
    }
  
    IEnumerator startTimer()
    {
        yield return new WaitForSeconds(1);
        timer++;
    }
    public void StartShaking()
    {
        Debug.Log(gameObject.name);
        //Debug.Log(gameObject.GetComponent<AppAttriutes>().name + " Object");
        for (int i= 0; i < Apps.Count;i++)
        {
           // if (!Apps[i].gameObject == DraggedObject) 
            {
                Apps[i].GetComponent<Animator>().enabled = true;
            }
        }
        DraggedObject.GetComponent<Animator>().enabled = false;
    }
    public void StopShaking()
    {
        for (int i = 0; i < Apps.Count; i++)
        {
            {
                Apps[i].GetComponent<Animator>().enabled = false;
              Apps[i].transform.eulerAngles = new Vector3(0,0,0);
            }
        }
    }

}
