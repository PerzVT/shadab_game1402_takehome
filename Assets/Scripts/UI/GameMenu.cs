using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMenu : MonoBehaviour
{
    public static bool isPaused; //this is a static variable that we can use to check if the game is paused
    public ExitPopup exitPopup; //this is the exit popup that we created
    public WinScreen winScreen; //this is the win screen that we created
    
    public TextMeshProUGUI textPrefab; //this is the text prefab that we created
    private int numTeams; //this is the number of teams in the game
    private List<TextMeshProUGUI> scoreFields = new List<TextMeshProUGUI>(); //this is a list of score fields that we can use to display the scores
    
    void Start()
    {
        numTeams = GameManager.Instance.teams.Count; //get the number of teams

        for(int i = 0; i < numTeams; i++) //loop through all of the teams
        {
            TextMeshProUGUI newScoreField = Instantiate(textPrefab);  //create a new score field
            newScoreField.transform.SetParent(textPrefab.transform.parent, false); //set the parent of the score field
            newScoreField.color = GameManager.Instance.teams[i]; //set the color of the score field
            scoreFields.Add(newScoreField); //add the score field to the list
        }
        Destroy(textPrefab.gameObject); //destroy the text prefab
        isPaused = false; //set isPaused to false
        Time.timeScale = 1f; //set the time scale to 1
    }

    void Update()
    {
        Dictionary<int, int> capturedOutposts = GameManager.Instance.GetCapturedOutpostsByTeam(); //get the captured outposts by team

        for(int i = 0; i < numTeams; i++) //loop through all of the teams
        {
            scoreFields[i].text = ScoreManager.Instance.scores[i].ToString(); //set the score field text to the score

            if(capturedOutposts.ContainsKey(i)) //if the captured outposts dictionary contains the team
            {
                scoreFields[i].text += "\nOutposts: " + capturedOutposts[i].ToString(); //set the score field text to the score and the number of outposts captured by the team
            }
            else
            {
                scoreFields[i].text += "\nOutposts: 0"; //set the score field text to the score and 0 outposts captured by the team
            }
        }

        if (GameManager.Instance.IsGameOver())  //if the game is over
        {
            int winningTeam = GameManager.Instance.GetWinningTeam(); //get the winning team
            Color teamColor = GameManager.Instance.teams[winningTeam]; //get the color of the winning team
            winScreen.ShowWinScreen(winningTeam, teamColor); //show the win screen
        }

        if (Input.GetKeyDown(KeyCode.P)) //if the player presses the P key
        {
            TogglePause(); //toggle the pause
        }
    }

    void TogglePause() //this function will toggle the pause
    {
        isPaused = !isPaused; //toggle the pause
        exitPopup.gameObject.SetActive(isPaused); //set the exit popup to active

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None; //set the cursor lock state to none
            Cursor.visible = true;  //set the cursor to visible
            Time.timeScale = 0; //set the time scale to 0
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; //set the cursor lock state to locked
            Time.timeScale = 1; //set the time scale to 1
        }
    }
}
