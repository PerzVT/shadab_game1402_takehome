using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //for our laser class, we are going to have to initialize it with a colour, a start position, and an end position; as well, a laser needs to be responsible for destroying itself after a short period of time     
    [SerializeField] float lifeTime = 0.05f; //this is how long the laser will last before it destroys itself
    private LineRenderer line;
    void Awake()
    {
        line = GetComponent<LineRenderer>(); //we need to get a reference to the line renderer component on this game object
    }
    public void Init(Color c, Vector3 start, Vector3 end) //this is a public function that we can call from other scripts to initialize our laser
    {
        //we need to set where the laser starts and ends as well as its initial colour
        line.SetPosition(0, start);//the beginning of our line
        line.SetPosition(1, end); //the end of our line
        line.startColor = c; //the start colour of our line
        line.endColor = c; //the end colour of our line
        Invoke("DestroyMe", lifeTime);//this will call a function named "DestroyMe" after .5s
    }
    private void DestroyMe() //this is private and therefore it is not a reference to us singing sad songs about heartbreak
    {
        Destroy(this.gameObject);//everything in Unity (a monobehaviour, at least) is a gameObject, and if we destroy it, it is no longer in the scene
    }
    // Update is called once per frame
    void Update()
    {
    }
}
