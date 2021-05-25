using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static public GameController instance;

    public InputHandler inputHandler;

    static public bool isPlayerDashing;

    public Vector3 toFollow;
    public GameObject Player;


    // Start is called before the first frame update
    void Awake()
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

    // Update is called once per frame
    void Update()
    {
        UpdateToFollow(isPlayerDashing);
    }

    void UpdateToFollow(bool dashCheck)
    {
        if (dashCheck)
        {
            return;// if player is dashing dont give the current position of the player to the enemy and return the old one
        }
        toFollow = Player.transform.position;
    }

    // will refactor this as we progress, this was a basic test
}
