using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State { Chasing, MeleeAttack, RangedAttack, Utility, Idle, StealingUtility, ChangingPosition, Roaming };

    public enum Type { Ranged, Melee, FlyBoy, Utility, BigChungus, UtilityThief };

    public Type enemyType;

    public EnemyMovement enemyMovement;

    public EnemyCombat enemyCombat;

    public HealthScript enemyHealthSystem;

    public float attackRange;

    public bool isActive=true;

    public bool canBeKnocked = false;

    //Distance between player/target position to the enemy
    float distance;

    //Shows the current action
    public State currentAction;


    private void Awake()
    {
        enemyMovement = this.gameObject.GetComponent<EnemyMovement>();
        enemyCombat = this.gameObject.GetComponent<EnemyCombat>();
        enemyHealthSystem = this.gameObject.GetComponent<HealthScript>();
    }

    private void Start()
    {
        
        enemyMovement.InitialiseMovement();
        enemyHealthSystem.healthBarCanvas = this.transform.Find("Canvas").gameObject;
        EnemySpawner.instance.enemiesInTheScene.Add(this);

        if (enemyType == Type.Utility)
        {
            enemyMovement.agent.enabled = false;
            enemyMovement.obstacle.enabled = true;
            enemyMovement.enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        ManageHealthSystem();
        AIStateController();
    }


    //Function that manages the AI states
    void AIStateController()
    {
        //The enemies won't do anything if the player is dashing as if they are stunned
        if (GameController.isPlayerDashing)
            return;
        if (!isActive)
            return;

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
            case Type.BigChungus:
                BigChungusActions();
                break;
            case Type.UtilityThief:
                UtilityThiefActions();
                break;
                /*
            case Type.FlyBoy:
                FlyBoyActions();
                break;
                */
        }
        return;
    }

    void UtilityActions()
    {
        //I do nothing as of now
        return;
    }
    void UtilityThiefActions()
    {
        if (EnemySpawner.instance.UtilityStatesInTheScene.Count == 0)
        {
            return;
        }
        enemyCombat.Attack();

    }
    void RangedActions()
    {
        //rounding off the distance to a decimal with 2 places for more accuracy
        distance = GetPreciseDistance(this.gameObject.transform.position, GameController.instance.toFollow);
        if (distance <= attackRange)
        {
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
            enemyMovement.MoveToTarget(GameController.instance.toFollow, attackRange);
            if (enemyMovement.IsAtTarget(enemyMovement.agent.destination))
            {
                //Stop moving, and commence attack
                enemyCombat.doneAttacking = false;
                enemyMovement.GetRandomAngle(enemyMovement.targetOffset);

            }
            if (!enemyCombat.doneAttacking)
            {
                currentAction = State.RangedAttack;
                enemyMovement.agent.velocity = Vector3.zero;

                if (!enemyCombat.isAttacking)
                    enemyCombat.Attack();
                
            }

        }
        else
        {
            currentAction = State.Chasing;
            // enemyCombat.currentCount = 0;
            enemyMovement.MoveToTarget(GameController.instance.toFollow, attackRange);
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
        }
    }

    void MeleeActions()
    {
        distance = GetPreciseDistance(this.gameObject.transform.position, GameController.instance.toFollow);
        if (distance <= attackRange)
        {
            currentAction = State.MeleeAttack;
            
            //enemyMovement.agent.velocity = Vector3.zero;
            //Either go to the assigned spot and attack or attack if the player is in range
            if (enemyMovement.IsAtTarget(enemyMovement.agent.destination)|| GetPreciseDistance(this.gameObject.transform.position, GameController.instance.toFollow)<attackRange)
            {
                enemyMovement.agent.velocity = Vector3.zero;
                if (!enemyCombat.doneAttacking)
                {
                    
                    enemyCombat.doneAttacking = true;
                    enemyCombat.isBusy = true;
                    enemyMovement.agent.destination = this.gameObject.transform.position;
                    enemyCombat.Attack();
                    enemyMovement.agent.radius = 0.01f;
                    //StartCoroutine(enemyCombat.LeapAtTarget(enemyMovement.eController, 20f, 0.2f, GameController.instance.Player));                

                }
            }

            enemyMovement.UpdateDirection(GameController.instance.toFollow);
        }
        else if(!enemyCombat.isBusy)
        {
            enemyMovement.agent.radius = enemyMovement.agentRadius;
            enemyMovement.agent.enabled = true;
            currentAction = State.Chasing;
            enemyMovement.MoveToTarget(GameController.instance.toFollow, attackRange);
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
        }
    }
 


    void BigChungusActions()
    {
        //if player is stunned
        if (enemyCombat.isBusy)
        {
            currentAction = State.Idle;
            return;
        }
           
        if (GameController.instance.Player.gameObject.GetComponent<HealthScript>().CheckIfVulnerable())
        {
            distance = GetPreciseDistance(this.gameObject.transform.position, GameController.instance.toFollow);
            enemyMovement.targetPosition = Vector3.zero;
            if (distance <= attackRange && !enemyCombat.isBusy)
            {
                currentAction = State.MeleeAttack;
                enemyMovement.UpdateDirection(GameController.instance.toFollow);
                enemyMovement.agent.velocity = Vector3.zero;
                if (!enemyCombat.isAttacking)
                {
                    enemyCombat.isAttacking = true;
                    enemyMovement.agent.enabled = false;
                    //enemyMovement.agent.destination = this.transform.position;
                    enemyCombat.Attack();
                    enemyMovement.agent.radius = 0.01f;
                }


            }
            else
            {
                
                currentAction = State.Chasing;
                enemyMovement.agent.enabled = true;
                enemyMovement.MoveToTarget(GameController.instance.toFollow, attackRange);
                enemyMovement.UpdateDirection(GameController.instance.toFollow);
                enemyMovement.agent.radius = enemyMovement.agentRadius;
            }
        }
        else
        {
            //Get random point on screen to follow and roam
            currentAction = State.Roaming;
            enemyMovement.agent.enabled = true;
            if (enemyMovement.targetPosition == Vector3.zero)
            {
                enemyMovement.GetRandomPoint(this.gameObject.transform.position);
            }


            distance = GetPreciseDistance(this.gameObject.transform.position, enemyMovement.targetPosition);
            if (distance < 1.5f)
            {
                enemyMovement.GetRandomPoint(this.gameObject.transform.position);
            }
            else
            {
                enemyMovement.agent.radius = enemyMovement.agentRadius;
                enemyMovement.MoveToTarget(enemyMovement.targetPosition, 0f);
                enemyMovement.UpdateDirection(enemyMovement.targetPosition);
            }
            
        }

        
    }

    void ChangeToUtility()
    {

        enemyCombat.StopAllCoroutines();
        enemyCombat.enabled = false;

        enemyMovement.agent.destination = this.gameObject.transform.position;
        enemyMovement.agent.enabled = false;

        enemyMovement.enabled = false;

        enemyMovement.obstacle.enabled = true;
     
        //enemyHealthSystem.currentHealth = 100;
        EnemySpawner.instance.enemiesInTheScene.Remove(this);
        EnemySpawner.instance.UtilityStatesInTheScene.Add(this);

        currentAction = State.Utility;
        enemyHealthSystem.healthBarCanvas.SetActive(false);
        //Debug.Log(EnemySpawner.enemySpawner.UtilityStatesInTheScene[0]);
    }
    public void DestroyUtility()
    {
        EnemySpawner.instance.UtilityStatesInTheScene.Remove(this);
        Destroy(this.gameObject);
    }

    void ManageHealthSystem()
    {
        enemyHealthSystem.healthBarCanvas.transform.rotation = Camera.main.transform.rotation;
        enemyHealthSystem.healthSlider.targetGraphic.color = Color.Lerp(Color.red, Color.green, enemyHealthSystem.currentHealth / 100f);

        if (enemyHealthSystem.currentHealth <= 0 && enemyType != Type.Utility)
        {
            if (enemyType == Type.UtilityThief)
            {
                
                while (enemyCombat.soulValue > 0)
                {
                    EnemySpawner.instance.SpawnEnemy(EnemySpawner.instance.meleeEnemy);
                    enemyCombat.soulValue--;
                }
                isActive = false;
                //StartCoroutine(DisableTemporarily());
                
            }
            else
            {
                enemyType = Type.Utility;
                ChangeToUtility();
                return;
            }
        }
    }

    IEnumerator DisableTemporarily()
    {
        isActive = false;
        yield return new WaitForSeconds(enemyCombat.strikeDelay);
        enemyHealthSystem.currentHealth = enemyHealthSystem.maxHealth;
        enemyCombat.soulValue = 0;
        isActive = true;

    }

    public static float GetPreciseDistance(Vector3 _a, Vector3 _b)
    {
        _a.y = 0;
        _b.y = 0;
        return Mathf.Round(Vector3.Distance(_a, _b) * 100) / 100;
    }
}


