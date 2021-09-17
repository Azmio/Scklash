using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform placeHolder; //Parent Gameobject

    public float camDistance = 15f;

    void Update()
    {
        if(GameController.instance.Player.transform != null)
            placeHolder.transform.position = GameController.instance.Player.transform.position;//Follow player without being attached as child
    }
}
