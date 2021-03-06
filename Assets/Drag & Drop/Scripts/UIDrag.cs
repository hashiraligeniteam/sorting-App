/* GitHub project: https://github.com/danielcmcg/Unity-UI-Nested-Drag-and-Drop */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler,IPointerUpHandler
{
    Vector3 startPosition;
    Vector3 diffPosition;
    public bool Drag;
    public bool pointerUp;
    public bool triggerEnteredOnce;
    public bool triggerEnteredTwice;
    public bool Moving;
    //Empty cell logic
   // public bool IsEmpty;



    public void OnBeginDrag(PointerEventData eventData)
    {
       
    }
    public void OnDrag(PointerEventData eventData)
    {
        //if (!IsEmpty)
        {
            //GameHandler.Instance.Dargging = true;
            transform.position = Input.mousePosition - diffPosition;
            Drag = true;
            Moving = true;
           
           
            //     if (GetComponent<AppAttriutes>())
            //{
            //     GetComponent<AppAttriutes>().HighLighter.GetComponent<Image>().sprite = GetComponent<AppAttriutes>().AppSprite;
            //     GetComponent<AppAttriutes>().HighLighter.SetActive(true);
            //    GetComponent<Image>().sprite = GetComponent<AppAttriutes>().HighliterSprite;
            //     //= GetComponent<AppAttriutes>().MainSPrite;
            // }
        }

        //Debug.Log(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //GameHandler.Instance.Dargging = false;
        if (GetComponent<AppAttriutes>())
        {
            GetComponent<AppAttriutes>().HighLighter.SetActive(false);
            GetComponent<Image>().sprite = GetComponent<AppAttriutes>().AppSprite;
        }
        Drag = false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        GameHandler.Instance.DraggedObject = this.gameObject;
        GameHandler.Instance.OpenFolderRef.GetComponent<GridLayoutGroup>().enabled = false;
        GameHandler.Instance.AppsMainParent.GetComponent<GridLayoutGroup>().enabled = false;
        Debug.Log("hereeeeeeeeeeeeeee I called");
       
        //if (GetComponent<AppAttriutes>())
        //{
        //    //GetComponent<AppAttriutes>().HighLighter.gameObject.GetComponent<Image>().sprite = null;
        //    GetComponent<AppAttriutes>().HighLighter.gameObject.GetComponent<Image>().sprite = GetComponent<AppAttriutes>().AppSprite;
        //}
        //  if (!IsEmpty) 
        {
            GameHandler.Instance.CloseRealtion();
            GameHandler.Instance.SwapableObject = null;
            gameObject.AddComponent<GraphicRaycaster>();
            GetComponent<Canvas>().overrideSorting = true;
            GetComponent<Canvas>().sortingOrder = 1;
            GameHandler.Instance.appBeingused = true;
            pointerUp = false;
            startPosition = transform.position;
            diffPosition = Input.mousePosition - startPosition;
            EventSystem.current.SetSelectedGameObject(gameObject);
            GameHandler.Instance.finalAppPosition = transform.localPosition;
            GameHandler.Instance.StartShaking();
        }
       
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        GameHandler.Instance.StopShaking();
       // StopShaking();
        //if (!IsEmpty)
        {
            GameHandler.Instance.StartCoroutine(GameHandler.Instance.DestroyingObjects(GetComponent<GraphicRaycaster>(), GetComponent<Canvas>()));
            Drag = false;
            pointerUp = true;

            //  Moving = false;
            if (GetComponent<TriggerCheck>().InsideFolder && !RectTransformUtility.RectangleContainsScreenPoint(GameHandler.Instance.OpenFolderRef.GetComponent<Image>().rectTransform, Input.mousePosition))
            {
                GameHandler.Instance.InsideFolderApps.Remove(this.gameObject);
                GameHandler.Instance.Apps.Add(this.gameObject);
                transform.SetParent(GameHandler.Instance.AppsMainParent.transform);
                if (GameHandler.Instance.OpenFolderRef.transform.childCount == 1)
                {
                    GameHandler.Instance.InsideFolderApps.Remove(GameHandler.Instance.OpenFolderRef.transform.GetChild(0).gameObject);
                    GameHandler.Instance.Apps.Add(GameHandler.Instance.OpenFolderRef.transform.GetChild(0).gameObject);
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(0).transform.SetParent(GameHandler.Instance.AppsMainParent.transform);
                    GameHandler.Instance.currentFolderGridClosed.transform.parent.gameObject.SetActive(false);
                    GameHandler.Instance.currentFolderGridClosed.transform.parent.SetParent(GameHandler.Instance.transform);

                    DestroyImmediate(GameHandler.Instance.currentFolderGridClosed.transform.parent.gameObject);
                }

                int folderChildCount = GameHandler.Instance.OpenFolderRef.transform.childCount;

                for (int i = 0; i < folderChildCount; i++)
                {
                    Destroy(GameHandler.Instance.OpenFolderRef.transform.GetChild(0).GetComponent<UIDrag>());
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(0).SetParent(GameHandler.Instance.currentFolderGridClosed.transform);
                    GameHandler.Instance.DisableTriggers(GameHandler.Instance.currentFolderGridClosed.transform.GetChild(i).gameObject);
                }

                GameHandler.Instance.InsideFolderApps.Clear();
                GameHandler.Instance.FolderScreen.SetActive(false);
                GameHandler.Instance.currentFolderGridClosed = null;
                GameHandler.Instance.UpdateListIndex(false);

                foreach (GameObject app in GameHandler.Instance.Apps)
                {
                    GameHandler.Instance.ActivateTriggers(app);
                }

            }

            GameHandler.Instance.appBeingused = false;


           
            if (GetComponent<UIDrag>())
            {
                if (GameHandler.Instance.SwapableObject)
                {
                    //   if (!GameHandler.Instance.SwapableObject.GetComponent<TriggerCheck>().Folder  && !GetComponent<TriggerCheck>().InsideFolder)
                    {
                        if (GameHandler.Instance.SwapableObject.GetComponent<UIDrag>())
                        {
                            //   if (GameHandler.Instance.SwapableObject.GetComponent<UIDrag>().triggerEnteredOnce || GameHandler.Instance.SwapableObject.GetComponent<UIDrag>().triggerEnteredTwice)
                            {
                                this.gameObject.transform.GetComponentInChildren<SideTrigger>().SwitchApps(GameHandler.Instance.SwapableObject);
                            }

                        }

                    }


                }
                else 
                {
                    GameHandler.Instance.InfoScreenAppName.text = GetComponent<AppAttriutes>().AppName;
                    GameHandler.Instance.InfoScreenColor.text = GetComponent<AppAttriutes>().ColorName;
                    GameHandler.Instance.InfoScreenCategory.text = GetComponent<AppAttriutes>().AppCatagory;
                    GameHandler.Instance.InfoScreenCreator.text = GetComponent<AppAttriutes>().Creator;
                    GameHandler.Instance.APPImage.sprite = GetComponent<AppAttriutes>().AppSprite;
                    GameHandler.Instance.InfoScreen.SetActive(true);
                }


            }

           
            GameHandler.Instance.CheckScore(GetComponent<TriggerCheck>().InsideFolder);
        }
    }

    IEnumerator ResetGrids()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Log("Working");
        GameHandler.Instance.OpenFolderRef.GetComponent<GridLayoutGroup>().enabled = false;
        GameHandler.Instance.AppsMainParent.GetComponent<GridLayoutGroup>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        GameHandler.Instance.OpenFolderRef.GetComponent<GridLayoutGroup>().enabled = true;
        GameHandler.Instance.AppsMainParent.GetComponent<GridLayoutGroup>().enabled = true;
        Moving = false;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ResetGrids());
    }
    

}
