using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject unitPrefab; // Prefab for the unit
    [SerializeField]
    private float spawnInterval; // The interval between each spawn

    private float spawnTimer; // Timer to track when to spawn next unit
    private int team; // The team this spawner belongs to

    public void Initialize(GameObject unitPrefab, float spawnInterval, int team)
    {
        this.unitPrefab = unitPrefab;
        this.spawnInterval = spawnInterval;
        this.team = team; // Set the team when initializing the spawner
    }
    
    public int Team
    {
        set { team = value; } // Allow the team to be set publicly
    }

    public void UpdateSpawner()
    {
        // Increase the timer by the time since the last frame
        spawnTimer += Time.deltaTime;

        // If enough time has passed, spawn a new unit
        if (spawnTimer >= spawnInterval)
        {
            SpawnUnit();
            spawnTimer = 0;
        }
    }

    private void SpawnUnit()
    {
        // Instantiate a new unit at the position of the spawner
        GameObject unitInstance = Instantiate(unitPrefab, transform.position, transform.rotation);

        // Set the unit's team color based on the team of the spawner
        // Assuming the unit has a method to set its color
        Unit unitComponent = unitInstance.GetComponent<Unit>();
        if (unitComponent != null)
        {
            unitComponent.Team = team;
        }
    }
}
