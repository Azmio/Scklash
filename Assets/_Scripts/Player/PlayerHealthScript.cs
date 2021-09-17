using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    public int currentHealth = 0;
    public bool staggered = false;

    public float deathTimerDuration = 5f;

    void Awake()
    {

    }

    public void HealthUpdate()
    {
        //currentHealth = //Utility states

        if(currentHealth == 0)
        {

        }

        if (staggered)
            playerController.isInvulnerable = true;
        else
            playerController.isInvulnerable = false;
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
        if (!playerController.isInvulnerable) //Check if able to damage
        {
            currentHealth -= amount;
            playerController.healthSlider.value = currentHealth;
        }

        if (currentHealth > 0)
            return false;
        else
            return true;
    }

    /*public void Heal(int amount)
    {
        if (amount <= 0) //Full heal
            currentHealth = maxHealth;
        else
        {
            int newHealth = currentHealth + amount; //Calculate potential health

            if (newHealth > maxHealth)
                currentHealth = maxHealth; //Health will exceed maximum - set to maximum
            else
                currentHealth = newHealth; //Health below maximum - set to new health           
        }

        playerController.healthSlider.value = currentHealth;

    }*/

    void Stagger()
    {

    }

    IEnumerator StartTime(float maxTime)
    {
        yield return null; 
    }
}
