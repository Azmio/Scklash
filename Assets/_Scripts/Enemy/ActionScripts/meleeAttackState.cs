using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/Melee")]
public class meleeAttackState : AIState
{
    public AIState chaseState;
    public float attackRange;
    public float distance;
    public override void Movement(EnemyMovement ai)
    {
        attackRange = ai.attackRange;
        ai.UpdateDirection(GameController.gameController.toFollow);

        //rounding off the distance to a decimal with 2 places for more accuracy
        distance = Mathf.Round(Vector3.Distance(ai.transform.position, GameController.gameController.toFollow) * 100) / 100;

        if (distance > attackRange)
        {
            ai.currentState = chaseState;
        }
        else
        {
            Debug.Log("Melee attack on player");
        }
    }
}
