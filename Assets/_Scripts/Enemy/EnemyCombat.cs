using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    //Damage of each attack
    public float damage = 3f;

    //time delay between each hit, also equal to the time required to absorb a utility state
    public float strikeDelay = 1.5f;

    //time required to absorb
    public float timeRequiredToAbsorb = 5f;

    public bool isAttacking = false;

    //is player done attacking, used by the ranged attacks
    public bool doneAttacking = false;

    public int soulValue = 1;

    public bool isBusy = false;


    [Header("Explosion Parameters")]
    public float explosionRange = 10f;
    public int explosionDamage = 4;
    public float explosionKnockBackDuration = 0.4f;


    public virtual void Attack()
    {
        Debug.Log("I do nothing");
    }

    public virtual void StopAttacking()
    {
        Debug.Log("I do nothing");
    }
    public static IEnumerator KnockbackEntity(GameObject target, Vector3 knockbackEpicenter, float playerKnockBackSpeed = 2f, float knockBackDuration = 0.2f)
    {
        Debug.Log("Exploding");
        float startTime = Time.time;

        Vector3 knockbackDirection = (target.transform.position - knockbackEpicenter).normalized;
        Debug.Log("player knockback speed" + playerKnockBackSpeed);
        while (Time.time < startTime + knockBackDuration)
        {
            Debug.Log("adding force");
            target.GetComponent<CharacterController>().Move(knockbackDirection * playerKnockBackSpeed * Time.deltaTime);

            yield return null;
        }

        yield return null;

    }
}






//Deprecated code which was being used for the explosions
/*
     public IEnumerator StealUtility(EnemyAI _target,float _range)
    {
        Debug.Log("Steal Utility");
        isAttacking = true;
        float startTime = Time.time;
        float distanceCheck;
        while (Time.time < startTime + timeRequiredToAbsorb)
        {
            
            distanceCheck = EnemyAI.GetPreciseDistance(_target.transform.position, this.transform.position);
            if (distanceCheck <= _range)
            {
                yield return null;
            }
            else
            {
                isAttacking = false;
                yield break;
            }      
        }
        soulValue += _target.enemyCombat.soulValue;
        _target.DestroyUtility();   
        isAttacking = false;

    }
IEnumerator SpawnExplosions(int _count, float _delay)
{
    while (currentCount < _count)
    {
        Instantiate(explosionPrefab, GameController.instance.Player.transform.position, GameController.instance.Player.transform.rotation);
        yield return new WaitForSeconds(_delay);
        currentCount++;
    }
}
    public void ShootProjectile()
{
    if (timeUntilHit <= 0)
    {
        isAttacking = true;
        Debug.Log("Kashoot!");
        GameObject temp = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
        temp.GetComponent<Projectile>().projectileDamage=damage;
        //temp.projectileDamage = this.damage;
        timeUntilHit = strikeDelay;
        doneAttacking = true;
    }
    else
    {
        isAttacking = false;
        doneAttacking = false;
        timeUntilHit -= Time.deltaTime;
    }
}
    public void StealUtility(EnemyAI _target)
    {
        if (timeUntilAbsorption <= 0)
        {
            _target.DestroyUtility();
            utilitiesAbsorbed++;
            timeUntilAbsorption = timeRequiredToAbsorb;
            doneAttacking = true;

        }
        else
        {
            doneAttacking = false;
            timeUntilAbsorption -= Time.deltaTime;
        }
    }
   public void MeleeAttack()
    {
        if (timeUntilHit <= 0)
        {
            isAttacking = true;
            GameController.instance.Player.GetComponent<HealthScript>().Damage((int)damage);
            //StartCoroutine(KnockbackTarget(GameController.instance.Player));
            timeUntilHit = strikeDelay;
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
            timeUntilHit = strikeDelay;
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
?*/