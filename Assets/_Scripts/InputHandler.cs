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
    public Vector3 rotationVector { get; private set;}

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
        //Player movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        movementVector = new Vector3(x, 0f, z);

        //Player mouse direction input
        //float 
        //rotationVector = new Vector3(0, 0, 0);
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
                return Input.GetKeyDown(key.keyCode); //If key exists, log key event
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
                return Input.GetKeyDown(key.keyCode); //If key exists, log key event
            }
        }

        return false;
    }
}

public enum PlayerActions
{
    Dash,
    Attack,
    HunterMark
}