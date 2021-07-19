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

    public float safeDistance;

    void Awake()    
    {
        currentHealth = maxHealth; //Ensure on spawn health is at max
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

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

    public int GetHealth()
    {
        return currentHealth;
    }

    public void Damage(int amount)
    {
        currentHealth -= amount;

        healthSlider.value = currentHealth;

       // if (currentHealth <= 0) //If health depleted, destroy this object
       //   Destroy(gameObject);

        
    }

    public void Heal(int amount)
    {
        if (amount <= 0)//Full heal
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
                return false;
            else
                return true;

        }
    }

}
