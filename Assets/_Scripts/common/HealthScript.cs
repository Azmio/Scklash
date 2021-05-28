using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Start()    
    {
        currentHealth = maxHealth; //Ensure on spawn health is at max
        InitializeIfEnemy();
    }

    private void Update()
    {
        if (currentHealth <=0) // minor test
        {
            Destroy(this.gameObject);
        }
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

    public void Damage(int amount)
    {
        currentHealth -= amount;

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
    }

}
