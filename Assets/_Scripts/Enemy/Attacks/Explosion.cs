using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    //Time until the explosion
    public float timeDuration = 2.5f;

    //Indicator color at the start
    public Color startColor=Color.white;


    //Indicator color at the time of explosion
    public Color stopColor=Color.red;

    //Collider that will be used to check whether player is in the zone or not

    public CapsuleCollider capsuleCollider;
    void Start()
    {
        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        StartCoroutine(InitiateExplosion(timeDuration,0.3f));
    }
    IEnumerator InitiateExplosion(float _time,float _destroy)
    {
        float currentTime = 0;

        //Lerps the color of indicator over the given time duration
        while (currentTime < _time)
        {
            float t = currentTime / _time;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.Lerp( startColor, stopColor, t)) ;
            currentTime += Time.deltaTime;
            yield return null;
        }
        //Once the color changes the collider is enabled
        capsuleCollider.enabled = true;

        //The object is destroyed after a specific time period which marks the end of the explosion
        yield return new WaitForSeconds(_destroy);

        Destroy(this.gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            //Damage the player
            Debug.Log("Player Caught In Explosion");
        }
    }


}
