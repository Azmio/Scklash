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

    public EnemyCombat enemyCombat;

    // a basic finite state machine to make the AI more manageable
    public AIState currentState;

    public float attackRange = 2f;

    public bool isRanged = false;

    public float targetOffset;
    void Start()
    {
        //script still messy try fixing local avoidance, check the video from weird dude's rpg series
        InitialiseEnemy();
    }

    void InitialiseEnemy()//Does what it's intended to do
    {
        //Not sure if we need the character controller anymore but ill let it be
        eController = GetComponent<CharacterController>();

        agent = GetComponent<NavMeshAgent>();

        enemyCombat = GetComponent<EnemyCombat>();

        //So that the agent won't rotate on it's own and that we could do it via our move to player function
        agent.updateRotation = false;

        //Getting a random angle between 0 and 360 and converting it into radian
        targetOffset= Random.Range(0, 360) * 0.0174533f;

        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Movement(this);
    }
    public void MoveToPlayer(Vector3 target) //Move and rotate enemy to player's direction
    {
        agent.destination = GetTarget(target);
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
    Vector3 GetTarget(Vector3 _target)
    {
        Vector3 temp = Vector3.zero;

        // using the equation of circle, getting a point on the circumference of the circle based on an offset that is a randomised value for each enemy
        temp.x = _target.x + (attackRange * Mathf.Cos(targetOffset));

        temp.z = _target.z + (attackRange * Mathf.Sin(targetOffset));
        return temp;
    }

}
