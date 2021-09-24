using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedCombat : EnemyCombat
{
    //attack prefab
    [Header("Attack Parameters")]
    public GameObject attackPrefab;

    public override void Attack()
    {
        StartCoroutine(ShootProjectile());
    }
    public IEnumerator ShootProjectile()
    {
        isAttacking = true;

        yield return new WaitForSeconds(strikeDelay);

        //Shoot the projectile
        GameObject temp = Instantiate(attackPrefab, this.transform.position, this.transform.rotation);
        temp.GetComponent<Projectile>().projectileDamage = damage;
        Debug.Log("Kashoot!");


        isAttacking = false;
        doneAttacking = true;
    }

    public override void StopAttacking()
    {
        StopAllCoroutines();
    }
}
