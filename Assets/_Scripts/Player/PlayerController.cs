using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{    
    private InputHandler inputHandler;
    private CharacterController pController;
    private Camera mCamera;

    //Local Variables
    public static Transform playerTransform;
    private Quaternion lookDirection;
    private PlayerActions action;
    
    //Player movement/Rotation Values
    public float rotationSpeed = 10f;
    public float moveSpeed = 5f;
    public float cameraDistance = 15f;

    private float hitDistance = 30f;

    //Player Abilities
    public bool isBusy; //Currently preforming an action
    public float attackResetTime = 1f;
    public float dashTime = 0.3f;
    public float dashSpeed = 20f;
    public float dashResetTime = 0f;

    private void Awake()
    {
        inputHandler = InputHandler.instance;
        pController = GetComponent<CharacterController>();//Find attached component
        mCamera = Camera.main;
        playerTransform = this.transform;
    }

    void Start()
    {
        
    }
        
    void Update()
    {     
        Vector3 targetVector = new Vector3(inputHandler.movementVector.x, 0f, inputHandler.movementVector.z);//Input converted into Vector3


        MoveToTarget(targetVector);
        //RotateToTarget();
        if(inputHandler.GetKeyDown(PlayerActions.Dash) && !isBusy)
        {
           StartCoroutine(Dash(dashResetTime));
            Debug.Log("DAAAAAAAASSSSHHHHH");
        }
        else if(inputHandler.GetKeyDown(PlayerActions.Attack) && !isBusy)
        {
            Debug.Log("CHAAAAAAAAAAA");
        }

        /*if (Input.GetKeyDown(KeyCode.Space)) // this is a test do refactor later if you want 
        {
            GameController.isPlayerDashing = true;
            pController.Move(transform.forward * moveSpeed/2); //just to test because pressing space and seeing if the enemy followed the old position was lame
            StartCoroutine(ResetDash(1f));
        }*/
    }

    private void MoveToTarget(Vector3 target) //Move and then rotate character to target direction
    {
        float speed = moveSpeed * Time.deltaTime;
        target = Quaternion.Euler(0f, mCamera.transform.eulerAngles.y, 0f) * target;
        pController.Move(target * speed);        

        if (target != Vector3.zero) //If input ongoing, update player rotation
        {
            lookDirection = Quaternion.LookRotation(target);
            pController.transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed);
        }
    }    

    private void RotateToTarget()
    {
        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, hitDistance))
        {
            Vector3 targetPoint = ray.GetPoint(hit.distance);
            lookDirection = Quaternion.LookRotation(targetPoint);
            pController.transform.rotation = Quaternion.Slerp(pController.transform.rotation, lookDirection, rotationSpeed * Time.deltaTime);
        }
    }
    
    IEnumerator Dash(float resetTime)
    {
        float startTime = Time.time;

        isBusy = true;
        GameController.isPlayerDashing = true;
        
        while(Time.time < startTime + dashTime)
        {
            pController.Move(pController.transform.forward * dashSpeed * Time.deltaTime);

            yield return null;
        }

        //Animate
        //new WaitForSeconds(1f);
        isBusy = false;
        Debug.Log("tworks");
        yield return new WaitForSeconds(resetTime);
        GameController.isPlayerDashing = false;
    }
}
