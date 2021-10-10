using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    static public GameController instance; //Only 1 instance remain active

    public InputHandler inputHandler; //Player Controls

    public EnemySpawner enemySpawner;

    static public bool isPlayerDashing;
    public bool gameStarted;

    public Vector3 toFollow;
    public PlayerController Player;

    //GUI
    public GameObject worldTimerDisplay;
    public Text worldTimer;
    public float startingTime;

    public bool isPaused;

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

        gameStarted = false;
        isPaused = false;
    }

    private void Start()
    {
        //startingTime = startingTime * 60;
    }

    void Update()
    {
        UpdateToFollow(isPlayerDashing);
        worldTimer.text = startingTime.ToString("F0");
        startingTime -= Time.deltaTime;
        //Debug.Log(FormatTime(startingTime, Time.deltaTime));
        //Player.GetComponent<PlayerController>().focus = //Utility State enemies in list

        if (inputHandler.GetKeyDown(PlayerActions.Pause))
        {
            if(isPaused)
            {
                Time.timeScale = 1;
                isPaused = false;
            }
            else if(!isPaused)
            {
                Time.timeScale = 0;
                isPaused = true;
            }
        }
    }

    void UpdateToFollow(bool dashCheck)
    {
        if (dashCheck)
        {
            return;// if player is dashing dont give the current position of the player to the enemy and return the old one
        }

        toFollow = Player.gameObject.transform.position;
    }

    public void ModifyWorldTimer(int minutes, int seconds)
    {

    }

    public string FormatTime(float time)
    {
        int cTime = (int)time;
        int minutes = cTime / 60;
        int seconds = cTime % 60;
        float milliSeconds = time * 100;
        milliSeconds = (milliSeconds % 100);

        string timeString = String.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
        return timeString;
    }

    // will refactor this as we progress, this was a basic test
}
