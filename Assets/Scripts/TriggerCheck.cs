using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerCheck : MonoBehaviour
{
    public bool InsideFolder;
    public bool Folder;
    public GridLayoutGroup LayoutGroup;
    public bool once;
    public bool Middle;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<TriggerCheck>()) 
        {
           // Middle = true;

            if (GetComponent<AppAttriutes>()) 
            {
                GetComponent<AppAttriutes>().MainSPrite = GetComponent<Image>().sprite;
                GetComponent<AppAttriutes>().HighLighter.GetComponent<Image>().sprite = GetComponent<AppAttriutes>().MainSPrite;
                GetComponent<AppAttriutes>().HighLighter.SetActive(true);
                //GetComponent<Image>().sprite = GetComponent<AppAttriutes>().HighliterSprite;
            }
        }
        if (other.GetComponent<UIDrag>() && GetComponent<UIDrag>())
        {
            Debug.Log("First Drag");
            if (GetComponent<UIDrag>().Drag == false && other.gameObject.GetComponent<UIDrag>().Drag == false)
            {
                Debug.Log("First Drag 1");
                if (/*GetComponent<UIDrag>().pointerUp == true*/Middle && !InsideFolder )
                {
                    Debug.Log("First Drag 2");
                    if (!Folder  /*&& !GetComponent<UIDrag>().IsEmpty      Empty Cell Logic*/)
                    {
                        //Create folder
                        Debug.Log("Folder Creation");
                        if (other.GetComponent<TriggerCheck>().Folder && !other.GetComponent<TriggerCheck>())
                        {
                            Debug.Log("adding in folder");
                            GameHandler.Instance.AddInFolder(gameObject, other.gameObject);
                        }
                        else
                        {
                            Debug.Log("Create Folder");
                            if (!other.GetComponent<UIDrag>().Moving && !other.GetComponent<TriggerCheck>().InsideFolder)
                            {
                                Debug.Log(transform.parent.name + " Parent Name " + gameObject.name + " My name");
                                GameHandler.Instance.CreateFolder(this.gameObject, other.gameObject, this);
                                other.GetComponent<UIDrag>().Moving = false;
                            }
                          
                        }
                    }
                }
            }
        
        }
        
    }

    public void OnPointerUp() 
    {
        if (Folder && GetComponent<UIDrag>().Drag == false) 
        {
            if (LayoutGroup) 
            {
                GameHandler.Instance.FolderScreen.SetActive(true);
                int folderChildCount = LayoutGroup.transform.childCount;
                for (int i=0;i<folderChildCount;i++)
                {
                 
                    LayoutGroup.transform.GetChild(0).SetParent(GameHandler.Instance.OpenFolderRef.transform);
                }
                foreach (GameObject obj in GameHandler.Instance.Apps)
                {
                    GameHandler.Instance.DisableTriggers(obj);
                }
                GameHandler.Instance.InsideFolderApps.Clear();
                for (int i = 0; i < GameHandler.Instance.OpenFolderRef.transform.childCount; i++)
                {
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(i).gameObject.SetActive(true);
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(i).GetComponent<AppAttriutes>().AppsCountImage.SetActive(false);
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(i).GetComponent<TriggerCheck>().Middle = false;
                    GameHandler.Instance.ActivateTriggers(GameHandler.Instance.OpenFolderRef.transform.GetChild(i).gameObject);
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(i).GetComponent<TriggerCheck>().InsideFolder = true;
                    GameHandler.Instance.InsideFolderApps.Add(GameHandler.Instance.OpenFolderRef.transform.GetChild(i).gameObject);
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(i).gameObject.AddComponent<UIDrag>();
                }

                GameHandler.Instance.currentFolderGridClosed = LayoutGroup.gameObject;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<TriggerCheck>())
        {
            Debug.Log("Exit");
            Middle = false;
            if (GetComponent<AppAttriutes>())
            {
                GetComponent<AppAttriutes>().HighLighter.SetActive(false);
            }
                //GetComponent<AppAttriutes>().HighLighter.GetComponent<Image>().sprite = GetComponent<AppAttriutes>().HighliterSprite; 
                //GetComponent<Image>().sprite = GetComponent<AppAttriutes>().MainSPrite;
            }
        }

}
