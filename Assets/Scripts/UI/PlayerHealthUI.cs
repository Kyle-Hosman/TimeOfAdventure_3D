using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    private void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onPlayerHealthChange += PlayerHealthChange;
    }

    private void OnDisable() 
    {
        GameEventsManager.instance.playerEvents.onPlayerHealthChange -= PlayerHealthChange;
    }

    private void PlayerHealthChange(int health) 
    {
        healthSlider.value = (float) health / (float) 100; // Assuming 100 is the max health
        healthText.text = health + " / 100"; // Update this if max health is dynamic
    }
}
