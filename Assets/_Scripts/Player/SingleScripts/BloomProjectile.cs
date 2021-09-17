using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Enemy")
        {
            EnemyAI enemyScript = other.GetComponent<EnemyAI>();

            if(enemyScript != null)
            {
                /*if(enemyScript.isUtility)
                {
                    //explode script
                }
                else
                {
                    //deal a ping of damage
                }*/
            }
            else
            {
                //probably a breakable object - do something?
            }

            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
