using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{    
    private InputHandler inputHandler;
    private CharacterController pController;
    private Camera mCamera;
    public GameObject item;
    //Local Variables
    public static Transform playerTransform;
    private Quaternion lookDirection;
    private PlayerActions action;
    
    //Player movement/Rotation Values
    public float rotationSpeed = 10f;
    public float moveSpeed = 5f;
    public float cameraDistance = 15f;

    private float hitDistance = 100f;

    //Player Abilities
    public bool isBusy; //Currently preforming an action
    //Dash
    public float dashTime = 0.3f;
    public float dashSpeed = 20f;
    public float dashResetTime = 0f;
    //Basic Attack
    public float attackRange = 3f;
    [Range(0, 360)] public float attackArc = 150f;
    public float attackResetTime = 1f;
    public Collider[] colliders;
    public int attackDamage = 10;
    public int attackDamageMultiplier = 3;

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
        //RotateToTarget(targetVector);

        if(inputHandler.GetKeyDown(PlayerActions.Dash) && !isBusy)
        {
            StartCoroutine(Dash(dashResetTime));
            //Debug.Log("DAAAAAAAASSSSHHHHH");
        }
        if(inputHandler.GetKeyDown(PlayerActions.Attack) && !isBusy)
        {
            //Face Click Location
            RotateToTarget();

            StartCoroutine(Attack(attackResetTime));
            //Debug.Log("CHAAAAAAAAAAA");
        }
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
        //Character rotate to mouse on screen position
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);

        if(plane.Raycast(ray, out hitDistance))
        {
            //Debug.DrawRay(ray.origin, ray.direction * hitDistance, Color.red);            
            Vector3 targetPoint = ray.GetPoint(hitDistance);
            lookDirection = Quaternion.LookRotation(targetPoint-transform.position);
            pController.transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed*100f);
        }
    }

    IEnumerator Dash(float resetTime)
    {
        float startTime = Time.time;

        isBusy = true;
        GameController.isPlayerDashing = true;

        //Animate

        while (Time.time < startTime + dashTime)
        {
            pController.Move(pController.transform.forward * dashSpeed * Time.deltaTime);

            yield return null;
        }
        
        isBusy = false;
        yield return new WaitForSeconds(resetTime);
        GameController.isPlayerDashing = false;
    }

    IEnumerator Attack(float resetTime)
    {
        isBusy = true;
        //Animate

       
        //yield return new WaitForSeconds(0.5f);

        colliders = Physics.OverlapSphere(transform.position, attackRange, 1<<8); //create sphere around player with radius of 3
        
        if (colliders.Length > 0)
        {
            //Debug.Log("ENEMY Located");
            foreach (Collider target in colliders)
            {
                Vector3 targetDirection = (target.transform.position - transform.position);

                float angle = Vector3.Angle(targetDirection, transform.forward);

                //Debug.Log(target.name + ": " + targetDirection + "   Angle:" + angle);

                if (angle <= attackArc/2)
                {
                    HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>();

                    Vector3 targetFacing = (enemyHealth.transform.forward - transform.position).normalized;

                    if(Vector3.Dot(targetFacing, transform.forward) < 0.05f)
                    {
                        enemyHealth.Damage(attackDamage*attackDamageMultiplier);
                        Debug.Log("Target Facing Away From Player");
                    }
                    else
                    {
                        enemyHealth.Damage(attackDamage);
                        Debug.Log("Target Facing Towards Player");
                    }
                    
                    Debug.Log("Target facing: " + Vector3.Dot(targetFacing, transform.forward));
                    
                    yield return null;
                }
                else
                {
                    Debug.Log("Enemy Not within attack angle");
                    yield return null;
                }
            }
        }
        else
            Debug.Log("No Enemy Near");


        isBusy = false;
        yield return new WaitForSeconds(resetTime);
    }
}
