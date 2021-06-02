using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Awake()    
    {
        currentHealth = maxHealth; //Ensure on spawn health is at max
    }

    private void Update()
    {
        //Damage(0); //Test destroy to avoid update check
    }

    private void OnDestroy()
    {
        //do something fancy
        //Debug.Log(gameObject.name + " DED");
    }

    public int GetHealth()
    {
        return currentHealth;
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
