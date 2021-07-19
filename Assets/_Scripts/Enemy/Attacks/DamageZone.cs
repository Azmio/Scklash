using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{

    public float damage;
    public float angle =55f;
    public float areaRadius =3f;
    public float timeUntilInitialises = 1.5f;
    public float timeItExistsFor = 0.5f;

    SphereCollider damageZone;

    public bool isInZone = false;
    private void Awake()
    {
        damageZone = GetComponent<SphereCollider>();
        damageZone.radius = areaRadius;
        damageZone.enabled = false;
    }
    void Start() 
    {
        StartCoroutine(EnableDamageZone());
    }

    // Update is called once per frame
    IEnumerator EnableDamageZone()
    {
        yield return new WaitForSeconds(timeUntilInitialises);
        damageZone.enabled = true;
        Destroy(this.gameObject, timeItExistsFor);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            float dotProductValue;
            Vector3 playerDir;
            playerDir = (GameController.instance.Player.transform.position - this.transform.position).normalized;


            dotProductValue = Vector3.Dot(playerDir, transform.forward);

            if (dotProductValue >= Mathf.Cos(Mathf.Deg2Rad * angle))
            {
                //damage player
                isInZone = true;
                Debug.Log("Player in the damage zone");
                other.GetComponent<HealthScript>().Damage((int)damage);
            }
            else
            {
                isInZone = false;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (this.enabled)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, areaRadius);
            Gizmos.DrawRay(this.transform.position, transform.forward * areaRadius);
            Gizmos.DrawRay(this.transform.position, Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward * areaRadius);
            Gizmos.DrawRay(this.transform.position, Quaternion.AngleAxis(-angle, Vector3.up) * this.transform.forward * areaRadius);

        }

    }

    /*
          IEnumerator DamagePlayer(float _time)
          {
              Debug.Log("Trigger entered");
              float dotProductValue;
              Vector3 playerDir;
              float startTime = Time.time;

              while (Time.time < startTime + _time)
              {
                  playerDir = (GameController.instance.Player.transform.position - this.transform.position).normalized;


                  dotProductValue = Vector3.Dot(playerDir, transform.forward);

                  if (dotProductValue >= Mathf.Cos(Mathf.Deg2Rad * angle))
                  {
                      //damage player
                      isInZone = true;
                      break;
                  }
                  else
                  {
                      isInZone = false;
                  }
                  yield return null;
              }

              if (isInRadius&&isInZone)
              {
                  //damage player and destroy
              }

              Destroy(this.gameObject);
          }

          void DamageIfInZone()
          {
              Debug.DrawRay(this.transform.position, transform.forward * radius);
              Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(55, Vector3.up) * this.transform.forward*radius);
              Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(-55, Vector3.up) * this.transform.forward*radius);

              float dotProductValue;
              Vector3 playerDir;
              //if (EnemyAI.GetPreciseDistance(this.transform.position, GameController.instance.Player.transform.position)<radius)
              targetRadius = EnemyAI.GetPreciseDistance(this.transform.position, targetObject.transform.position);
              Vector3 offsetPosition = targetObject.transform.position;
              offsetPosition.x = offsetPosition.x + bodyOffset;
              offsetPosition.y = offsetPosition.y + bodyOffset;
              offsetPosition.z = offsetPosition.z + bodyOffset;
              if (targetRadius < radius)
              {

                  //playerDir = (GameController.instance.Player.transform.position - this.transform.position).normalized;
                  playerDir = (offsetPosition - this.transform.position).normalized;


                  dotProductValue = Vector3.Dot(playerDir, transform.forward);

                  if (dotProductValue >= Mathf.Cos(Mathf.Deg2Rad*angle))
                  {
                      //damage player
                      isInZone = true;
                  }
                  else
                  {
                      isInZone = false;
                  }
              }
              else
              {
                  isInZone = false;
              }
          }
          void OnDrawGizmos()
          {
              if (this.enabled)
              {
                  Gizmos.color = Color.red;
                  Gizmos.DrawWireSphere(transform.position, radius);
              }

          }
          */
}
