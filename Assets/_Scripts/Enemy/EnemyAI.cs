using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum SpawnerState { Chasing, MeleeAttack, RangedAttack };

    public bool isRanged;

    public EnemyMovement enemyMovement;

    public EnemyCombat enemyCombat;


    public float attackRange;

    //Distance between player/target position to the enemy
    float distance;

    //Shows the current action
    public SpawnerState currentAction;

    public float timeUntilPositionChange = 1.5f;

    public float timeUntilRangedAttack = 0;

    public bool isChangingPosition = false;



    // Start is called before the first frame update
    void Start()
    {
        enemyMovement = this.gameObject.GetComponent<EnemyMovement>();
        enemyCombat = this.gameObject.GetComponent<EnemyCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        AIStateController();
    }


    //Function that manages the AI states
    void AIStateController()
    {
        //rounding off the distance to a decimal with 2 places for more accuracy
        distance = Mathf.Round(Vector3.Distance(this.gameObject.transform.position, GameController.gameController.toFollow) * 100) / 100;

        //if player is enemy's attack range and melee type
        if (distance <= attackRange && !isRanged)
        {
            currentAction = SpawnerState.MeleeAttack;

            enemyMovement.UpdateDirection(GameController.gameController.toFollow);

            enemyMovement.agent.velocity = Vector3.zero;
            enemyCombat.MeleeAttack();
        }

        //if player is in enemy's attack range and ranged type
        else if (distance <= attackRange && isRanged)
        {
            enemyMovement.UpdateDirection(GameController.gameController.toFollow);

            //Changes enemy's position after each attack, the enemy gets a time limit in which it can move and if it's done moving it will commence the attack
            if (isChangingPosition && timeUntilRangedAttack >= 0)
            {
                currentAction = SpawnerState.Chasing;
              //  Debug.Log("Changing Position");
                timeUntilRangedAttack -= Time.deltaTime;
                enemyMovement.MoveToPlayer(GameController.gameController.toFollow, attackRange);
                return;
            }
            //If the time until attack variable is zero then we set the variables required to commence an attack and reset the last enemy explosion count
            else if (timeUntilRangedAttack <= 0)
            {
                currentAction = SpawnerState.RangedAttack;
               // Debug.Log("Done changing position");
                isChangingPosition = false;
                enemyCombat.doneAttacking = false;
                enemyCombat.currentCount = 0;
                timeUntilRangedAttack = timeUntilPositionChange;
            }
            //If enemy is done attacking we change it's state to chasing and provide it with a new offset where the enemy moves
            else if (enemyCombat.doneAttacking)
            {
                currentAction = SpawnerState.Chasing;

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
            currentAction = SpawnerState.Chasing;
            enemyCombat.currentCount = 0;
            enemyMovement.MoveToPlayer(GameController.gameController.toFollow, attackRange);
            enemyMovement.UpdateDirection(GameController.gameController.toFollow);
        }
    }
}
