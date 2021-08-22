using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigChungusCombat : EnemyCombat
{
    //attack prefab
    public GameObject attackPrefab;

    public override void Attack()
    {
        StartCoroutine(BigChungusAttack());
    }

    public IEnumerator BigChungusAttack()
    {
        isBusy = true;
        DamageZone damageZone = Instantiate(attackPrefab, transform.position, transform.rotation).GetComponent<DamageZone>();
        damageZone.damage = damage;

        yield return new WaitForSeconds(strikeDelay);

        isBusy = false;
        isAttacking = false;

    }
}
