using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{      
    public CharacterController pController;
    private Camera mCamera;

    public static Transform playerTransform;
    
    public float rotationSpeed = 5f;
    public float moveSpeed = 5f;
    public float cameraDistance = 15f;

    private void Awake()
    {
        pController = GetComponent<CharacterController>();//Find attached component
        mCamera = Camera.main;
        playerTransform = this.transform;
    }

    void Start()
    {
        
    }
        
    void Update()
    {     
        Vector3 targetVector = new Vector3(InputHandler.movementVector.x, 0f, InputHandler.movementVector.z);//Input converted into Vector3
        MoveToTarget(targetVector);
    }

    private void MoveToTarget(Vector3 target) //Move and then rotate character to target direction
    {
        float speed = moveSpeed * Time.deltaTime;
        target = Quaternion.Euler(0f, mCamera.transform.eulerAngles.y, 0f) * target;
        pController.Move(target * speed);

        Quaternion lookDirection;

        if (target != Vector3.zero) //If input ongoing, update player rotation
        {
            lookDirection = Quaternion.LookRotation(target);
            pController.transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed);
        }        
    }    
}
