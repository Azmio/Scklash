using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeDuration = 2.5f;
    public Vector3 target;
    public Color startColor=Color.white;
    public Color stopColor=Color.red;
    public CapsuleCollider capsuleCollider;
    void Start()
    {
        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        StartCoroutine(InitiateExplosion(timeDuration,0.3f));
    }
    IEnumerator InitiateExplosion(float _time,float _destroy)
    {
        float currentTime = 0;
        while (currentTime < _time)
        {
            float t = currentTime / _time;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.Lerp( startColor, stopColor, t)) ;
            currentTime += Time.deltaTime;
            yield return null;
        }

        capsuleCollider.enabled = true;

        yield return new WaitForSeconds(_destroy);

        Destroy(this.gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player Caught In Explosion");
        }
    }


}
