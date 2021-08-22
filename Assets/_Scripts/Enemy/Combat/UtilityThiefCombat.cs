using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UtilityThiefCombat : EnemyCombat
{
    public GameObject attackPrefab;

    public float timeRequiredToSuck;
    

    //strike delay is going to be the reset time, that the game will wait for before re-enabling the utility thief


    public GameObject enemyTarget;
    float minDistance = float.MaxValue;

    public override void Attack()
    {
        //Get a target if you dont have one
        if (enemyTarget == null)
        {
            GetUtilityTarget();
        }
        //attack if you have a target
        else
        {
            //if not attacking then spawn a wire
            if (!isAttacking)
            {
                StartCoroutine(SpawnWire());
            }
        }
    }
    void GetUtilityTarget()
    {
        minDistance = float.MaxValue;
        foreach (EnemyAI target in EnemySpawner.instance.UtilityStatesInTheScene)
        {
            //target isnt active if it's being attacked by a wire already thus only a single wire would spawn at a single target
            if (EnemyAI.GetPreciseDistance(this.gameObject.transform.position, target.transform.position) < minDistance && target.isActive)
            {
                minDistance = EnemyAI.GetPreciseDistance(this.gameObject.transform.position, target.transform.position);
                enemyTarget = target.gameObject;
                target.isActive = false;
            }
        }
    }

    public IEnumerator SpawnWire()
    {
        //change isAttacking to true so that the coroutine won't be called again
        isAttacking = true;

        //Spawn the wire at a random position that is within 2.5 units of the utility state
        Vector3 spawnTemp = GetWireSpawn(enemyTarget.transform.position);
        Vector3 lookDir = (enemyTarget.transform.position - spawnTemp).normalized;
        lookDir.y = 0;
        Wire temp = Instantiate(attackPrefab, spawnTemp, Quaternion.LookRotation(lookDir, Vector3.up)).GetComponent<Wire>();
        //Switch the wire's target to utility state
        temp.target = enemyTarget;
        temp.timeRequiredToAbsorb = timeRequiredToSuck;

        //wait for the wire to grow, absorb and shrink back into the ground
        while (temp.isBusy)
            yield return null;

        //Check if it was done absorbing succesfully
        if(temp.doneAbsorbing)
        {
            //Increment the soul value 
            soulValue++;
        }
        Destroy(temp.gameObject);
        //Once done, turn isAttacking to false
        isAttacking = false;
    }

    public Vector3 GetWireSpawn(Vector3 targetObjectPosition)
    {
        
        Vector3 temp = Vector3.zero;
        float angle = Random.Range(0, 360) * 0.0174533f;
        // using the equation of circle, getting a point on the circumference of the circle based on an offset that is a randomised value for each enemy
        temp.x = targetObjectPosition.x + (2.5f * Mathf.Cos(angle));

        temp.z = targetObjectPosition.z + (2.5f * Mathf.Sin(angle));


        Vector3 targetPosition = temp;
        if (NavMesh.SamplePosition(temp, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            
            targetPosition = hit.position;

        }
        return targetPosition;
    }

}
