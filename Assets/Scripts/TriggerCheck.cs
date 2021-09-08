using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponent<UIDrag>().Drag == false && collision.gameObject.GetComponent<UIDrag>().Drag == true) 
        {
            Debug.Log("****************************Here**************************  "+transform.GetComponent<AppAttriutes>().AppName);
            GameHandler.Instance.UpdateChildList(collision.gameObject,this.gameObject);
            StartCoroutine(OnOffOtherCollider(collision.gameObject));
            
            
        }
    }

    IEnumerator OnOffOtherCollider(GameObject other)
    {
        GameHandler.Instance.DisableTriggers(this.gameObject, other);
        other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.4f);
        other.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        GameHandler.Instance.EnableTriggers();
    }
}
