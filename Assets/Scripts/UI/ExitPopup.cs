using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExitPopup : MonoBehaviour
{
    public void OnNoButton()
    {
        GameMenu.isPaused = false; //we created a static variable isPaused which is why we can change the value
        gameObject.SetActive(false); //we are setting the game object to false
    }
    public void OnYesButton()
    {
        EditorApplication.ExitPlaymode();  //this is a function that will exit play mode
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
