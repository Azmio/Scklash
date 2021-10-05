using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject healthBarCanvas;
    public Slider healthSlider;
    public bool invulnerable;
    public float safeDistance;

    void Awake()    
    {
        currentHealth = maxHealth; //Ensure on spawn health is at max

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        invulnerable = false;
    }

    private void Update()
    {
        //Damage(0); //Test destroy to avoid update check
    }

    private void OnDestroy()
    {
        //do something fancy
       // Debug.Log(gameObject.name + " DED");
    }

    public int GetHealth() //Return Health Check
    {
        return currentHealth;
    }

    public bool Damage(int amount) //Damage this Object
    {
        if(!invulnerable) //Check if able to damage
        {
            currentHealth -= amount;
            FMODUnity.RuntimeManager.PlayOneShot ("event:/MetalHit",  transform.position);              //PLAY SOUND OF BEING HIT
            healthSlider.value = currentHealth;
        }

        if (currentHealth > 0)
            return false;
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot ("event:/EnemyDeath1",  transform.position);            //PLAY SOUND OF DEATH
            return true;
        }
           

       // if (currentHealth <= 0) 
        
       //   Destroy(gameObject); If health depleted, destroy this object
    }

    public void Heal(int amount)
    {
        if (amount <= 0) //Full heal
            currentHealth = maxHealth;
        else
        {
            int newHealth = currentHealth + amount; //Calculate potential health

            if(newHealth > maxHealth)
                currentHealth = maxHealth; //Health will exceed maximum - set to maximum
            else            
                currentHealth = newHealth; //Health below maximum - set to new health           
        }

        healthSlider.value = currentHealth;

    }

    public bool CheckIfVulnerable()
    {
        if (EnemySpawner.instance.UtilityStatesInTheScene.Count == 0)
            return true;
        else
        {
            float minDistance = float.MaxValue;
            foreach (EnemyAI utility in EnemySpawner.instance.UtilityStatesInTheScene)
            {
                float currentDis = EnemyAI.GetPreciseDistance(this.gameObject.transform.position, utility.transform.position);
                if (currentDis < minDistance)
                {
                    minDistance = currentDis;
                }
            }
            if (minDistance <= safeDistance)
            {
                //enable obstacle
                return false;
            }
                
            else
            {
                //disable obstacle
                return true;
            }
                

        }
    }

}
