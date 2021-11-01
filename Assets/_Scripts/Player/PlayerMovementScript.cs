using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    private Quaternion lookRotation;
    private Vector3 lookDir;

    //Player movement/Rotation Values
    [Header("Player Movement Values")]
    public float minSM = 0f, maxSM = 1.8f;
    public float speedMultiplier = 0f;
    public float rotationSpeed = 10f;
    private float hitDistance = 100f;

    public float movementSpeed = 5f;

    //[Range(1, 1.7f)] public float moveSpeedModifier = 1f;
    
    private Vector3 targetVector;
    
    public bool canMove = true;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashTime = 0.3f;

    void Start()
    {
        canMove = true;
        speedMultiplier = 0f;
    }
    public float utilityKillCount;
    public void MovementUpdate()
    {
        if (canMove) //player is able to move
        {
            targetVector = new Vector3(InputHandler.instance.movementVector.x, 0f, InputHandler.instance.movementVector.z); //Input converted into Vector3

            MoveToTarget(targetVector);
        }
        else
        {
            Vector3 currentPos = new Vector3();

            currentPos = transform.position; //Pause player position if cannot move

            transform.position = currentPos;
        }
               
        speedMultiplier = 1 + (utilityKillCount * 0.3f);
    }

    public void RotateToTarget(Transform target)
    {
        lookDir = (target.position - transform.position).normalized;
        lookDir.y = 0;
        lookRotation = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * 100f);
    }

    public void RotateToClickLocation()
    {
        //Character rotate to mouse on screen position
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = playerController.mCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out hitDistance))
        {
            //Debug.DrawRay(ray.origin, ray.direction * hitDistance, Color.red);            
            Vector3 targetPoint = ray.GetPoint(hitDistance);
            lookRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * 100f);
        }
    }

    public void MoveToTarget(Vector3 target) //Move and then rotate character to target direction
    {        
        //moveSpeedModifier = Mathf.Lerp(1f, 1.7f, playerController.speedMultiplier);

        float speed = movementSpeed * speedMultiplier;

        target = Quaternion.Euler(0f, playerController.mCamera.transform.eulerAngles.y, 0f) * target;

        playerController.charController.Move(target.normalized * speed * Time.deltaTime);

        if (target != Vector3.zero && !playerController.isBusy) //If input ongoing, update player rotation
        {
            lookRotation = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed);
        }
    }

    public IEnumerator Dash(float resetTime)
    {        
        float startTime = Time.time; //Set initial time stamp

        playerController.isBusy = true;
        //canMove = false;
        GameController.isPlayerDashing = true;

        //Animate

        while (Time.time < (startTime+dashTime))
        {
            playerController.charController.Move(transform.forward * dashSpeed * Time.deltaTime);

            yield return null;
        }

        canMove = true;
        playerController.isBusy = false;
        yield return new WaitForSeconds(resetTime);
        GameController.isPlayerDashing = false;
    }

}
