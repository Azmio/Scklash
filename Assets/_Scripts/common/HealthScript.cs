using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Slider healthSlider;

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

        healthSlider.value = currentHealth;
    }

    void InitializeIfEnemy()
    {
        if (this.tag == "Enemy")
        {
            EnemySpawner.enemySpawner.enemiesInTheScene.Add(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        //do something fancy

        if (this.tag == "Enemy")
        {
            EnemySpawner.enemySpawner.enemiesInTheScene.Remove(this.gameObject);
        }

        Debug.Log(gameObject.name + " DED");
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void Damage(int amount)
    {
        currentHealth -= amount;

        healthSlider.value = currentHealth;

        if (currentHealth <= 0) //If health depleted, destroy this object
            Destroy(gameObject);

        
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

}
