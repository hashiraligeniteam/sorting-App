using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class SideTrigger : MonoBehaviour
{
    [FormerlySerializedAs("first")] public bool rightTrigger;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GetComponentInParent<UIDrag>())
        {
            if (GetComponentInParent<UIDrag>().Drag == false && other.gameObject.GetComponentInParent<UIDrag>().Drag == true)
            {
             
                if (rightTrigger)
                {
                  
                    GetComponentInParent<UIDrag>().triggerEnteredOnce = true;
                }
                else
                {
                   
                    GetComponentInParent<UIDrag>().triggerEnteredTwice = true;
                }

                AssignSwapObject(other.gameObject);
            }
        }
      
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (GetComponentInParent<UIDrag>().GetComponent<AppAttriutes>())
        //{
        //    GetComponentInParent<UIDrag>().GetComponent<AppAttriutes>().HighLighter.GetComponent<Image>().sprite = GetComponentInParent<UIDrag>().GetComponent<AppAttriutes>().AppSprite;
        //    GetComponentInParent<UIDrag>().GetComponent<AppAttriutes>().HighLighter.SetActive(true);
        //    GetComponentInParent<UIDrag>().GetComponent<Image>().sprite = GetComponentInParent<UIDrag>().GetComponent<AppAttriutes>().HighliterSprite;
        //    = GetComponent<AppAttriutes>().MainSPrite;
        //}
    }
    int indexNumber;
    private void AssignSwapObject(GameObject other)
    {
        if ((GetComponentInParent<UIDrag>().triggerEnteredOnce == true) &&
           (GetComponentInParent<UIDrag>().triggerEnteredTwice == true))
        {
            GameHandler.Instance.SwapableObject = transform.parent.gameObject;
            return;
        }
        else if (GetComponentInParent<UIDrag>().triggerEnteredOnce)
        {
            if (GetComponentInParent<TriggerCheck>().InsideFolder) 
            {
                indexNumber = GameHandler.Instance.InsideFolderApps.IndexOf(GetComponentInParent<UIDrag>().gameObject);
                if (indexNumber !=0)
                {
                  //  if (GameHandler.Instance.InsideFolderApps[indexNumber-1].GetComponent<UIDrag>().triggerEnteredTwice)
                    {
                        GameHandler.Instance.SwapableObject = GameHandler.Instance.InsideFolderApps[indexNumber-1].gameObject;
                        return;
                    }
                }
            } 
            else 
            {
                 indexNumber = GameHandler.Instance.Apps.IndexOf(GetComponentInParent<UIDrag>().gameObject);
                if (indexNumber != 0) 
                {
                 //   if (GameHandler.Instance.Apps[indexNumber-1].GetComponent<UIDrag>().triggerEnteredTwice)
                    {
                        GameHandler.Instance.SwapableObject = GameHandler.Instance.Apps[indexNumber-1].gameObject;
                        return;
                    }
                }
            }
        }
        else if (GetComponentInParent<UIDrag>().triggerEnteredTwice)
        {
            if (GetComponentInParent<TriggerCheck>().InsideFolder)
            {
                indexNumber = GameHandler.Instance.InsideFolderApps.IndexOf(GetComponentInParent<UIDrag>().gameObject);
                if (indexNumber < GameHandler.Instance.InsideFolderApps.Count)
                {
                  //  if (GameHandler.Instance.InsideFolderApps[indexNumber].GetComponent<UIDrag>().triggerEnteredTwice)
                    {
                        GameHandler.Instance.SwapableObject = GameHandler.Instance.InsideFolderApps[indexNumber].gameObject;
                        return;
                    }
                }
            }
            else
            {
                indexNumber = GameHandler.Instance.Apps.IndexOf(GetComponentInParent<UIDrag>().gameObject);
                if (indexNumber < GameHandler.Instance.InsideFolderApps.Count)
                {
                    //if (GameHandler.Instance.Apps[indexNumber].GetComponent<UIDrag>().triggerEnteredTwice)
                    {
                        GameHandler.Instance.SwapableObject = GameHandler.Instance.Apps[indexNumber].gameObject;
                        return;
                    }
                }
            }
        }
    }
    public void SwitchApps(GameObject other) 
    {
        GameHandler.Instance.UpdateChildList(other.gameObject, transform.parent.gameObject);
        StartCoroutine(OnOffOtherCollider(other));
        GetComponentInParent<UIDrag>().triggerEnteredOnce = false;
        GetComponentInParent<UIDrag>().triggerEnteredTwice = false;
    }
    private IEnumerator OnOffOtherCollider(GameObject other)
    {
        GameHandler.Instance.DisableTriggers(transform.parent.gameObject, other.transform.parent.gameObject);
        other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        other.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        GameHandler.Instance.EnableTriggers();
    }
    void OnTriggerExit2D(Collider2D other)
    {
        //if ((GetComponentInParent<UIDrag>().triggerEnteredOnce == true) &&
        //  (GetComponentInParent<UIDrag>().triggerEnteredTwice == true))
        //{
        //    GameHandler.Instance.SwapableObject = null;
        //}
        if (GetComponentInParent<UIDrag>())
        {
           

            if (!GetComponentInParent<TriggerCheck>().Middle)
            {
                StartCoroutine(wait(other));
            }
        }
    }
    IEnumerator wait(Collider2D other) 
    {
        yield return new WaitForSeconds(0.5f);
        if (GetComponentInParent<UIDrag>().triggerEnteredOnce || other.gameObject.GetComponentInParent<UIDrag>().triggerEnteredTwice)
        {
            //GetComponentInParent<UIDrag>().GetComponent<AppAttriutes>().HighLighter.SetActive(false);
            //GetComponentInParent<UIDrag>().GetComponent<Image>().sprite = GetComponentInParent<UIDrag>().GetComponent<AppAttriutes>().AppSprite;
            GetComponentInParent<UIDrag>().triggerEnteredOnce = false;
            GetComponentInParent<UIDrag>().triggerEnteredTwice = false;
        }
    }

}
