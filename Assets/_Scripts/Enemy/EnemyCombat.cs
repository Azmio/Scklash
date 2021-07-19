using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    //Damage of each attack
    public float damage = 3f;

    //time delay between each hit
    public float strikeDelay = 1.5f;

    //time until the next hit
    public float timeUntilHit = 0;

    //time left until the utility is absorbed
    public float timeUntilAbsorption = 5;

    //time required to absorb
    public float timeRequiredToAbsorb = 5f;

    //Number of explosions per attack
    public int explosionCount = 3;

    //delay between each explosion

    //Number of explosions done already
    public float currentCount = 0;

    //prefab of explosion object
    public GameObject explosionPrefab;

    public GameObject projectilePrefab;

    public GameObject damageZonePrefab;

    public bool isAttacking = false;

    //is player done attacking, used by the ranged attacks
    public bool doneAttacking = false;

    public int utilitiesAbsorbed = 0;

    public float knockBackSpeed = 20f;

    public bool isBusy = false;

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


    public IEnumerator BigChungusAttack()
    {
        isBusy = true;
        DamageZone damageZone = Instantiate(damageZonePrefab, transform.position, transform.rotation).GetComponent<DamageZone>();
        damageZone.damage = damage;

        yield return new WaitForSeconds(strikeDelay);

        isBusy = false;
        isAttacking = false;
        
    }

    //obsolete
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

    IEnumerator KnockbackTarget(GameObject target,Vector3 _knockbackDir)
    {

        Debug.Log("oki knockbacking daddy uwu");
        float startTime = Time.time;

        Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;

        while (Time.time < startTime + 0.2 )
        {
            // target.GetComponent<CharacterController>().Move(knockbackDirection * knockBackSpeed * Time.deltaTime);
            target.GetComponent<CharacterController>().Move(_knockbackDir * knockBackSpeed * Time.deltaTime);
            yield return null;
        }

        yield return null;

        isAttacking = false;
    }

    public IEnumerator LeapAtTarget(CharacterController _controller, float _leapSpeed, float _leapDuration, PlayerController _target)
    {
        Debug.Log("leaping uwu");

        yield return new WaitForSeconds(1.5f);

        float startTime = Time.time;
        float distanceCheck;

        while (Time.time < startTime + _leapDuration)
        {
            _controller.Move(transform.forward * _leapSpeed * Time.deltaTime);
            distanceCheck = EnemyAI.GetPreciseDistance(_target.transform.position, this.transform.position);
            // Debug.Log("distance is"+distanceCheck);

            if (distanceCheck <= 1.4 && !isAttacking)
            {
                //damage the player
                Debug.Log("KnockBack player uwu");
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
