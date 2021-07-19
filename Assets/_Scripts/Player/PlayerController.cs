using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Camera Distance not in use")]

    private InputHandler inputHandler;
    private CharacterController pController;
    private Camera mCamera;
    //Local Variables
    public static Transform playerTransform;
    private Quaternion lookDirection;
    private PlayerActions action;
    
    //Player movement/Rotation Values
    [Header("Player Movement")]
    public float rotationSpeed = 10f;
    public float moveSpeed = 5f;
    [Range(0, 7.5f)] public float moveSpeedModifier = 0f;
    public float cameraDistance = 15f;
    private Vector3 targetVector;
    private float hitDistance = 100f;
    private bool canMove = true;


    //Player Abilities
    [Header("Ability Active")]
    public bool isBusy; //Currently preforming an action


    //Focus bar
    [Header("Current Focus")]
    public int focus;

    //Dash
    [Header("Dash Values")]
    public float dashTime = 0.3f;
    public float dashSpeed = 20f;
    public float dashResetTime = 0.3f;

    //Basic Attack
    [Header("Basic Attack Values")]
    public float attackRange = 5f;
    [Range(0, 360)] public float attackArc = 150f;
    public float attackResetTime = 1f;
    //[HideInInspector]
    public List<GameObject> targetList;
    public int attackDamage = 10;
    public int attackDamageMultiplier = 1;
    
    //Attack Combo
    private float lastAttackTime, lastHoldTime;
    private int comboCounter;
    public float maxComboDelay = 0.9f;

    [Header("Slash Attack")]
    public float slashRange = 5f;
    [Range(0, 360)] public float slashArc = 50f;

    //Hold Attack
    [Header("KnockBack Values")]
    [SerializeField] private bool isHolding;
    [SerializeField] private bool isSpamming;
    public float holdDetect = 0.1f;

    //private float[] attackTimes;


    private void Awake()
    {
        inputHandler = InputHandler.instance;
        pController = GetComponent<CharacterController>();//Find attached component
        mCamera = Camera.main;
        playerTransform = this.transform;
    }

    void Start()
    {
        isHolding = false;
        isSpamming = false;
    }
        

    void Update()
    {
        if (canMove)
        {
            targetVector = new Vector3(inputHandler.movementVector.x, 0f, inputHandler.movementVector.z);//Input converted into Vector3
            MoveToTarget(targetVector);
        }
        else
        {
            Vector3 currentPos = new Vector3();

            currentPos = playerTransform.position;

            transform.position = currentPos;
        }
    
        //RotateToTarget(targetVector);

        float timeHeld = Time.time - lastHoldTime;

        if(inputHandler.GetKeyDown(PlayerActions.Attack) && !isBusy)
        {
            lastHoldTime = Time.time;
            //RotateToClickLocation();
            isHolding = false;
        }

        if (inputHandler.GetKeyUp(PlayerActions.Attack) && isHolding)
        {
            isHolding = false;
            isBusy = false;
        }
        else if (inputHandler.GetKeyUp(PlayerActions.Attack) && !isBusy && !isHolding)
        {
            //Face Click Location
            //RotateToClickLocation();
            isHolding = false;
            StartCoroutine(Attack(attackResetTime));
        }
        else if (inputHandler.GetKey(PlayerActions.Attack) && !isBusy)
        {
            if (Time.time - lastHoldTime > holdDetect && !isHolding)
            {
                isHolding = true;
                Debug.Log("Held: " + Time.time + " :: " + lastHoldTime + " :: " + (Time.time - lastHoldTime));
            }

            if(isHolding)
            {
                if(detectAttackable(attackRange, attackArc))
                {
                    foreach(GameObject enemy in targetList)
                    {
                        StartCoroutine(KnockbackTarget(enemy));
                        isBusy = true;
                        Debug.Log("Boop");
                    }
                }
            }
        }     

        if (inputHandler.GetKeyDown(PlayerActions.Slash) && !isBusy)
        {
            //RotateToClickLocation();
            StartCoroutine(Slash(attackResetTime));
        }


        
        if (comboCounter >= 1 && (Time.time - lastAttackTime) > maxComboDelay)        
            comboCounter = 0;   
    }

    Vector3 lastPosition = Vector3.zero;
    void FixedUpdate()
    {
        float sp = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;

        //Debug.Log("Speedz: " + sp);
    }

    private void MoveToTarget(Vector3 target) //Move and then rotate character to target direction
    {
 
        float speed = (moveSpeed + moveSpeedModifier) * Time.deltaTime;
        target = Quaternion.Euler(0f, mCamera.transform.eulerAngles.y, 0f) * target;

        pController.Move(target.normalized * speed);   

        if (target != Vector3.zero && !isBusy) //If input ongoing, update player rotation
        {
            lookDirection = Quaternion.LookRotation(target);
            pController.transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed);
        }
    }    

    private Vector3 RotateToClickLocation()
    {   
        //Character rotate to mouse on screen position
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out hitDistance))
        {
            //Debug.DrawRay(ray.origin, ray.direction * hitDistance, Color.red);            
            Vector3 targetPoint = ray.GetPoint(hitDistance);
            lookDirection = Quaternion.LookRotation(targetPoint - transform.position);
            pController.transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed * 100f);

            return targetPoint;
        }
        else
            return Vector3.zero;
    }

    private void RotateToTarget(Transform target)
    {
        lookDirection = Quaternion.LookRotation(target.position - transform.position);
        pController.transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed*100f);
    }



    public IEnumerator Dash(float resetTime)
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

    private bool detectAttackable(float range, float Arc)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, 1 << 8); //create sphere around player with radius of 3
        targetList = new List<GameObject>();

        if (colliders.Length > 0)
        {
            foreach (Collider target in colliders)
            {
                Vector3 targetDirection = (target.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(targetDirection, transform.forward);

                if (angle <= Arc / 2)
                {
                    targetList.Add(target.gameObject);
                }
            }

            return true;
        }
        else
            return false;
    }


    IEnumerator Attack(float resetTime)
    {
        canMove = false;
        isBusy = true;
        //targetVector = Vector3.zero;
        
        //Animate
        attackDamageMultiplier = 1;
        RotateToClickLocation();

        /**if (colliders.Length > 0)
        {
            //float currentTime = Time.time;        
            comboCounter++;
            lastAttackTime = Time.time;
            Debug.Log("Combo: " + comboCounter);

            foreach (Collider target in colliders)
            {
                Vector3 targetDirection = (target.transform.position - transform.position).normalized;

                float angle = Vector3.Angle(targetDirection, transform.forward);
                //Debug.Log(target.name + ": " + targetDirection + "   Angle:" + angle);

                if (angle <= attackArc/2)
                {
                    HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>();

                    Vector3 targetFacing = (enemyHealth.transform.forward - transform.position).normalized;

                    if (enemyHealth.GetHealth() > 0 && comboCounter == 4)
                    {
                        attackDamageMultiplier += 2;
                        Debug.Log("Combo Attack Complete");
                        yield return KnockbackTarget(enemyHealth.gameObject);
                        resetTime = 0.5f;
                        comboCounter = 0;
                    }

                    if(Vector3.Dot(targetFacing, transform.forward) < 0.05f)
                    {
                        attackDamageMultiplier += 2;
                        Debug.Log("Sneak Attack Multiplier Added");
                    }

                    enemyHealth.Damage(attackDamage * attackDamageMultiplier);
                    //Debug.Log("Target facing: " + Vector3.Dot(targetFacing, transform.forward));                    
                    yield return null;
                }
                else
                {
                    //Debug.Log("Enemy Not within attack angle");
                    yield return null;
                }
            }
        }
        else
        {
            //Debug.Log("No Target In Range");
            comboCounter = 0;
        }*/

        if (detectAttackable(attackRange, attackArc))
        {
            comboCounter++;
            lastAttackTime = Time.time;

            foreach (GameObject target in targetList)
            {
                HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>();
                Vector3 targetFacing = (enemyHealth.transform.forward - transform.position).normalized;

                if (enemyHealth.GetHealth() > 0 && comboCounter == 4)
                {
                    attackDamageMultiplier *= 2;
                    Debug.Log("Combo Attack Complete");
                    yield return KnockbackTarget(enemyHealth.gameObject);
                    resetTime = 0.5f;
                    comboCounter = 0;
                }

                if (Vector3.Dot(targetFacing, transform.forward) < 0.05f)
                {
                    attackDamageMultiplier *= 2;
                    Debug.Log("Sneak Attack Multiplier Added");
                }

                enemyHealth.Damage(attackDamage * attackDamageMultiplier);

                //focus += 10 * attackDamageMultiplier;
                Debug.Log("Attacked: " + target.name + " / Current Health: " + enemyHealth.GetHealth() + " / Target facing: " + targetFacing + " / Damage Done: " + attackDamage * attackDamageMultiplier);
            }
        }
        else
            comboCounter = 0;

        yield return new WaitForSeconds(0.2f);
        canMove = true;

        attackDamageMultiplier = 1;  
        //Debug.Log("Attack Reset Time: " + resetTime);
        yield return new WaitForSeconds(resetTime);
        resetTime = 0.3f;
        isBusy = false;
    }

    IEnumerator Slash(float resetTime)
    {
        isBusy = true;

        bool slicable = false;

        RotateToClickLocation();
        

        if (detectAttackable(slashRange, slashArc))
        {
            List<GameObject> targetEnemyList = new List<GameObject>();

            foreach (GameObject target in targetList)
            {
                HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>();

                if (enemyHealth.currentHealth <= 0) //Enemy should be in utility state
                {
                    targetEnemyList.Add(enemyHealth.gameObject);
                    slicable = true;
                    Physics.IgnoreLayerCollision(0, 8, true);
                    StartCoroutine(Dash(0f));
                }
            }

            if (slicable)
            {
                //Become temp invuln
                if (targetEnemyList.Count > 1)
                {
                    GameObject targetEnemy = new GameObject();
                    float dist = 100f;

                    foreach(GameObject target in targetEnemyList)
                    {
                        if(Vector3.Distance(transform.position, target.transform.position) < dist)
                        {
                            targetEnemy = target;
                        }
                    }

                    
                    //RotateToTarget(targetEnemy.transform);
                    

                    //Set off utility state explosion
                }
                else
                {
                    //Physics.IgnoreLayerCollision(0, 8, true);
                    RotateToTarget(targetEnemyList[0].transform);
                    StartCoroutine(Dash(0f));

                    //Set off utility state explosion
                }
            }
        }

        yield return new WaitForSeconds(resetTime);
        Physics.IgnoreLayerCollision(0, 8, false);
        isBusy = false;

    }

     


    IEnumerator KnockbackTarget(GameObject target)
    {
        isBusy = true;

        float startTime = Time.time;

        Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;

        while (Time.time < startTime + dashTime)
        {
            target.GetComponent<CharacterController>().Move(knockbackDirection * dashSpeed * Time.deltaTime);

            yield return null;
        }

        yield return null;

        isBusy = false;
    }
}
