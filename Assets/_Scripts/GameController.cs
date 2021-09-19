using System;
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
    public GameObject worldTimerDisplay;
    public Text worldTimer;
    public int startingTime;



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
        WorldTimer();
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

    private void WorldTimer()
    {

    }

    public void ModifyWorldTimer(int minutes, int seconds)
    {

    }

    public string FormatTime(float time)
    {
        int cTime = (int)time;
        int minutes = cTime / 60;
        int seconds = cTime % 60;
        float milliSeconds = time * 1000;
        milliSeconds = (milliSeconds % 1000);

        string timeString = String.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
        return timeString;
    }

    // will refactor this as we progress, this was a basic test
}
