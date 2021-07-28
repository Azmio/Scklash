using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    static public GameController instance; //Only 1 instance remain active

    public InputHandler inputHandler; //Player Controls

    static public bool isPlayerDashing;

    public Vector3 toFollow;
    public PlayerController Player;

    //GUI
    public Slider focusSlider;


    void Awake() //Set up
    {
        if (instance == null)
        {
            instance = this;
            inputHandler = InputHandler.instance;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    void Update()
    {
        UpdateToFollow(isPlayerDashing);
        
        //Player.GetComponent<PlayerController>().focus = //Utility State enemies in list
    }

    void UpdateToFollow(bool dashCheck)
    {
        if (dashCheck)
        {
            return;// if player is dashing dont give the current position of the player to the enemy and return the old one
        }

        toFollow = Player.gameObject.transform.position;
    }

    // will refactor this as we progress, this was a basic test
}
