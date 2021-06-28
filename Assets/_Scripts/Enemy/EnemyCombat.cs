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

    public bool isAttacking = false;

    //is player done attacking, used by the ranged attacks
    public bool doneAttacking = false;

    public int utilitiesAbsorbed = 0;

    public float knockBackSpeed = 2f;

    public void MeleeAttack()
    {
        if (timeUntilHit <= 0)
        {
            isAttacking = true;
            GameController.instance.Player.GetComponent<HealthScript>().Damage((int)meleeDamage);
            //StartCoroutine(KnockbackTarget(GameController.instance.Player));
            timeUntilHit = meleeHitDelay;
        }
        else
        {
            isAttacking = false;
            timeUntilHit -= Time.deltaTime;
        }
    }

    public void RangedAttack()
    {
        
        if (timeUntilHit <= 0 && currentCount< explosionCount)
        {
            isAttacking = true;
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
            isAttacking = false;
            doneAttacking = true;
            timeUntilHit -= Time.deltaTime;
        }
    }
    
    public void StealUtility(EnemyAI _target)
    {
        if (timeUntilHit <= 0)
        {
            _target.DestroyUtility();
            utilitiesAbsorbed++;
            timeUntilHit = 5;
            doneAttacking = true;

        }
        else
        {
            doneAttacking = false;
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

    IEnumerator KnockbackTarget(GameObject target)
    {
        isAttacking = true;

        float startTime = Time.time;

        Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;

        while (Time.time < startTime )
        {
            target.GetComponent<CharacterController>().Move(knockbackDirection * knockBackSpeed * Time.deltaTime);

            yield return null;
        }

        yield return null;

        isAttacking = false;
    }
}
