using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] int fullHealth = 100; // the full health of the unit
    [SerializeField] public int team; // the team the unit is on
    [SerializeField] int health; // the current health of the unit
    [SerializeField] int damage = 10; // the damage the unit does
    [SerializeField] Laser laserPrefab;

    protected Animator animator; // the animator of the unit
    protected Rigidbody rb; // the rigidbody of the unit
    protected const float DISTANCE_LASER_IF_NO_HIT = 500.0f; // the distance the laser will go if it doesn't hit anything

    private const float RAYCAST_LENGTH = 0.3f; // the length of the raycast to check if the unit is grounded
    private Color myColor; // the color of the unit
    private Eye[] eyes = new Eye[2]; // the eyes of the unit
    public float viewAngle = 80; // the angle of the unit's vision

    internal bool isAlive = true; // whether the unit is alive or not
    Vector3 startPos; // the starting position of the unit
    public float respawnTime = 5.0f; // the time it takes for the unit to respawn

    protected virtual void Start()
    {
        animator = GetComponent<Animator>(); // Get the animator component
        eyes = GetComponentsInChildren<Eye>(); // Get the eyes of the unit
        myColor = GameManager.Instance.teams[team]; // Get the color of the unit
        transform.Find("Teddy_Body").GetComponent<SkinnedMeshRenderer>().material.color = myColor; // Set the color of the unit
        startPos = this.transform.position; // Get the starting position of the unit
        Respawn();
    }

    public int Team
    {
        get
        {
            return team; // Get the team of the unit
        }
         set
        {
            team = value; // Set the team of the unit
        }
    }

    protected virtual void OnHit(Unit attacker)
    {
        health -= attacker.damage; // Decrease the health of the unit
        if(health <= 0) // If the unit has no health left, die
        {
            Die();
        }
    }

    protected bool CanSee(Transform target, Vector3 targetPosition)
    {
        Vector3 startPos = GetEyesPosition(); // Get the position of the eyes
        Vector3 dir = targetPosition - startPos; // Get the direction to the target
        if (Vector3.Angle(transform.forward, dir) > viewAngle) // If the target is outside of the unit's vision, return false
            return false;

        Ray ray = new Ray(startPos, dir); // Create a ray from the eyes to the target
        LayerMask mask = ~LayerMask.GetMask("Outpost"); // Ignore the outpost layer
        RaycastHit hit; // The hit info from the raycast
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) // If the raycast hits something
        {
            if (hit.transform != target) // If the raycast doesn't hit the target, return false
                return false;
        }
        else // If the raycast doesn't hit anything, return false
        {
            return false;
        }
        {
            if(hit.transform != target)
            {
                return false;
            }
        }
        return true;
    }

    protected Vector3 GetEyesPosition() // Get the position of the eyes
    {
        return (eyes[0].transform.position + eyes[1].transform.position) / 2.0f; // Return the average position of the eyes
    }

    protected void ShootAt(RaycastHit hit) // Shoot at the target
    {
        Unit unit = hit.transform.GetComponent<Unit>(); // Get the unit component of the target
        if(unit != null)
        {
            if(unit.team != team) // If the target is on a different team, shoot at it
            {
                unit.OnHit(this); // Tell the target it has been hit
                ShowLasers(hit.point); // Show the lasers
            }
        }
    }

    protected void ShowLasers(Vector3 targetPosition)
    {
        foreach(Eye eye in eyes) // For each eye, show a laser
        {
            Laser laser = Instantiate(laserPrefab) as Laser; // Create a laser
            laser.Init(myColor, eye.transform.position, targetPosition); // Initialize the laser
        }
    }

    void Update()
    {
        //perz was here
    }

    protected bool IsGrounded()
    {
        Vector3 origin = transform.position; // Get the position of the unit
        origin.y += RAYCAST_LENGTH * 0.5f; // Move the origin up
        LayerMask mask = LayerMask.GetMask("Terrain"); // Get the terrain layer
        return Physics.Raycast(origin, Vector3.down, RAYCAST_LENGTH, mask); // Return whether the unit is grounded or not
    }

    protected virtual void Die() // Kill the unit
    {
        if (!isAlive) // If the unit is already dead,
            return; 

        gameObject.layer = LayerMask.NameToLayer("DeadTeddy"); // Set the layer of the unit to dead
        isAlive = false; // Set the unit to dead
        animator.SetBool("Dead", true); // Play the death animation
        Invoke("Respawn", respawnTime); // Respawn the unit after a delay
    }

    protected virtual void Respawn()
    {
        isAlive = true; // Set the unit to alive
        gameObject.layer = LayerMask.NameToLayer("LiveTeddy"); // Set the layer of the unit to live
        health = fullHealth; // Set the health of the unit to full
        this.transform.position = startPos; // Move the unit to the starting position
        animator.SetBool("Dead", false); // Stop the death animation
    }
}
