using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    private Camera playerCam; //This is the camera that is attached to the player
    private Transform camContainer; //This is the transform of the camera's parent

    [SerializeField] float speed = 5; //This is the speed of the player
    [SerializeField] float mouseXSensitivity = 1; //This is the sensitivity of the mouse
    [SerializeField] float mouseYSensitivity = 1; //This is the sensitivity of the mouse
    [SerializeField] float jumpHeight = 15.0f; //This is the height of the jump
    [SerializeField] float invert = 1.0f; //This is the invert of the mouse

    private const float ANIMATOR_SMOOTHING = 0.4f; //This is the smoothing of the animator
    private Vector3 animatorInput; //This is the input for the animator

    // Start is called before the first frame update
    protected override void Start()
    {

        base.Start();
        playerCam = GetComponentInChildren<Camera>(); //This gets the camera that is attached to the player
        camContainer = playerCam.transform.parent; //This gets the transform of the camera's parent
    }
    // Update is called once per frame
    void Update()
    {
        camContainer.Rotate(invert * Input.GetAxis("Mouse Y") * mouseYSensitivity, 0, 0); //This rotates the camera's parent
        float rotationX = Input.GetAxis("Mouse X") * mouseXSensitivity; //This gets the rotation of the mouse
        this.transform.Rotate(0, rotationX, 0); //This rotates the player
        float horizontal = Input.GetAxis("Horizontal"); //This gets the horizontal input
        float vertical = Input.GetAxis("Vertical"); //This gets the vertical input
        Vector3 input = new Vector3(horizontal, 0, vertical).normalized * speed; //This creates a vector3 for the input
        animatorInput = Vector3.Lerp(animatorInput, input, ANIMATOR_SMOOTHING); //This lerps the animator input
        animator.SetFloat("HorizontalSpeed", animatorInput.x); //This sets the horizontal speed of the animator
        animator.SetFloat("VerticalSpeed", animatorInput.z); //This sets the vertical speed of the animator
        if (Input.GetButtonDown("Jump") && IsGrounded()) //This checks if the player is pressing the space bar and is grounded
        {
            //Debug.Log("Jump jump. Kriss kross will make you jump jump");
            input.y = jumpHeight; //This sets the y velocity of the player
            animator.SetTrigger("Jumping"); //This sets the trigger for the jumping animation
        }
        else
        {
            input.y = GetComponent<Rigidbody>().velocity.y; //This sets the y velocity of the player
        }
     
        if (Input.GetButtonDown("Fire1")) //This checks if the player is pressing the left mouse button
        {
            LayerMask mask = ~LayerMask.GetMask("Outpost", "Teddy", "Terrain"); //This creates a layer mask

            Ray ray = new Ray(GetEyesPosition(), playerCam.transform.forward); //This creates a ray
            RaycastHit hit; // This creates a raycast hit
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) //This checks if the raycast hits something
            {
                ShootAt(hit); //This calls the shoot at function and passes the raycast hit
            }
            else
            {
                Vector3 targetPos = GetEyesPosition() + playerCam.transform.forward * DISTANCE_LASER_IF_NO_HIT; //This creates a vector3 for the target position
                ShowLasers(targetPos); //This calls the show lasers function
            }
        }
        if (Input.GetButtonDown("Fire2")) //This checks if the player is pressing the right mouse button
        {
            animator.SetTrigger("Taunt"); //This sets the trigger for the taunt animation
        }
        GetComponent<Rigidbody>().velocity = transform.TransformVector(input); //This sets the velocity of the player
    }
}