/*
     void RangedActionsOld()
    {
        //rounding off the distance to a decimal with 2 places for more accuracy
        distance = GetPreciseDistance(this.gameObject.transform.position, GameController.instance.toFollow);
        if (distance <= attackRange)
        {
            enemyMovement.UpdateDirection(GameController.instance.toFollow);

            //If enemy is changing position and done attacking
            if (enemyMovement.isChangingPosition && enemyCombat.doneAttacking)
            {
                //In order to grab new target according to the new offset
                enemyMovement.MoveToTarget(GameController.instance.toFollow, attackRange);

                //check if player is at target place
                if (enemyMovement.IsAtTarget(enemyMovement.agent.destination))
                {
                    //Stop moving, and commence attack
                    enemyMovement.isChangingPosition = false;
                    enemyCombat.doneAttacking = false;
                    enemyMovement.GetRandomAngle(enemyMovement.targetOffset);
                    return;
                }
                currentAction = State.ChangingPosition;
            }
            else if (!enemyCombat.doneAttacking)
            {
                currentAction = State.RangedAttack;
                enemyMovement.agent.velocity = Vector3.zero;

                if (!enemyCombat.isAttacking)
                    StartCoroutine(enemyCombat.ShootProjectile());

                enemyMovement.isChangingPosition = true;

            }
        }
        else
        {
            currentAction = State.Chasing;
            // enemyCombat.currentCount = 0;
            enemyMovement.MoveToTarget(GameController.instance.toFollow, attackRange);
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
        }
    }
    void UtilityThiefActionsOld()
    {
        if (EnemySpawner.instance.UtilityStatesInTheScene.Count == 0)
        {
            return;
        }
        enemyCombat.Attack();
        /*
        else
        {

            distance = GetPreciseDistance(this.gameObject.transform.position, enemyMovement.enemyTarget.transform.position);

            if (distance <= attackRange)
            {
                currentAction = State.StealingUtility;
                enemyMovement.UpdateDirection(enemyMovement.enemyTarget.transform.position);

                if (!enemyCombat.isAttacking)
                {
                    StartCoroutine(enemyCombat.StealUtility(enemyMovement.enemyTarget.GetComponent<EnemyAI>(), attackRange));
                    //enemyCombat.StealUtility(enemyMovement.enemyTarget.GetComponent<EnemyAI>());
                }

            }
            else
            {
                currentAction = State.Chasing;
                enemyMovement.MoveToTarget(enemyMovement.enemyTarget.transform.position, attackRange);
                enemyMovement.UpdateDirection(enemyMovement.enemyTarget.transform.position);
            }
        
        }
        
    }
    void UtilityThiefActions()
    {
        if (EnemySpawner.instance.UtilityStatesInTheScene.Count == 0)
        {
            return;
        }
        if (enemyMovement.enemyTarget == null)
        {
            enemyMovement.GetUtilityTarget();
            // enemyCombat.doneAttacking = false;
        }
        else
        {

            distance = GetPreciseDistance(this.gameObject.transform.position, enemyMovement.enemyTarget.transform.position);

            if (distance <= attackRange)
            {
                currentAction = State.StealingUtility;
                enemyMovement.UpdateDirection(enemyMovement.enemyTarget.transform.position);

                if (!enemyCombat.isAttacking)
                {
                    StartCoroutine(enemyCombat.StealUtility(enemyMovement.enemyTarget.GetComponent<EnemyAI>(), attackRange));
                    //enemyCombat.StealUtility(enemyMovement.enemyTarget.GetComponent<EnemyAI>());
                }

            }
            else
            {
                currentAction = State.Chasing;
                enemyMovement.MoveToTarget(enemyMovement.enemyTarget.transform.position, attackRange);
                enemyMovement.UpdateDirection(enemyMovement.enemyTarget.transform.position);
            }

        }
    }
   void FlyBoyActions()
   {
       //ranged enemy behaviour when there are no utility states
       if (EnemySpawner.instance.UtilityStatesInTheScene.Count == 0)
       {
           enemyMovement.enemyTarget = GameController.instance.Player.gameObject;
           distance = GetPreciseDistance(this.gameObject.transform.position, GameController.instance.toFollow);

           //reset time required to hit
           if (distance <= attackRange * 7)
           {
               enemyMovement.UpdateDirection(GameController.instance.toFollow);

               //If enemy is changing position and done attacking
               if (enemyMovement.isChangingPosition && enemyCombat.doneAttacking)
               {
                   //In order to grab new target according to the new offset  
                   currentAction = State.ChangingPosition;
                   enemyCombat.timeUntilHit = enemyCombat.strikeDelay;
                   enemyMovement.FlyToTarget(attackRange * 7);
                   //check if player is at target place                 
                   if (enemyMovement.IsAtTarget(enemyMovement.targetPosition))
                   {
                       enemyMovement.isChangingPosition = false;
                       enemyCombat.doneAttacking = false;
                       //Stop moving, and commence attack
                       enemyMovement.HoverInPlace(enemyMovement.floatOffset / 2);
                       return;
                   }

               }
               else if (!enemyCombat.doneAttacking)
               {
                   currentAction = State.RangedAttack;
                   enemyCombat.ShootProjectile();
                   enemyMovement.GetRandomAngle(enemyMovement.targetOffset);
                   enemyMovement.HoverInPlace(enemyMovement.floatOffset / 2);
                   enemyMovement.isChangingPosition = true;
               }
           }
           else
           {
               currentAction = State.Chasing;
               // enemyCombat.currentCount = 0;
               enemyMovement.FlyToTarget(attackRange * 7);
               enemyMovement.UpdateDirection(GameController.instance.toFollow);
           }

       }//if there are utility states
       else if (EnemySpawner.instance.UtilityStatesInTheScene.Count > 0)
       {
           if (enemyMovement.enemyTarget == GameController.instance.Player.gameObject)
           {
               enemyMovement.enemyTarget = null;
           }
           if (enemyMovement.enemyTarget == null)
           {
               enemyMovement.GetUtilityTarget();
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
                       enemyCombat.StealUtility(enemyMovement.enemyTarget.GetComponent<EnemyAI>());
                   }

               }
               else
               {
                   //fly to the utility
                   currentAction = State.Chasing;
                   enemyMovement.FlyToTarget(attackRange);
               }

           }

       }
       return;
   }

    void MeleeActionsOld()
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
            enemyMovement.MoveToTarget(GameController.instance.toFollow, attackRange);
            enemyMovement.UpdateDirection(GameController.instance.toFollow);
        }
    }
   */
