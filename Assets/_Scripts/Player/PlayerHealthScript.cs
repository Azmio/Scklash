using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    public int currentHealth = 0;
    public bool staggered = false;

    public bool activeDeathTimer;
    public float deathTimerDuration = 5f;

    void Awake()
    {

    }

    private void Start()
    {
        activeDeathTimer = false;
    }

    public void HealthUpdate()
    {
        //currentHealth = //Utility states

        if (staggered)
            playerController.isInvulnerable = true;
        else
            playerController.isInvulnerable = false;


        if(currentHealth == 0 && !activeDeathTimer)
        {
            activeDeathTimer = true;

            StartCoroutine(DeathTimer());
        }

        if(activeDeathTimer && currentHealth > 0)
        {
            StopCoroutine(DeathTimer());
        }
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
        }

        if (currentHealth > 0)
            return false;
        else
            return true;
    }

    IEnumerator HitEffect(float invulnDuration)
    {
        //player become invulnerable for x time when damaged.

        while(invulnDuration > 0)
        {
            invulnDuration -= Time.deltaTime;
            playerController.isInvulnerable = true;
        }

        yield return null;
        playerController.isInvulnerable = false;
    }

    IEnumerator DeathTimer()
    {
        playerController.deathTimer.text = deathTimerDuration.ToString();
        playerController.deathTimerDisplay.SetActive(true);

        while(deathTimerDuration > 0)
        {
            deathTimerDuration -= Time.deltaTime;

            playerController.deathTimer.text = GameController.instance.FormatTime(deathTimerDuration);
            yield return null;
        }

        if(deathTimerDuration <= 0)
        {
            Debug.Log("You dead for reals this time");
            yield return null;
        }

        yield return null;
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
}
