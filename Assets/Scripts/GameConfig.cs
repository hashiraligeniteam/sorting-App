using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public List<AppsAttributes> AT;
    public List<Levels> LC;
    public int Col = 4;
   
    public AppsAttributes GetAppByName(string Name) 
    {
        for (int i = 0; i<AT.Count; i++)
        {
            if (AT[i].AppName.Equals(Name))
            {
                return AT[i];
            }
        }
        return null;
    }
    public string GetObjectiveByLevel(int LevelNo) 
    {
        string Objective = "";
        for (int i =0;i<LC[LevelNo].Sequencing.Length;i++)
        {
            Objective += (i + 1) + ". Score " + LC[LevelNo].Sequencing[i].ScoreToComplete + " Links by "+ LC[LevelNo].Sequencing[i].State + "\n";
        }
        return Objective;
    }
    
}
