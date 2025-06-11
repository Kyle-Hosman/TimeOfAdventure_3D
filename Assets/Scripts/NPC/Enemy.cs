using UnityEngine;

public class EnemyNPC : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Optional")]
    public Animator animator;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Call this from your player's attack logic (e.g., via collision, trigger, or raycast)
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (animator != null)
            animator.SetTrigger("Hit"); // Optional: play hit animation

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        if (animator != null)
            animator.SetTrigger("Die"); // Optional: play death animation

        // Disable enemy logic, collider, etc.
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Optionally destroy after delay
        Destroy(gameObject, 2f);
    }
}