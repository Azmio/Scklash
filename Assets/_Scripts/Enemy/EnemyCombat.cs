using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    //Damage of each melee attack
    public float meleeDamage = 3f;

    //time delay between each melee hit
    public float meleeHitDelay = 1f;

    //time until the next hit
    public float timeUntilHit = 0;

    //Number of explosions per attack
    public int explosionCount = 3;

    //delay between each explosion
    public float explosionDelay = 0.5f;

    //Number of explosions done already
    public float currentCount = 0;

    //prefab of explosion object
    public GameObject explosionPrefab;


    //is player done attacking, used by the ranged attacks
    public bool doneAttacking = false;

    public void MeleeAttack()
    {
        if (timeUntilHit <= 0)
        {
            GameController.instance.Player.GetComponent<HealthScript>().Damage((int)meleeDamage);
            timeUntilHit = meleeHitDelay;
        }
        else
        {
            timeUntilHit -= Time.deltaTime;
        }
    }

    public void RangedAttack()
    {
        
        if (timeUntilHit <= 0 && currentCount< explosionCount)
        {
            Instantiate(explosionPrefab, GameController.instance.toFollow, GameController.instance.Player.transform.rotation);
            timeUntilHit = explosionDelay;
            currentCount++;
        }
        else if (currentCount < explosionCount)
        {
            timeUntilHit -= Time.deltaTime;
        }
        else
        {
            doneAttacking = true;
            timeUntilHit -= Time.deltaTime;
        }
    }
    

    //Deprecated code which was being used for the explosions
    IEnumerator SpawnExplosions(int _count, float _delay)
    {
        while (currentCount < _count)
        {
            Instantiate(explosionPrefab, GameController.instance.Player.transform.position, GameController.instance.Player.transform.rotation);
            yield return new WaitForSeconds(_delay);
            currentCount++;
        }

        
    }
}
