using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedyCombat : EnemyCombat
{
    // Start is called before the first frame update
    
    CharacterController ec;
    [Header("Attack Parameters")]
    public float knockBackSpeed;
    public void Awake()
    {
        ec = this.gameObject.GetComponent<CharacterController>();
    }
    public override void Attack()
    {
        StartCoroutine(LeapAtTarget(ec, 20f, 0.2f, GameController.instance.Player));
    }
    IEnumerator KnockbackTarget(GameObject target, Vector3 _knockbackDir)
    {

        float startTime = Time.time;

        Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;

        while (Time.time < startTime + 0.2)
        {

            target.GetComponent<CharacterController>().Move(_knockbackDir * knockBackSpeed * Time.deltaTime);
            yield return null;
        }

        yield return null;

        isAttacking = false;
    }

    public IEnumerator LeapAtTarget(CharacterController _controller, float _leapSpeed, float _leapDuration, PlayerController _target)
    {

        yield return new WaitForSeconds(strikeDelay);

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
                _target.GetComponent<PlayerHealthScript>().Damage((int)damage);
                isAttacking = true;
                StartCoroutine(KnockbackTarget(_target.gameObject, transform.forward));
            }
            yield return null;
        }

        doneAttacking = false;
        isBusy = false;

    }

    public override void StopAttacking()
    {
        StopAllCoroutines();
    }
}
