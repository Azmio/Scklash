using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float moveSpeed = 5f;
    public CharacterController eController;

    void Start()
    {
        eController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToPlayer(GameController.instance.toFollow);
    }
    private void MoveToPlayer(Vector3 target) //Move and rotate enemy to player's direction
    {
        float speed = moveSpeed * Time.deltaTime;
        target = Vector3.Normalize(target - this.transform.position);
        eController.Move(target * speed);

        Quaternion lookDirection;
        lookDirection = Quaternion.LookRotation(target);
        eController.transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, rotationSpeed);
        

    }
}
