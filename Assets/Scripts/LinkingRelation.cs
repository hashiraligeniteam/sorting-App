using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkingRelation : MonoBehaviour
{
    public GameObject[] LeftImages;
    public GameObject[] RightImages;
    public GameObject[] UpImages;
    public GameObject[] DownImages;

    public void OnRight(int CurrentIndex) 
    {
        //Debug.Log(CurrentIndex + " Right " + GameHandler.Instance.Apps[CurrentIndex ].GetComponent <AppAttriutes>().name);
        for (int i = 0;i< GameHandler.Instance.Apps[CurrentIndex + 1].GetComponent<LinkingRelation>().LeftImages.Length;i++)
        {
            if (GameHandler.Instance.Apps[CurrentIndex + 1].GetComponent<LinkingRelation>().LeftImages[i].active)
            {
                return;
            }
        }
        for (int i = 0; i < RightImages.Length; i++)
        {
            if (RightImages[i].active)
            {
                if (i != 3)
                    return;
                RightImages[i].SetActive(false);
                RightImages[i+1].SetActive(true);
                return;
            }
        }
        RightImages[0].SetActive(true);
        //  if (GameHandler.Instance.Apps[CurrentIndex+1].GetComponent<Linking>().) { }
    }
    public void OnLeft(int CurrentIndex)
    {
        for (int i = 0; i < GameHandler.Instance.Apps[CurrentIndex - 1].GetComponent<LinkingRelation>().RightImages.Length; i++)
        {
            if (GameHandler.Instance.Apps[CurrentIndex - 1].GetComponent<LinkingRelation>().LeftImages[i].active)
            {
                return;
            }
        }
        for (int i = 0; i < LeftImages.Length; i++)
        {
            if (LeftImages[i].active)
            {
                if (i != 3)
                    return;
                LeftImages[i].SetActive(false);
                LeftImages[i + 1].SetActive(true);
                return;
            }
        }
        LeftImages[0].SetActive(true);
    }
    public void OnUp( int backindex) 
    {
        for (int i = 0; i < GameHandler.Instance.Apps[backindex].GetComponent<LinkingRelation>().DownImages.Length; i++)
        {
            if (GameHandler.Instance.Apps[backindex].GetComponent<LinkingRelation>().DownImages[i].active)
            {
                return;
            }
        }
        for (int i = 0; i < UpImages.Length; i++)
        {
            if (UpImages[i].active)
            {
                if (i != 3)
                    return;
                UpImages[i].SetActive(false);
                UpImages[i + 1].SetActive(true);
                return;
            }
        }
        UpImages[0].SetActive(true);
    }
    public void OnDown(int backindex) 
    {
        for (int i = 0; i < GameHandler.Instance.Apps[backindex].GetComponent<LinkingRelation>().UpImages.Length; i++)
        {
            if (GameHandler.Instance.Apps[backindex].GetComponent<LinkingRelation>().UpImages[i].active)
            {
                return;
            }
        }
        for (int i = 0; i < DownImages.Length; i++)
        {
            if (DownImages[i].active)
            {
                if (i != 3)
                    return;
                DownImages[i].SetActive(false);
                DownImages[i + 1].SetActive(true);
                return;
            }
        }
        DownImages[0].SetActive(true);
    }

    public void Close() 
    {
        CloseImages(LeftImages);
        CloseImages(RightImages);
        CloseImages(UpImages);
        CloseImages(DownImages);
    }

     void CloseImages(GameObject[] Ref) 
     {
        for (int i =0;i<Ref.Length;i++) 
        {
            Ref[i].SetActive(false);
        }
     }

}
