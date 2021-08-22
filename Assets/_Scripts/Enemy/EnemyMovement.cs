using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float turnSpeed = 3f;

    public CharacterController eController;
    
    //testing NavMesh to make the Enemies "smarter"
    public NavMeshAgent agent;
    public float agentRadius;
    public NavMeshObstacle obstacle;

    //an offset which is nothing but a random angle that will be used to position the enemy on a random point which will be in the attack radius
    public float targetOffset;

    public GameObject enemyTarget;

    public Vector3 targetPosition= Vector3.zero;

    float minDistance = float.MaxValue;

    public bool isChangingPosition = false;

    public void InitialiseMovement()//Does what it's intended to do
    {
        //Not sure if we need the character controller anymore but ill let it be
        eController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        obstacle.enabled = false;
        agentRadius = agent.radius;

        //So that the agent won't rotate on it's own and that we could do it via our move to player function
        agent.updateRotation = false;

        //Getting a random angle between 0 and 360 and converting it into radian which will be the enemy offset
        GetRandomAngle();


        //Setting the nav mesh agent's speed as the move speed
        agent.speed = moveSpeed;
    }

    public void MoveToTarget(Vector3 target,float _attackRange) //Give the navmesh agent a target point
    {
        agent.destination = GetTarget(target, _attackRange);
        return;
    }


    public void UpdateDirection(Vector3 target)
    {
        Vector3 lookPos = target - transform.position;
        lookPos.y = 0;

        //Look direction take's the player's position in order to change the rotation of the enemies to make it seem more natural and to get more control over the rotation
        Quaternion lookDirection;
        lookDirection = Quaternion.LookRotation(lookPos);

        // Spherical lerp to get a smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * turnSpeed);

    }



    // A function that would assign the player's target as a point that is around the player within the attack range instead of the player itself
    // thus making the enemies seem like they surround the player and also decrease the clutter where the enemies try and push each other when in large groups
    public Vector3 GetTarget(Vector3 _target, float _attackRange)
    {
        Vector3 temp = Vector3.zero;
        // using the equation of circle, getting a point on the circumference of the circle based on an offset that is a randomised value for each enemy
        temp.x = _target.x + (_attackRange * Mathf.Cos(targetOffset));

        temp.z = _target.z + (_attackRange * Mathf.Sin(targetOffset));
        return temp;
    }


    //Give a Random offset angle in radian if previous value was nil otherwise give an angle that is 5-30 degrees greater or lesser
    public void GetRandomAngle(float _previous=0)
    {
        //Debug.Log("Changing angle");
        if (_previous == 0)
            targetOffset = Random.Range(0, 360) * 0.0174533f;
        else
            targetOffset = _previous + (Random.Range(15, 30) * 0.0174533f * (Random.Range(0, 2) * 2 - 1));
    }
   
    public void GetUtilityTarget()
    {
        minDistance = float.MaxValue;
        foreach (EnemyAI target in EnemySpawner.instance.UtilityStatesInTheScene)
        {
            if(EnemyAI.GetPreciseDistance(this.gameObject.transform.position, target.transform.position)<minDistance)
            {
                minDistance = EnemyAI.GetPreciseDistance(this.gameObject.transform.position, target.transform.position);
                enemyTarget = target.gameObject;
            }
        }
    }

    public bool IsAtTarget(Vector3 _target)
    {
        float tempDistance = EnemyAI.GetPreciseDistance(this.gameObject.transform.position, _target);
        //Debug.Log("Is at target distance " + tempDistance);
        if ( tempDistance<= 1)
            return true;

        return false;
    }

    public Vector3 GetRandomDirection()
    {
        Vector3 temp = (GameController.instance.toFollow - transform.position).normalized;
        temp = Quaternion.AngleAxis(Random.Range(25, 45), Vector3.up) * temp;


        // return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        return temp;
    }

    public void GetRandomPoint(Vector3 _currentPosition)
    {
        targetPosition = _currentPosition + GetRandomDirection() * Random.Range(10f, 15f);
        Vector3 temp = targetPosition;
        if(NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas))
        {
            Debug.Log("Changed the dood's location to a point on the nav mesh");
            targetPosition = hit.position;

        }
    }
}


/*
 Flying enemy code
   public void HoverInPlace(float _offset)
   {
       agent.enabled = false;
       // make player float up and down when sitting idle
       Vector3 temp = transform.position;
       temp.y= flyHeight + Mathf.Sin(Time.fixedTime * Mathf.PI)*_offset;
       transform.position = temp;
   }


   public void FlyToTarget(float attackRange)
   {
       UpdateDirection(enemyTarget.transform.position);
       targetPosition = GetTarget(enemyTarget.transform.position, attackRange);
       Vector3 targetDirection = targetPosition - this.transform.position;
       targetDirection.y = 0;
       HoverInPlace(floatOffset);
       eController.Move(targetDirection.normalized * moveSpeed * Time.deltaTime);
   }
   public IEnumerator Fly(float _time,float _smoothingVar)
   {
       Debug.Log("Making the enemy fly");
       isFlying = true;
       float controlVar = 0;
       float increment = _smoothingVar / _time;
       Vector3 temp;
       while (controlVar<=1)
       {
           temp = this.gameObject.transform.position;
           temp.y = Mathf.Lerp(1, flyHeight, controlVar);
           this.gameObject.transform.position = temp;

           controlVar += increment;

           yield return new WaitForSeconds(_smoothingVar);
       }

       yield return new WaitForSeconds(_time / 2);
       //changingYPosition = false;
   }

   public IEnumerator Land(float _time, float _smoothingVar)
   {
       Debug.Log("Making the enemy land");
       float startHeight = this.gameObject.transform.position.y;
       float controlVar = 0;
       float increment = _smoothingVar / _time;
       isFlying = false;
       Vector3 temp;
       while (controlVar <= 1)
       {
           temp = this.gameObject.transform.position;

           temp.y = Mathf.Lerp(startHeight,1.08f, controlVar);

           this.gameObject.transform.position = temp;

           controlVar += increment;

           yield return new WaitForSeconds(_smoothingVar);
       }

       yield return new WaitForSeconds(_time / 2);
       //changingYPosition = false;
   }

   */