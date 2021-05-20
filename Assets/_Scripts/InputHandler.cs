using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputHandler : MonoBehaviour
{
    //Accessable player input values
    public static Vector3 movementVector{get; private set;}//WSAD

    void Update()
    {
        //Player directional input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        movementVector = new Vector3(x, 0f, z);
    }
}
