using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))//other.tag == "Enemy")
        {
            EnemyAI enemyScript = other.GetComponent<EnemyAI>();

            if (enemyScript != null)
            {
                if (enemyScript.enemyType == EnemyAI.Type.Utility)
                {
                    enemyScript.Explode();
                }
                else
                {
                    enemyScript.enemyHealthSystem.Damage(1);
                }

                Destroy(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
