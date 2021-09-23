using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;

    public KeyBinds keyBinds;

    //Accessable player input values
    public Vector3 movementVector {get; private set;}
    public bool isMoving;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    void Update()
    {
        //Player movement input WASD
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");
        movementVector = new Vector3(x, 0f, z); //apply movement

        if (movementVector != Vector3.zero)
            isMoving = true;
        else
            isMoving = false;

        
    }

    public KeyCode GetKeyCode(PlayerActions currentAction) //Check key Code
    {
        foreach(KeyBinds.KeyBindCheck key in keyBinds.keyBindChecks) //Iterate through current keybinds
        {
            if(key.action == currentAction)
            {
                return key.keyCode; //Return key code        
            }
        }

        return KeyCode.None;
    }

    public bool GetKeyDown(PlayerActions currentAction) //Check if key is pressed
    {
        foreach (KeyBinds.KeyBindCheck key in keyBinds.keyBindChecks) //Iterate through current keybinds
        {
            if (key.action == currentAction)
            {
                return Input.GetKeyDown(key.keyCode); //If key exists, log key event
            }
        }

        return false;
    }

    public bool GetKey(PlayerActions currentAction) //Check if key is held
    {
        foreach (KeyBinds.KeyBindCheck key in keyBinds.keyBindChecks) //Iterate through current keybinds
        {
            if (key.action == currentAction)
            {
                return Input.GetKey(key.keyCode); //If key exists, log key event
            }
        }

        return false;
    }

    public bool GetKeyUp(PlayerActions currentAction) //Check if key is released
    {
        foreach (KeyBinds.KeyBindCheck key in keyBinds.keyBindChecks) //Iterate through current keybinds
        {
            if (key.action == currentAction)
            {
                return Input.GetKeyUp(key.keyCode); //If key exists, log key event
            }
        }

        return false;
    }
}

public enum PlayerActions //Player Abilities to key bind
{    
    Attack,
    Slash,
    Bloom,
    Dash,
    Pause
}