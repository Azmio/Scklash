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

    //an offset which is nothing but a random angle that will be used to position the enemy on a random point which will be in the attack radius
    public float targetOffset;

    public bool changePosition = false;

    void Start()
    {
        InitialiseEnemy();
    }

    void InitialiseEnemy()//Does what it's intended to do
    {
        //Not sure if we need the character controller anymore but ill let it be
        eController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        //So that the agent won't rotate on it's own and that we could do it via our move to player function
        agent.updateRotation = false;
        //Getting a random angle between 0 and 360 and converting it into radian which will be the enemy offset
        GetRandomAngle();

        //Setting the nav mesh agent's speed as the move speed
        agent.speed = moveSpeed;
    }

    public void MoveToPlayer(Vector3 target,float _attackRange) //Give the navmesh agent a target point
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
    Vector3 GetTarget(Vector3 _target, float _attackRange)
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
        if (_previous == 0)
            targetOffset = Random.Range(0, 360) * 0.0174533f;
        else
            targetOffset = _previous + (Random.Range(10, 30) * 0.0174533f * (Random.Range(0, 2) * 2 - 1));
    }

}
