using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 attackDirection;

    public float speed = 2.5f;

    public float playerDistance;

    public float destroyTime;

    public Vector3 playerAngle;

    public float offsetAngle;

    public float projectileDamage;
    void Start()
    {
        InitialiseProjectile();
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsTarget();
       // Debug.DrawRay(this.transform.position, this.transform.forward * 5);
    }
    void InitialiseProjectile()
    {
        playerDistance = Vector3.Distance(GameController.instance.Player.transform.position, this.transform.position);
        destroyTime = playerDistance / speed;
        GetAttackDirection();
        Destroy(this.gameObject,destroyTime);
    }
    void GetAttackDirection()
    {
        playerAngle = GameController.instance.Player.transform.position - this.transform.position;
        //playerAngle.y = 0;
        this.transform.rotation = Quaternion.LookRotation(playerAngle);
        offsetAngle = Random.Range(0f, 15f) * ((Random.Range(0,2)*2)-1);
        playerAngle = Quaternion.AngleAxis(offsetAngle, Vector3.up) * this.transform.forward;
    }

    void MoveTowardsTarget()
    {
        // GetAttackDirection();
        // Debug.DrawRay(this.transform.position, Vector3.Normalize(GameController.instance.Player.transform.position - this.transform.position) * 5);
        // Debug.DrawRay(this.transform.position, playerAngle.normalized*5);
        this.transform.Translate(playerAngle.normalized * speed * Time.deltaTime, Space.World);

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            //Damage the player
            Debug.Log("Player Hit by projectile");
            GameController.instance.Player.GetComponent<HealthScript>().Damage((int)projectileDamage);
            Destroy(this.gameObject);
        }
        else if(other.tag!="Enemy")
        {
            //Add some sort of minor explosion effect

            Destroy(this.gameObject);
        }
    }

}
