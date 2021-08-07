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

    //prefab of explosion object
    public GameObject explosionPrefab;

    public GameObject projectilePrefab;

    public GameObject damageZonePrefab;

    //attack prefab
    public GameObject attackPrefab;

    public bool isAttacking = false;

    //is player done attacking, used by the ranged attacks
    public bool doneAttacking = false;

    public int soulValue = 1;

    public float knockBackSpeed = 20f;

    public bool isBusy = false;

    private void OnDisable()
    {
        Debug.Log("this is not enabled anymore " + this.enabled);
    }


    public IEnumerator BigChungusAttack()
    {
        isBusy = true;
        DamageZone damageZone = Instantiate(damageZonePrefab, transform.position, transform.rotation).GetComponent<DamageZone>();
        damageZone.damage = damage;

        yield return new WaitForSeconds(strikeDelay);

        isBusy = false;
        isAttacking = false;
        
    }



    public IEnumerator ShootProjectile()
    {
        isAttacking = true;
        
        yield return new WaitForSeconds(strikeDelay);

        //Shoot the projectile
        GameObject temp = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
        temp.GetComponent<Projectile>().projectileDamage = damage;
        Debug.Log("Kashoot!");


        isAttacking = false;
        doneAttacking = true;
    }
    */
    public IEnumerator ShootProjectile()
    {
        isAttacking = true;
        
        yield return new WaitForSeconds(strikeDelay);

    

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
    IEnumerator KnockbackTarget(GameObject target,Vector3 _knockbackDir)
    {

        float startTime = Time.time;

        Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;

        while (Time.time < startTime + 0.2 )
        {
            
            target.GetComponent<CharacterController>().Move(_knockbackDir * knockBackSpeed * Time.deltaTime);
            yield return null;
        }

        yield return null;

        isAttacking = false;
    }

    public IEnumerator LeapAtTarget(CharacterController _controller, float _leapSpeed, float _leapDuration, PlayerController _target)
    {

        yield return new WaitForSeconds(1.5f);

        float startTime = Time.time;
        float distanceCheck;

        while (Time.time < startTime + _leapDuration)
        {
            if (this.enabled == false)
            {
                Debug.Log("Cancelling Leap");
                yield break;
            }
            _controller.Move(transform.forward * _leapSpeed * Time.deltaTime);
            distanceCheck = EnemyAI.GetPreciseDistance(_target.transform.position, this.transform.position);
            // Debug.Log("distance is"+distanceCheck);

            if (distanceCheck <= 1.4 && !isAttacking)
            {
                //damage the player
                _target.GetComponent<HealthScript>().Damage((int)damage);
                isAttacking = true;
                StartCoroutine(KnockbackTarget(_target.gameObject, transform.forward));
            }
            yield return null;
        }

        doneAttacking = false;
        isBusy = false;

    }
}






//Deprecated code which was being used for the explosions
/*
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