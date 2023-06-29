using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public List<int> scores = new List<int>(); //this is a list of scores that we can use to keep track of the score for each team
    public static ScoreManager Instance 
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ScoreManager>(); //find the score manager in the scene
                _instance.scores = new List<int>(new int[GameManager.Instance.teams.Count]);  //create a new list of scores with the same number of elements as the number of teams
            }
            return _instance; //return the instance
        }
    }
}
