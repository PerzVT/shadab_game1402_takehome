using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance; //this is the singleton instance
    public List<Color> teams = new List<Color>(); //this is a list of colours that we can use to represent our teams
    internal List<Outpost> outposts = new List<Outpost>(); //this is a list of outposts that we can use to keep track of all of the outposts in the scene
    public WinScreen winScreen; //this is a reference to the win screen that we can use to show the win screen when the game is over

    public static GameManager Instance
    {
        get
        {
            if(_instance == null) //if we don't have an instance yet
            {
                _instance = GameObject.FindObjectOfType<GameManager>(); //find the game manager in the scene
                _instance.OnCreateInstance(); //call the on create instance function
            }
            return _instance; //return the instance
        }
    }
    private void OnCreateInstance()
    {
        outposts = new List<Outpost>(GetComponentsInChildren<Outpost>()); //get all of the outposts in the scene
    }

    void Update()
    {
        if (IsGameOver())
        {
            int winningTeam = GetWinningTeam(); //get the winning team
            Color teamColor = teams[winningTeam]; //get the color of the winning team
            winScreen.ShowWinScreen(winningTeam, teamColor); //show the win screen
        }        
    }

    public bool IsGameOver()
    {
        return AreAllOutpostsCapturedByOneTeam() || DoesAnyTeamHaveTwentyPoints(); //if either of these are true, the game is over
    }

    private bool AreAllOutpostsCapturedByOneTeam() //this is a function that returns true if all of the outposts are captured by one team
    {
        Dictionary<int, int> outpostsByTeam = GetCapturedOutpostsByTeam(); //get the outposts by team

        foreach (KeyValuePair<int, int> entry in outpostsByTeam) //loop through all of the outposts by team
        {
            if (entry.Value == outposts.Count) //if the number of outposts captured by a team is equal to the number of outposts in the scene
                return true; //return true
        }

        return false;
    }

    private bool DoesAnyTeamHaveTwentyPoints() //this is a function that returns true if any team has twenty points
    {
        for (int i = 0; i < ScoreManager.Instance.scores.Count; i++) //loop through all of the scores
        {
            if (ScoreManager.Instance.scores[i] >= 20) //if any of the scores are greater than or equal to twenty
                return true; //return true
        }

        return false;
    }

    public int GetWinningTeam()
    {
        Dictionary<int, int> outpostsByTeam = GetCapturedOutpostsByTeam(); //get the outposts by team
        foreach (KeyValuePair<int, int> entry in outpostsByTeam) //loop through all of the outposts by team
        {
            if (entry.Value == outposts.Count) //if the number of outposts captured by a team is equal to the number of outposts in the scene
                return entry.Key; //return the team
        }

        for (int i = 0; i < ScoreManager.Instance.scores.Count; i++) //loop through all of the scores
        {
            if (ScoreManager.Instance.scores[i] >= 20) //if any of the scores are greater than or equal to twenty
                return i;
        }

        return -1;
    }

    public Dictionary<int, int> GetCapturedOutpostsByTeam() //this is a function that returns a dictionary of outposts captured by team
    {
        Dictionary<int, int> capturedOutposts = new Dictionary<int, int>(); //this is a dictionary of outposts captured by team

        foreach (Outpost outpost in outposts) //loop through all of the outposts
        {
            if(outpost.currentValue == 1)   //if the outpost is captured
            {
                if(capturedOutposts.ContainsKey(outpost.Team)) //if the dictionary already contains the team
                {
                    capturedOutposts[outpost.Team]++; //increment the number of outposts captured by the team
                }
                else
                {
                    capturedOutposts[outpost.Team] = 1; //set the number of outposts captured by the team to one
                }
            }
        }

        return capturedOutposts; //return the dictionary
    }
}
