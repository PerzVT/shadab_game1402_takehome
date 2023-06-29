using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outpost : MonoBehaviour
{
    [SerializeField] private float flagTop; //this is the top of the flag
    [SerializeField] private float flagBottom; //this is the bottom of the flag
    [SerializeField] private GameObject aiUnitPrefab; //this is the unit we are going to spawn
    [SerializeField] private float spawnInterval = 3.0f; // Time in seconds between each spawn
    [SerializeField] private float scoreInterval = 5.0f; //how many seconds until someone gets a point
    [SerializeField] private float valueIncrease = 0.005f; //how long for them to increase a value

    private SkinnedMeshRenderer flag; //this is the flag we are going to move
    private int team = -1; //this keeps track of the current team (-1 being no one). internal is public to the project
    private float timer; //this will keep track of the time within the outpost
    private float spawnTimer; // Keeps track of the time for spawning

    internal float currentValue = 0; //this is the current value of the outpost

    // The Team property allows to set team privately and get publicly
    public int Team //this is a property, which is a special kind of function
    {
        get { return team; } //this is the get part of the property
        private set { team = value; } //this is the set part of the property
    }

    // Start is called before the first frame update
    void Start()
    {
        flag = GetComponentInChildren<SkinnedMeshRenderer>(); //this is the flag we are going to move and update
    }

    // Update is called once per frame
    void Update()
    {
        if(team != -1) //recall -1 is the nothing value
        {
            Color teamColor = GameManager.Instance.teams[team]; //this is the color of the team we are on
            //we want to lerp between white and the team color
            flag.material.color = Color.Lerp(Color.white, teamColor, currentValue); //this is the current value of the outpost
            flag.transform.parent.localPosition = new Vector3(0, Mathf.Lerp(flagBottom, flagTop, currentValue), 0); //this is the current value of the outpost
            
            //once our current value is 1, we want to start collecting points
            if(currentValue == 1)
            {
                timer += Time.deltaTime; //change in time in our scene
                if (timer >= scoreInterval)
                {
                    timer = 0; //reset the timer
                    ScoreManager.Instance.scores[team]++; //add 1 to the current team's score
                }

                // Increase the spawn timer
                spawnTimer += Time.deltaTime;

                // Check if it's time to spawn a new unit
                if(spawnTimer >= spawnInterval)
                {
                    // Reset the timer
                    spawnTimer = 0;

                    // Spawn a new AI Unit at the location of the outpost
                    GameObject newUnit = Instantiate(aiUnitPrefab, transform.position, Quaternion.identity);
                    
                    // Change the team of the new unit
                    AIController aiController = newUnit.GetComponent<AIController>();
                    if (aiController != null)
                    {
                        aiController.Team = team;
                        aiController.GetComponentInChildren<Renderer>().material.color = GameManager.Instance.teams[team];
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Unit unit = other.GetComponent<Unit>(); // we need to see if what collided is actually a unit
        if(unit != null) //we only care about things that trigger this that are units
        {
            Debug.Log("We are here to stay"); //this is for debugging purposes
            if(unit.Team == Team) //if the unit is on the same team as the outpost
            {
                //this is for when our current team is staying there
                currentValue += valueIncrease; //this is the same as currentValue = currentValue + valueIncrease
                if(currentValue >= 1) //if the current value is greater than 1, we need to change the team
                {

                    currentValue = 1; //reset the current value
                }
            }
            else
            {
                //this is for when a new team enters. They immediately decrease the currentValue
                currentValue -= valueIncrease; //this is the same as currentValue = currentValue - valueIncrease
                if(currentValue <= 0) //if the current value is less than 0, we need to change the team
                {
                    team = unit.Team; //this is the new team
                    currentValue = 0; //reset the current value
                }
            }
        }
    }
}
