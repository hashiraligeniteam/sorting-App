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
    
    public void OnDrag(PointerEventData eventData)
    {
        GameHandler.Instance.Dargging = true;
        transform.position = Input.mousePosition - diffPosition;
        Drag = true;
        //Debug.Log(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameHandler.Instance.Dargging = false;
        Drag = false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = transform.position;
        diffPosition = Input.mousePosition - startPosition;
        EventSystem.current.SetSelectedGameObject(gameObject);

        GameHandler.Instance.finalAppPosition = transform.localPosition;
        //EventSystem.current.currentSelectedGameObject.transform.SetParent(canvas_.transform);
        //EventSystem.current.currentSelectedGameObject.transform.SetAsFirstSibling();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //GameHandler.Instance.UpdateListIndex();
        Debug.Log("PointerUp");
        transform.localPosition = GameHandler.Instance.finalAppPosition;
    }
    

}
