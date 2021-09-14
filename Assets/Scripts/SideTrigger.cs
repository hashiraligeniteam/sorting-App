using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideTrigger : MonoBehaviour
{
    public bool first;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GetComponentInParent<UIDrag>())
        {
            if (GetComponentInParent<UIDrag>().Drag == false && other.gameObject.GetComponentInParent<UIDrag>().Drag == true)
            {
                Debug.Log("BeforeFirst");
                if (first)
                {
                    Debug.Log("In IF");
                    GetComponentInParent<UIDrag>().triggerEnteredOnce = true;
                }
                else
                {
                    Debug.Log("In Else");
                    GetComponentInParent<UIDrag>().triggerEnteredTwice = true;
                }
                
                SwitchApps(other.gameObject);
            }
        }
    }

    private void SwitchApps(GameObject other)
    {
        if ((GetComponentInParent<UIDrag>().triggerEnteredOnce == true) &&
            (GetComponentInParent<UIDrag>().triggerEnteredTwice == true))
        {
            GameHandler.Instance.UpdateChildList(other.transform.parent.gameObject,transform.parent.gameObject);
            StartCoroutine(OnOffOtherCollider(other));
            
            GetComponentInParent<UIDrag>().triggerEnteredOnce = false;
            GetComponentInParent<UIDrag>().triggerEnteredTwice = false;
        }
    }
    
    IEnumerator OnOffOtherCollider(GameObject other)
    {
        GameHandler.Instance.DisableTriggers(transform.parent.gameObject, other.transform.parent.gameObject);
        other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        other.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        GameHandler.Instance.EnableTriggers();
    }
    
}
