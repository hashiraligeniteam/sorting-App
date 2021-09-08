using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager 
{
    public static GameManager _Instance;
    public int LevelNo;
    public float score;
    
    public static GameManager Instance 
    {
        get
        {
            if (_Instance ==null) 
            {
                _Instance = new GameManager();
            }
            return _Instance;
        }
        
    }

}
