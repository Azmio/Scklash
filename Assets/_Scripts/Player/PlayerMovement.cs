using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //private InputHandler input;
    public CharacterController pController;

    public float rotationSpeed = 5f;
    public float moveSpeed = 5f;

    private void Awake()
    {
        pController = GetComponent<CharacterController>();//Find attached component
    }

    void Start()
    {
        
    }
        
    void Update()
    {
        Vector3 targetVector = new Vector3(InputHandler.movementVector.x, 0f, InputHandler.movementVector.z);
        MoveToTarget(targetVector);




        /*
        //Get player WASD input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(x, 0f, z);
        Vector3 movement = transform.right//transform.right * x + transform.forward* z;

        pController.Move(movement * moveSpeed * Time.deltaTime);*/
    }

    private void MoveToTarget(Vector3 target)
    {
        float speed = moveSpeed * Time.deltaTime;
        target = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f) * target;
        pController.Move(target * speed);

        Quaternion lookDirection;

        if (target != Vector3.zero)
        {
            lookDirection = Quaternion.LookRotation(target);
            pController.transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed);
        }
        
        
    }
}
