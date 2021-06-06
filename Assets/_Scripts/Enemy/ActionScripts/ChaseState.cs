using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName ="AI States/Chase")]
public class ChaseState : AIState
{
    public float attackRange;

    public AIState meleeAttackState;

    public AIState rangedAttackState;

    public float distance;
    public override void Movement(EnemyMovement ai)
    {
        ai.MoveToPlayer(GameController.gameController.toFollow);
        ai.UpdateDirection(GameController.gameController.toFollow);

        //rounding off the distance to a decimal with 2 places for more accuracy
        distance = Mathf.Round(Vector3.Distance(ai.transform.position, GameController.gameController.toFollow)*100) / 100;

        attackRange = ai.attackRange;
        Debug.Log(distance) ;
        if (distance <= attackRange)
        {
            ai.agent.velocity = Vector3.zero;

            if (ai.isRanged)
            {
                ai.currentState = rangedAttackState;
            }
            else
            {
                ai.currentState = meleeAttackState;
            }

        }
    }

}
