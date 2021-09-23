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
    public static float deathTimerDuration = 5f;

    public bool isDead;

    void Awake()
    {
        isDead = false;
    }

    private void Start()
    {
        activeDeathTimer = false;

        //Invoke("ActivateHealth", 10f);
    }

    public void HealthUpdate()
    {
        //currentHealth = //Utility states

        if (staggered)
            playerController.isInvulnerable = true;
        else
            playerController.isInvulnerable = false;
        
        if(EnemySpawner.instance != null)
            currentHealth = EnemySpawner.instance.UtilityStatesInTheScene.Count;

        if (!GameController.instance.gameStarted && currentHealth > 0)
            GameController.instance.gameStarted = true;

        if(currentHealth == 0 && !activeDeathTimer && GameController.instance.gameStarted && !isDead)
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
        float timer = deathTimerDuration;

        playerController.deathTimer.text = deathTimerDuration.ToString();
        playerController.deathTimerDisplay.SetActive(true);

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (currentHealth > 0)
            {
                Debug.Log("Health Acquired!");
                break;
            }

            playerController.deathTimer.text = GameController.instance.FormatTime(timer);
            yield return null;
        }

        if (timer <= 0)
        {
            isDead = true;
            Debug.Log("You dead for reals this time");
        }

        yield return null;
        playerController.deathTimerDisplay.SetActive(false);
        activeDeathTimer = false;
    }

    void ActivateHealth()
    {
        GameController.instance.gameStarted = true;
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
