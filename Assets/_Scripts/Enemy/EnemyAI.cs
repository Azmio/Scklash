using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State { Chasing, MeleeAttack, RangedAttack, Utility, Idle, StealingUtility };

    public enum Type { Ranged, Melee, FlyBoy, Utility};

    public Type enemyType;

    public EnemyMovement enemyMovement;

    public EnemyCombat enemyCombat;

    public HealthScript enemyHealthSystem;

    public float attackRange;

    //Distance between player/target position to the enemy
    float distance;

    //Shows the current action
    public State currentAction;

    public float timeUntilPositionChange = 1.5f;

    public float timeUntilRangedAttack = 0;

    public bool isChangingPosition = false;
    
    // Start is called before the first frame update

    private void Awake()
    {
        enemyMovement = this.gameObject.GetComponent<EnemyMovement>();
        enemyCombat = this.gameObject.GetComponent<EnemyCombat>();
        enemyHealthSystem = this.gameObject.GetComponent<HealthScript>();
    }

    private void Start()
    {
        InitializeIfEnemy();
        enemyMovement.InitialiseMovement();

    }

    // Update is called once per frame
    void Update()
    {
        ManageHealthSystem();
        AIStateController();
    }
    void InitializeIfEnemy()
    {
        enemyHealthSystem.healthBarCanvas = this.transform.Find("Canvas").gameObject;
        EnemySpawner.enemySpawner.enemiesInTheScene.Add(this);
    }
 
    //Function that manages the AI states

    void AIStateController()
    {
        switch (enemyType)
        {
            case Type.Utility:
                UtilityActions();
                break;
            case Type.Melee:
                MeleeActions();
                break;
            case Type.Ranged:
                RangedActions();
                break;
            case Type.FlyBoy:
                FlyBoyActions();
                break;
        }
        return;
    }


    void ManageHealthSystem()
    {
        enemyHealthSystem.healthBarCanvas.transform.rotation = Camera.main.transform.rotation;
        enemyHealthSystem.healthSlider.targetGraphic.color = Color.Lerp(Color.red, Color.green, enemyHealthSystem.currentHealth / 100f);

        if (enemyHealthSystem.currentHealth <= 0 && enemyType != Type.Utility)
        {
            if (enemyType == Type.FlyBoy)
            {
                if (enemyCombat.utilitiesAbsorbed <= 0)
                {
                    enemyType = Type.Utility;
                    ChangeToUtility();
                    return;
                }
                else
                {
                    while (enemyCombat.utilitiesAbsorbed > 0)
                    {
                        EnemySpawner.enemySpawner.SpawnEnemy(EnemySpawner.enemySpawner.meleeEnemy);
                        enemyCombat.utilitiesAbsorbed--;
                    }
                }
                Destroy(this.gameObject);
            }
            else
            {
                enemyType = Type.Utility;
                ChangeToUtility();
                return;
            }
        }
    }

    public void DestroyUtility()
    {
        EnemySpawner.enemySpawner.UtilityStatesInTheScene.Remove(this);
        Destroy(this.gameObject);
    }

    void FlyBoyActions()
    {
        if (EnemySpawner.enemySpawner.UtilityStatesInTheScene.Count == 0)
        {
            currentAction = State.Idle;
            enemyMovement.HoverInPlace(enemyMovement.floatOffset);
            return;
        }
        
        if (enemyMovement.enemyTarget == null)
        {
            enemyMovement.GetTarget();
            enemyCombat.doneAttacking = false;
        }
        else
        {
            distance = GetPreciseDistance(this.gameObject.transform.position, enemyMovement.enemyTarget.transform.position);

            if (distance <= attackRange)
            {
                currentAction = State.StealingUtility;
                Debug.Log("stealing life");
                enemyMovement.HoverInPlace(enemyMovement.floatOffset / 2);
                enemyMovement.UpdateDirection(enemyMovement.enemyTarget.transform.position);

                if (!enemyCombat.doneAttacking)
                {
                    enemyCombat.StealUtility(enemyMovement.enemyTarget);
                }

            }
            else
            {
                //fly to the utility
                currentAction = State.Chasing;
                enemyMovement.FlyToTarget(attackRange);
            }
        }

        //get random utility target or wait by floating up and down xD
        //fly to random utility target
        return;
    }
    void UtilityActions()
    {
        //I do nothing as of now
        return;
    }

    void MeleeActions()
    {
        //rounding off the distance to a decimal with 2 places for more accuracy
        distance = GetPreciseDistance(this.gameObject.transform.position, GameController.instance.toFollow);
        if (distance <= attackRange)
        {
            currentAction = State.MeleeAttack;
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
            enemyMovement.agent.velocity = Vector3.zero;
            enemyCombat.MeleeAttack();
        }
        else
        {
            currentAction = State.Chasing;
            enemyMovement.MoveToPlayer(GameController.instance.toFollow, attackRange);
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
        }
    }
    void RangedActions()
    {
        //rounding off the distance to a decimal with 2 places for more accuracy
        distance = GetPreciseDistance(this.gameObject.transform.position, GameController.instance.toFollow);
        if (distance <= attackRange)
        {
            enemyMovement.UpdateDirection(GameController.instance.toFollow);

            //Changes enemy's position after each attack, the enemy gets a time limit in which it can move and if it's done moving it will commence the attack
            if (isChangingPosition && timeUntilRangedAttack >= 0)
            {
                currentAction = State.Chasing;
                //  Debug.Log("Changing Position");
                timeUntilRangedAttack -= Time.deltaTime;
                enemyMovement.MoveToPlayer(GameController.instance.toFollow, attackRange);
                return;
            }
            //If the time until attack variable is zero then we set the variables required to commence an attack and reset the last enemy explosion count
            else if (timeUntilRangedAttack <= 0)
            {
                currentAction = State.RangedAttack;
                // Debug.Log("Done changing position");
                isChangingPosition = false;
                enemyCombat.doneAttacking = false;
                enemyCombat.currentCount = 0;
                timeUntilRangedAttack = timeUntilPositionChange;
            }
            //If enemy is done attacking we change it's state to chasing and provide it with a new offset where the enemy moves
            else if (enemyCombat.doneAttacking)
            {
                currentAction = State.Chasing;

                enemyMovement.GetRandomAngle(enemyMovement.targetOffset);

                //this variable is set true and checked by the initial if statement 
                isChangingPosition = true;
            }

            //If the enemy isn't done attacking we start with the ranged attack after setting the enemy's velocity to zero so that it wont move while attacking
            else
            {
                //  Debug.Log("Attacking");
                enemyCombat.RangedAttack();
                enemyMovement.agent.velocity = Vector3.zero;
            }
        }
        else
        {
            currentAction = State.Chasing;
            enemyCombat.currentCount = 0;
            enemyMovement.MoveToPlayer(GameController.instance.toFollow, attackRange);
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
        }
    }
    void ChangeToUtility()
    {
        enemyMovement.agent.destination = this.gameObject.transform.position;
        enemyMovement.enabled = false;
        enemyCombat.enabled = false;
        EnemySpawner.enemySpawner.enemiesInTheScene.Remove(this);
        EnemySpawner.enemySpawner.UtilityStatesInTheScene.Add(this);
        currentAction = State.Utility;
        enemyHealthSystem.healthBarCanvas.SetActive(false);
        //Debug.Log(EnemySpawner.enemySpawner.UtilityStatesInTheScene[0]);
    }
    public static float GetPreciseDistance(Vector3 _a, Vector3 _b)
    {
        _a.y = 0;
        _b.y = 0;
        return Mathf.Round(Vector3.Distance(_a, _b) * 100) / 100;
    }
    //Obsolete stuff

    /*
        //rounding off the distance to a decimal with 2 places for more accuracy
        distance = GetPreciseDistance(this.gameObject.transform.position, GameController.instance.toFollow); 

        //if player is enemy's attack range and melee type
        if (distance <= attackRange && !isRanged)
        {
            currentAction = State.MeleeAttack;
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
            enemyMovement.agent.velocity = Vector3.zero;
            enemyCombat.MeleeAttack();
        }
        //if player is in enemy's attack range and ranged type
        else if (distance <= attackRange && isRanged)
        {
            enemyMovement.UpdateDirection(GameController.instance.toFollow);

            //Changes enemy's position after each attack, the enemy gets a time limit in which it can move and if it's done moving it will commence the attack
            if (isChangingPosition && timeUntilRangedAttack >= 0)
            {
                currentAction = State.Chasing;
              //  Debug.Log("Changing Position");
                timeUntilRangedAttack -= Time.deltaTime;
                enemyMovement.MoveToPlayer(GameController.instance.toFollow, attackRange);
                return;
            }
            //If the time until attack variable is zero then we set the variables required to commence an attack and reset the last enemy explosion count
            else if (timeUntilRangedAttack <= 0)
            {
                currentAction = State.RangedAttack;
               // Debug.Log("Done changing position");
                isChangingPosition = false;
                enemyCombat.doneAttacking = false;
                enemyCombat.currentCount = 0;
                timeUntilRangedAttack = timeUntilPositionChange;
            }
            //If enemy is done attacking we change it's state to chasing and provide it with a new offset where the enemy moves
            else if (enemyCombat.doneAttacking)
            {
                currentAction = State.Chasing;

                enemyMovement.GetRandomAngle(enemyMovement.targetOffset);

                //this variable is set true and checked by the initial if statement 
                isChangingPosition = true;
            }

            //If the enemy isn't done attacking we start with the ranged attack after setting the enemy's velocity to zero so that it wont move while attacking
            else
            {
              //  Debug.Log("Attacking");
                enemyCombat.RangedAttack();
                enemyMovement.agent.velocity = Vector3.zero;
            }
        }
        //if enemy is not in attack range
        else
        {
            currentAction = State.Chasing;
            enemyCombat.currentCount = 0;
            enemyMovement.MoveToPlayer(GameController.instance.toFollow, attackRange);
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
        }
     */
}
