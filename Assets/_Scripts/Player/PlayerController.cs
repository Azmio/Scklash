using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    internal InputHandler inputHandler;

    [SerializeField]
    internal PlayerHealthScript playerHealth;

    [SerializeField]
    internal PlayerMovementScript movementScript;

    [SerializeField]
    internal PlayerCombatScript combatScript;

    public CharacterController charController;
    public Camera mCamera;

    public GameObject deathTimerDisplay;
    public Text deathTimer;

    public Text health;
    public Text displaySpeedMult;
    //public Quaternion lookRotation;

    [SerializeField]
    internal bool isBusy, isInvulnerable;

    //Animations shiz
    public Animator anim;
    [SerializeField]
    private Vector3 animBlendVector;
    Vector3 animVelocity;
    public float animSmoothTime = 0.1f;

    //int verticalAnimationID;
    //int horizontalAnimationID;
    


    private void Awake()
    {
        charController = GetComponent<CharacterController>();//Find attached component
        mCamera = Camera.main;
        deathTimerDisplay.SetActive(false);

        //verticalAnimationID = Animator.StringToHash("Vertical");
        //horizontalAnimationID = Animator.StringToHash("Horizontal");

    }

    private void Start()
    {
        isBusy = false;
        isInvulnerable = false;


    }

    private void Update()
    {       
        movementScript.MovementUpdate();
        combatScript.CombatUpdate();
        playerHealth.HealthUpdate();

        //animBlendVector = Vector3.SmoothDamp(animBlendVector, inputHandler.movementVector, ref animVelocity, animSmoothTime);

        //anim.SetFloat(verticalAnimationID, animBlendVector.z);
        //anim.SetFloat(horizontalAnimationID, animBlendVector.x);

        anim.SetBool(Animator.StringToHash("isMoving"), inputHandler.isMoving);
        //anim.SetInteger(Animator.StringToHash("AttackIndex"), combatScript.comboIndex);
        //anim.SetBool(Animator.StringToHash("isAttacking"), combatScript.isAttacking);



        //Debug.Log("isMoving: " + inputHandler.isMoving.ToString() + " isAttacking: " + combatScript.isAttacking.ToString() + " comboIndex: " + combatScript.comboIndex.ToString());

        health.text = "HP: " + playerHealth.currentHealth.ToString();
        displaySpeedMult.text = movementScript.speedMultiplier.ToString("F1") + "x";
    }

    

    private void StartUp()
    {
        
    }

    /***
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
    [Range(1, 1.7f)] public float moveSpeedModifier = 1f;
    public float cameraDistance = 15f;
    private Vector3 targetVector;
    private float hitDistance = 100f;
    private bool canMove = true;


    //Player Abilities
    [Header("Ability Active")]
    public bool isBusy; //Currently preforming an action


    //Focus bar
    [Header("Current Focus")]
    [Range(0, 100)] public int focus;
    public int focusValue = 5;

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

    //Slash Attack
    [Header("Slash Attack")]
    public float slashRange = 5f;
    [Range(0, 360)] public float slashArc = 50f;

    //Bloom Attack
    [Header("Bloom Values")]
    public float bloomReset = 0.5f;

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
        GameController.instance.focusSlider.value = focus;
        float timeHeld;

        if (isHolding)
            timeHeld = Time.time - lastHoldTime;

        if (canMove) //player is able to move
        {
            targetVector = new Vector3(inputHandler.movementVector.x, 0f, inputHandler.movementVector.z); //Input converted into Vector3

            MoveToTarget(targetVector);
        }
        else
        {
            Vector3 currentPos = new Vector3();

            currentPos = playerTransform.position; //Pause player position if cannot move

            transform.position = currentPos;
        }        

        if(inputHandler.GetKeyDown(PlayerActions.Attack) && !isBusy) //Detect Main Attack Button Down - Default : LM Button
        {
            lastHoldTime = Time.time; //Record Time
            isHolding = false;
        }

        if (inputHandler.GetKeyUp(PlayerActions.Attack) && isHolding) //Detect Main Attack Button Up
        {
            isHolding = false;
            isBusy = false;
        }
        else if (inputHandler.GetKeyUp(PlayerActions.Attack) && !isBusy && !isHolding) //If not Holding, excecute normal attack
        {
            isHolding = false;
            StartCoroutine(Attack(attackResetTime));
        }
        else if (inputHandler.GetKey(PlayerActions.Attack) && !isBusy) //If Attack Button is active
        {
            if (Time.time - lastHoldTime > holdDetect && !isHolding) //Determine if player is holding button
            {
                isHolding = true;
                //Debug.Log("Held: " + Time.time + " :: " + lastHoldTime + " :: " + (Time.time - lastHoldTime));
            }

            if(isHolding) //Excute Knockback in arc range
            {
                if(detectAttackable(attackRange, attackArc, 8))
                {
                    foreach(GameObject enemy in targetList)
                    {
                        StartCoroutine(KnockbackTarget(transform.position, enemy));
                        isBusy = true;
                    }
                }
            }
        }     

        if (inputHandler.GetKeyDown(PlayerActions.Slash) && !isBusy) //Slash Target - Default : RM Button
        {
            StartCoroutine(Slash(attackResetTime));
        }

        if(inputHandler.GetKeyDown(PlayerActions.Bloom) && !isBusy)
        {
            StartCoroutine(Bloom(bloomReset));
        }
        
        if (comboCounter >= 1 && (Time.time - lastAttackTime) > maxComboDelay) //Combo Counter timer check 
            comboCounter = 0;   
    }
    /**
    Vector3 lastPosition = Vector3.zero;
    void FixedUpdate()
    {
        float sp = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
    }*/

    /***private void MoveToTarget(Vector3 target) //Move and then rotate character to target direction
    {
        float cFocus = (float)focus;
        moveSpeedModifier = Mathf.Lerp(1f, 1.7f, cFocus/100);

        //Debug.Log(moveSpeedModifier);
        float speed = moveSpeed * moveSpeedModifier;
        target = Quaternion.Euler(0f, mCamera.transform.eulerAngles.y, 0f) * target;

        pController.Move(target.normalized * speed * Time.deltaTime);   

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

    Vector3 TargetFacing(Vector3 targetDirection, Vector3 origin)
    {
        return (targetDirection - origin).normalized;
    }

    public IEnumerator Dash(float resetTime) //Dash Ability
    {
        float startTime = Time.time; //Set initial time stamp
        
        isBusy = true;
        GameController.isPlayerDashing = true;

        //Animate

        while (Time.time < startTime + dashTime) //Move player for duration
        {
            pController.Move(pController.transform.forward * dashSpeed * Time.deltaTime);

            yield return null;
        }
        
        isBusy = false;
        yield return new WaitForSeconds(resetTime);
        GameController.isPlayerDashing = false;
        
    }

    private bool detectAttackable(float range, float Arc, int layer) //Detect any attackable objects within arc settings
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, 1 << layer); //create sphere around player with radius of 3
        targetList = new List<GameObject>();

        if (colliders.Length > 0)
        {
            foreach (Collider target in colliders)
            {
                //Vector3 targetDirection = (target.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(TargetFacing(target.transform.position, transform.position), transform.forward);

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


    IEnumerator Attack(float resetTime) //Attack Ability
    {
        canMove = false;
        isBusy = true;
        
        //Animate
        attackDamageMultiplier = 1;
        RotateToClickLocation();

        if (detectAttackable(attackRange, attackArc, 8))
        {
            comboCounter++;
            lastAttackTime = Time.time;

            foreach (GameObject target in targetList)
            {
                HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>(); //Acquire enemy health information                
                //Vector3 targetFacing = (enemyHealth.transform.forward - transform.position).normalized; //Check target facing

                if (enemyHealth.GetHealth() > 0 && comboCounter == 4) //Combo modifier added
                {                    
                    attackDamageMultiplier *= 2;
                    Debug.Log("Combo Attack Complete");
                    yield return KnockbackTarget(transform.position, enemyHealth.gameObject);
                    resetTime = 0.5f;
                    comboCounter = 0;
                }

                if (Vector3.Dot(TargetFacing(enemyHealth.transform.forward, transform.position), transform.forward) < 0.05f) //Sneak modifier added
                {
                    attackDamageMultiplier *= 2;
                    Debug.Log("Sneak Attack Multiplier Added");
                }

                if (!enemyHealth.Damage(attackDamage * attackDamageMultiplier)) //Apply Final Damage
                    focus += 5;                                                 //returns true if target is now dead
                                
                //Debug.Log("Attacked: " + target.name + " / Current Health: " + enemyHealth.GetHealth() + " / Target facing: " + targetFacing + " / Damage Done: " + attackDamage * attackDamageMultiplier);
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

    IEnumerator Slash(float resetTime) //Slash Ability
    {
        isBusy = true;

        bool slicable = false;

        RotateToClickLocation();        

        if (detectAttackable(slashRange, slashArc, 8)) //Detect Potential Slash Targets
        {
            List<GameObject> targetEnemyList = new List<GameObject>();

            foreach (GameObject target in targetList)
            {
                HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>();

                if (enemyHealth.currentHealth <= 0) //Enemy should be in utility state
                {
                    targetEnemyList.Add(enemyHealth.gameObject);    //Target added to list
                    slicable = true;                                //Potential Target active
                    Physics.IgnoreLayerCollision(0, 8, true);       //Prevent player collision                    
                }
            }

            if (slicable)
            {
                transform.GetComponent<HealthScript>().invulnerable = true; //Become Invulnerable
                if (targetEnemyList.Count > 1)                      //If more than one potential target
                {
                    GameObject targetEnemy = new GameObject();
                    float dist = 100f;

                    foreach(GameObject target in targetEnemyList)
                    {
                        if(Vector3.Distance(transform.position, target.transform.position) < dist) //Filter to closest enemy position
                        {
                            targetEnemy = target;
                        }
                    }

                    RotateToTarget(targetEnemy.transform);          //Rotate to enemy
                    StartCoroutine(Dash(0f));                       //Activate Dashh Through
                    Debug.Log("Boop1");
                    //Set off utility state explosion
                    focus += 5;
                }
                else
                {
                    RotateToTarget(targetEnemyList[0].transform);   //Rotate to enemy
                    StartCoroutine(Dash(0f));                       //Activate Dash Through
                    Debug.Log("Boop2");
                    //Set off utility state explosion
                    focus += 5;
                }
            }
        }

        yield return new WaitForSeconds(resetTime);
        Physics.IgnoreLayerCollision(0, 8, false);
        transform.GetComponent<HealthScript>().invulnerable = false;
        isBusy = false;
    }

    IEnumerator KnockbackTarget(Vector3 kPosition, GameObject target) //Knockback ability
    {
        isBusy = true;

        float startTime = Time.time;

        Vector3 knockbackDirection = (target.transform.position - kPosition).normalized; //Knockback away from player position

        while (Time.time < startTime + dashTime) //Knock back for duration
        {
            target.GetComponent<CharacterController>().Move(knockbackDirection * dashSpeed * Time.deltaTime);

            yield return null;
        }

        yield return null;

        isBusy = false;
    }

    IEnumerator Bloom(float resetTime)
    {
        Debug.Log("bloom start");

        isBusy = true;
        
        bool bloomable = false;

        RotateToClickLocation();

        if (detectAttackable(slashRange, slashArc, 8)) //Detect Potential Slash Targets
        {
            List<GameObject> targetEnemyList = new List<GameObject>();

            foreach (GameObject target in targetList)
            {
                HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>();

                if (enemyHealth.currentHealth <= 0) //Enemy should be in utility state
                {
                    targetEnemyList.Add(enemyHealth.gameObject);    //Target added to list
                    bloomable = true;                    
                }
            }

            if (bloomable)
            {
                if (targetEnemyList.Count > 1)                      //If more than one potential target
                {
                    GameObject targetEnemy = new GameObject();
                    float dist = 100f;
                    Debug.Log("Cough 1");

                    foreach (GameObject target in targetEnemyList)
                    {
                        if (Vector3.Distance(transform.position, target.transform.position) < dist) //Filter to closest enemy position
                        {
                            targetEnemy = target;
                            targetEnemy.tag = "UtilityState";
                        }
                    }

                    //targetEnemy.GetComponent<EnemyAI>()

                    Collider[] colliders = Physics.OverlapSphere(targetEnemy.transform.position, attackRange, 1 << 8);
                    float startTime = Time.time;
                    //List<GameObject> bloomTargets = new List<GameObject>();

                    if (colliders.Length > 0)
                    {
                        foreach (Collider target in colliders)
                        {
                            Vector3 knockbackDirection = (target.transform.position - targetEnemy.transform.position).normalized; //Knockback away from initial Utility State Enemy position
                            float kDist = Vector3.Distance(target.transform.position, targetEnemy.transform.position);

                            if(target.tag == "Enemy")
                            {
                                target.GetComponent<HealthScript>().Damage(4);
                            }

                            StartCoroutine(KnockbackTarget(targetEnemyList[0].transform.position, target.gameObject));
                        }                        
                    }
                    yield return new WaitForSeconds(.5f);
                    targetEnemy.GetComponent<EnemyAI>().DestroyUtility();
                }
                else
                {
                    Debug.Log("Cough 2");

                    Collider[] colliders = Physics.OverlapSphere(targetEnemyList[0].transform.position, attackRange, 1 << 8);
                    float startTime = Time.time;
                    
                    if (colliders.Length > 0)
                    {
                        foreach (Collider target in colliders)
                        {
                            Vector3 knockbackDirection = (target.transform.position - targetEnemyList[0].transform.position).normalized; //Knockback away from initial Utility State Enemy position
                            float kDist = Vector3.Distance(target.transform.position, targetEnemyList[0].transform.position);

                            if (target.tag == "Enemy")
                            {
                                target.GetComponent<HealthScript>().Damage(4);
                            }
                            
                            StartCoroutine(KnockbackTarget(targetEnemyList[0].transform.position,target.gameObject));
                        }
                    }
                    yield return new WaitForSeconds(.5f);
                    targetEnemyList[0].GetComponent<EnemyAI>().DestroyUtility();
                }
            }
        }

        yield return new WaitForSeconds(resetTime + 5f);
        Debug.Log("bloom end");
        isBusy = false;
    }*/
}
