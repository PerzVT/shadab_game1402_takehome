using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : Unit
{
    private enum State //this is an enum that represents the state of the AI
    {
        Idle,
        MovingToOutpost,
        Chasing
    }

    public Vector3 aimOffset = new Vector3(0, 1.5f, 0); //this is the offset from the center of the unit that we want to aim at
    public float shootInterval = 0.5f; //this is how often we want to shoot
    public float lookDistance = 10; //this is how far we want to look for enemies
    private State currentState; //this is the current state of the AI
    private NavMeshAgent agent; //this is the navmesh agent that we are going to use to move
    private Unit currentEnemy; //this is the current enemy we are chasing
    private Outpost currentOutpost; //this is the current outpost we are moving to

    protected override void Start()
    {
        base.Start(); //call the base start function
        agent = GetComponent<NavMeshAgent>(); //get the navmesh agent
        SetState(State.Idle); //set the state to idle
    }

    private void SetState(State newState)
    {
        currentState = newState; //set the current state to the new state
        StopAllCoroutines(); //stop all coroutines

        switch (currentState)
        {
            case State.Idle:
                StartCoroutine(OnIdle());
                break;
            case State.MovingToOutpost:
                StartCoroutine(OnMovingToOutpost());
                break;
            case State.Chasing:
                StartCoroutine(OnChasing());
                break;
        }
    }

    private IEnumerator OnIdle() //this is the idle state
    {
        currentOutpost = null;
        while (currentOutpost == null)
        {
            if(isAlive)
                LookForOutposts();
            yield return null; 
        }
        SetState(State.MovingToOutpost);
    }

    private IEnumerator OnMovingToOutpost() //this is the moving to outpost state
    {
        agent.SetDestination(currentOutpost.transform.position); 
        while(!(currentOutpost.Team == Team  && currentOutpost.currentValue == 1))
        {
            LookForEnemies();
            yield return null;
        }
        currentOutpost = null;
        SetState(State.Idle);
    }

    private IEnumerator OnChasing() //this is the chasing state
    {
        agent.ResetPath(); //reset the path
        float shootTimer = 0; //this is the timer for shooting
        while (currentEnemy.isAlive) //while the enemy is alive
        {
            shootTimer += Time.deltaTime;
            float distanceToEnemy = Vector3.Distance(currentEnemy.transform.position, this.transform.position);

            if(distanceToEnemy > lookDistance || !CanSee(currentEnemy.transform, currentEnemy.transform.position + aimOffset))
            {
                agent.SetDestination(currentEnemy.transform.position);
            }
            else if (shootTimer > shootInterval) //if the shoot timer is greater than the shoot interval
            {
                agent.ResetPath(); //reset the path
                shootTimer = 0;
                Vector3 dir = currentEnemy.transform.position - this.transform.position;
                dir.Normalize();

                LayerMask mask = ~LayerMask.GetMask("Outpost", "Terrain"); //we want to ignore the outpost and terrain layers
                Ray ray = new Ray(GetEyesPosition(), dir); //create a ray from our eyes position to the enemy
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                {
                    ShootAt(hit);
                }
                else
                {
                    Vector3 targetPos = GetEyesPosition() + dir * DISTANCE_LASER_IF_NO_HIT; //if we didn't hit anything, we want to show the laser at the max distance
                    ShowLasers(targetPos);
                }
            }
            yield return null;  
        }
        currentEnemy = null;
        SetState(State.Idle); //set the state to idle
    }

    private void LookForEnemies() //this is the function that looks for enemies
    {
        Collider[] surroundingColliders = Physics.OverlapSphere(this.transform.position, lookDistance); //get all the colliders in a sphere around the AI
        foreach(Collider coll in surroundingColliders)
        {
            Unit unit = coll.GetComponent<Unit>();
            if(unit != null)
            {
                if(unit.Team != Team && CanSee(unit.transform, unit.transform.position + aimOffset) && unit.isAlive)
                {
                    currentEnemy = unit; //set the current enemy to the unit
                    SetState(State.Chasing); //set the state to chasing
                }
            }
        }
    }

    private void LookForOutposts()
    {
        if(GameManager.Instance.outposts.Count > 0) //if there are outposts in the game
        {
            int r = Random.Range(0, GameManager.Instance.outposts.Count); //get a random outpost
            if(GameManager.Instance.outposts[r].Team != Team)
                currentOutpost = GameManager.Instance.outposts[r]; //set the current outpost to the random outpost
        }
    }
    
    void Update()
    {
        animator.SetFloat("VerticalSpeed", agent.velocity.magnitude); //set the vertical speed to the magnitude of the agent's velocity
    }
}
