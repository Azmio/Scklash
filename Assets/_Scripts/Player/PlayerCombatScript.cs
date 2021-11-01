using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatScript : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    //Basic Attack
    [Header("Basic Attack Values")]
    public float attackRange = 5f;
    [Range(0, 360)] public float attackArc = 150f;
    public float attackResetTime = 1f;
   
    //[HideInInspector]
    public List<GameObject> targetList;
    public int attackDamage = 10;
    public int attackDamageMultiplier = 1;

    public bool isPressed = false;
    public bool isHolding = false;
    bool actionCompleted = true;
    public float timer = 0.3f;

    //Attack Combo
    private float lastAttackTime;
    public int comboIndex; //Current Combo Count
    public float maxComboDelay = 0.9f;
    public bool isAttacking;

    //Knockback Values
    public float KnockbackDuration = 0.3f;
    public float knockbackSpeed = 20f;   

    //Slash Values
    [Header("Slash Attack")]
    public float slashRange = 5f;
    [Range(0, 360)] public float slashArc = 50f;

    //Bloom Values
    public GameObject bloomProjectilePrefab;
    public GameObject bloomSource;
    public float projectileForce;

    void Start()
    {
        comboIndex = 0;
        isAttacking = false;
    }

    
    public void CombatUpdate()
    {
        if (playerController.inputHandler.GetKeyDown(PlayerActions.Attack) && !playerController.isBusy) //If Attack Button is down
        {
            isPressed = true;                                           
            playerController.movementScript.canMove = false;
            playerController.movementScript.RotateToClickLocation();
        }

        if (playerController.inputHandler.GetKeyUp(PlayerActions.Attack) && !playerController.isBusy) //If Attack Button is active
        {
            if (!isHolding) //Check if holding hasn't been activated
            {
                StartCoroutine(BasicAttack(attackResetTime, comboIndex));
            }                     

            playerController.movementScript.canMove = true;
            isPressed = false;
            isHolding = false;
            actionCompleted = false;
            timer = 0.3f;
        }        

        if (isPressed && !actionCompleted)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)            
                isHolding = true;                
            
            if (isHolding)
            {
                playerController.anim.Play("Attack1");

                if (detectAttackable(attackRange, attackArc, 8))
                {
                    foreach (GameObject enemy in targetList)
                    {
                        StartCoroutine(KnockbackTarget(transform.position, enemy));
                    }                   
                }

                timer = 0.3f;
                actionCompleted = true;
            }
        }



        if (playerController.inputHandler.GetKeyDown(PlayerActions.Slash) && !playerController.isBusy) //Slash Target - Default : RM Button
        {
            StartCoroutine(Slash(attackResetTime));
        }

        if (playerController.inputHandler.GetKeyDown(PlayerActions.Dash) && !playerController.isBusy) //Slash Target - Default : RM Button
        {
            StartCoroutine(playerController.movementScript.Dash(0f));
        }

        if (playerController.inputHandler.GetKeyDown(PlayerActions.Bloom) && !playerController.isBusy) //Slash Target - Default : RM Button
        {
            StartCoroutine(Bloom(0f));
        }
    }
    
    Vector3 TargetFacing(Vector3 targetDirection, Vector3 origin)
    {
        return (targetDirection - origin).normalized;
    }

    private bool detectAttackable(float range, float Arc, int layer) //Detect any attackable objects within arc settings
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, 1 << layer); //create sphere around player with radius of 3
        targetList = new List<GameObject>();

        if (colliders.Length > 0)
        {
            foreach (Collider target in colliders)
            {                
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

    

    IEnumerator BasicAttack(float resetTime, int currentComboIndex) //Attack Ability
    {
        playerController.isBusy = true;
        playerController.movementScript.canMove = false;
        playerController.movementScript.RotateToClickLocation();

        if (detectAttackable(attackRange, attackArc, 8))
        {
            lastAttackTime = Time.time;

            switch (currentComboIndex)
            {
                case 0: //Initial Attack
                    comboIndex++;
                    //Debug.Log("Combo 1");  
                    playerController.anim.Play("Attack1");
                    //sound 1
                    break;

                case 1: //Second Attack
                    comboIndex++;
                    //Debug.Log("Combo 2");
                    playerController.anim.Play("Attack1");
                    //sound 2
                    break;

                case 2: //Third Attack
                    comboIndex++;
                    //Debug.Log("Combo 3");

                    //attackDamageMultiplier += 2;
                    playerController.anim.Play("Attack1");
                    //sound3
                    break;
            }

            foreach (GameObject target in targetList)
            {
                HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>();

                if (enemyHealth.GetHealth() > 0)
                {
                    /**if (Vector3.Dot(TargetFacing(enemyHealth.transform.forward, transform.position), transform.forward) < 0.05f) //Sneak modifier added
                    {
                        attackDamageMultiplier += 2;
                        Debug.Log("Sneak Attack Multiplier Added");
                    }*/

                    enemyHealth.Damage(attackDamage);

                    /**if (currentComboIndex == 3)
                    {
                        StartCoroutine(KnockbackTarget(transform.position, target));
                        comboIndex = 0; //Reset Combo Indexer
                    }*/
                }
            }
        }
        else
        {
            playerController.anim.Play("Attack1");
            comboIndex = 0;
        }

        if (comboIndex == 3)
            comboIndex = 0;

        playerController.isBusy = false;
        playerController.movementScript.canMove = true;        
        yield return new WaitForSeconds(resetTime);
    }

    IEnumerator KnockbackTarget(Vector3 kPosition, GameObject target) //Knockback ability
    {
        playerController.isBusy = true;
        playerController.movementScript.canMove = false;
        
        EnemyAI targetAI = target.GetComponent<EnemyAI>();
        
        if (targetAI != null)
        {            
            if (targetAI.canBeKnocked)
            {
                float startTime = Time.time;

                Vector3 knockbackDirection = (target.transform.position - kPosition).normalized; //Knockback away from player position
                knockbackDirection.y = 0;              

                while (Time.time < startTime + KnockbackDuration) //Knock back for duration
                {
                    target.GetComponent<CharacterController>().Move(knockbackDirection * knockbackSpeed * Time.deltaTime);

                    yield return null;
                }
            }
        }             

        playerController.isBusy = false;
        playerController.movementScript.canMove = true;
        yield return null;
    }

    IEnumerator Slash(float resetTime)
    {
        playerController.isBusy = true;
        playerController.movementScript.canMove = false;

        bool slicable = false;
        
        if (detectAttackable(slashRange, slashArc, 8)) //Detect Potential Slash Targets
        {
            List<GameObject> targetEnemyList = new List<GameObject>();

            foreach (GameObject target in targetList)
            {
                HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>();

                if (enemyHealth.currentHealth <= 0) //Enemy should be in utility state
                {
                    targetEnemyList.Add(enemyHealth.gameObject);    //Target added to list
                    //slicable = true;                                //Potential Target active
                    Physics.IgnoreLayerCollision(0, 8, true);       //Prevent player collision                    
                }
            }

            if (slicable)//(slicable)
            {
                playerController.isInvulnerable = true; //Become Invulnerable

                if (targetEnemyList.Count > 1)                      //If more than one potential target
                {
                    GameObject targetEnemy = new GameObject();
                    float dist = 100f;

                    foreach (GameObject target in targetEnemyList)
                    {
                        if (Vector3.Distance(transform.position, target.transform.position) < dist) //Filter to closest enemy position
                        {
                            targetEnemy = target;
                        }
                    }

                    playerController.movementScript.RotateToTarget(targetEnemy.transform);          //Rotate to enemy
                    StartCoroutine(playerController.movementScript.Dash(0f));                       //Activate Dashh Through
                    //Set off utility state explosion
                    playerController.movementScript.utilityKillCount++;
                }
                else
                {
                    playerController.movementScript.RotateToTarget(targetEnemyList[0].transform);   //Rotate to enemy
                    StartCoroutine(playerController.movementScript.Dash(0f));                       //Activate Dash Through
                    //Set off utility state explosion
                    playerController.movementScript.utilityKillCount++;
                }
            }
        }

        Physics.IgnoreLayerCollision(0, 8, false);
        transform.GetComponent<HealthScript>().invulnerable = false;
        playerController.isBusy = false;
        playerController.movementScript.canMove = true;
        yield return new WaitForSeconds(resetTime);
    }   

    IEnumerator Bloom(float resetTime)
    {
        playerController.isBusy = true;
        playerController.movementScript.canMove = false;
        playerController.movementScript.RotateToClickLocation();

        //Create object and set Additional Values
        GameObject projectile = Instantiate(bloomProjectilePrefab, bloomSource.transform.position, transform.rotation);
        Rigidbody projectileRigidBody = projectile.GetComponent<Rigidbody>();
        projectileRigidBody.AddForce(transform.forward * projectileForce);

        //Destroy(projectile, 8);

        playerController.isBusy = false;
        playerController.movementScript.canMove = true;
        yield return new WaitForSeconds(resetTime);
    }

    /***IEnumerator Bloom(float resetTime)
    {
        playerController.isBusy = true;
        playerController.movementScript.canMove = false;
        playerController.movementScript.RotateToClickLocation();

        bool canBloom = false;
        List<GameObject> targetEnemyList = new List<GameObject>();

        if (detectAttackable(slashRange, slashArc, 8)) //Detect Potential Slash Targets
        {
            foreach (GameObject target in targetList)
            {
                HealthScript enemyHealth = (HealthScript)target.GetComponent<HealthScript>();

                if(enemyHealth.currentHealth <= 0) //Enemy should be in utility state
                {
                    targetEnemyList.Add(enemyHealth.gameObject);    //Target added to list
                    canBloom = true;
                }
            }
        }

        if(canBloom)
        {
            GameObject targetEnemy = new GameObject();
            float dist = 100f;

            foreach(GameObject target in targetEnemyList)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < dist) //Filter to closest enemy position
                {
                    targetEnemy = target;
                }
            }

            //target.GetComponent<UtilityStateScript>().BoomBoom();
        }

        playerController.isBusy = false;
        playerController.movementScript.canMove = true;
        yield return new WaitForSeconds(resetTime);
    }*/
    
}
