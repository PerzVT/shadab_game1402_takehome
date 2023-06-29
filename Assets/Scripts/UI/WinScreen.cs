using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinScreen : MonoBehaviour
{
    public GameObject winScreenUI; //this is a reference to the win screen UI
    public Image winScreenBackground; //this is a reference to the win screen background
    public TextMeshProUGUI winText; //this is a reference to the win text
    public float displayTime = 5.0f; //this is the amount of time that the win screen will be displayed for
    public Button replayButton; //this is a reference to the replay button

    private float timer = 0.0f; //this is a timer that we can use to determine how long the win screen has been displayed for
    private bool gameIsOver = false; //this is a flag that we can use to determine if the game is over

    void Start()
    {
        replayButton.onClick.AddListener(ReplayGame); //add a listener to the replay button
    }

    public void ShowWinScreen(int winningTeam, Color teamColor) //this function will show the win screen
    {
        winScreenUI.SetActive(true); //set the win screen UI to active
        winScreenBackground.color = teamColor; //set the win screen background color to the team color
        winText.text = "Team " + (winningTeam + 1).ToString() + " Wins!"; //set the win text to the winning team
        winText.color = Color.white; //set the win text color to white
        gameIsOver = true; //set the game is over flag to true
        timer = 0.0f; //reset the timer
        Time.timeScale = 0f; //set the time scale to 0
    }

    private void Update()
    {
        if(gameIsOver) //if the game is over
        {
            timer += Time.unscaledDeltaTime; //increment the timer
            if(timer >= displayTime) //if the timer is greater than or equal to the display time
            {
                winScreenUI.SetActive(false); //set the win screen UI to inactive
                Time.timeScale = 1f; //set the time scale to 1
                gameIsOver = false; //set the game is over flag to false
                GameMenu.isPaused = false; //set the is paused flag to false
            }
        }
    }

    public void ReplayGame() //this function will replay the game
    {
        Time.timeScale = 1f; //set the time scale to 1
        Debug.Log("Restarting Game!"); //log that we are restarting the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reload the scene
    }
}
