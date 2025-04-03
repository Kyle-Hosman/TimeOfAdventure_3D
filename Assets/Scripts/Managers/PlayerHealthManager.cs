using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int startingHealth = 100;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    private void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onPlayerHealthGained += HealthGained;
    }

    private void OnDisable() 
    {
        GameEventsManager.instance.playerEvents.onPlayerHealthGained -= HealthGained;
    }

    private void Start()
    {
        GameEventsManager.instance.playerEvents.PlayerHealthChanged(currentHealth);
    }

    private void HealthGained(int healthChange) 
    {
        currentHealth += healthChange;
        //currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);

        // Notify UI only if health actually changes
        if (healthChange != 0)
        {
            GameEventsManager.instance.playerEvents.PlayerHealthChanged(currentHealth);
        }

        if (currentHealth <= 0)
        {
            // Handle player death
            Debug.Log("Player has died.");
            GameEventsManager.instance.playerEvents.PlayerDeath();
        }
    }
}
