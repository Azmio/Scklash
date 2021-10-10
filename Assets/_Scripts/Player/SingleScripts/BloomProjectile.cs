using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomProjectile : MonoBehaviour
{ 
    void OnBecameInvisible()
    {
        Destroy(gameObject);
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
                    GameController.instance.Player.movementScript.utilityKillCount++;
                }
                else
                {
                    enemyScript.enemyHealthSystem.Damage(1);
                }

                Destroy(this.gameObject);
            }
        }
    }
}
