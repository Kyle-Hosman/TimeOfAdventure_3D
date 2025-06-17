using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyNPC : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Optional")]
    public Animator animator;

    [Header("UI")]
    public GameObject healthBarPrefab;

    private GameObject healthBarInstance;
    private UnityEngine.UI.Slider healthBarSlider;
    private Canvas healthBarCanvas;
    private float healthBarYOffset = 2.2f; // Adjust as needed for your model
    private bool healthBarVisible = false;
    private float healthBarHideDelay = 3f;
    private float lastDamageTime;

    private bool isDead = false;

    private CharacterController controller;
    private float verticalVelocity = 0f;
    public float gravity = 20f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHealth = maxHealth;
        if (animator == null)
            animator = GetComponent<Animator>();
        // Instantiate health bar
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform);
            healthBarCanvas = healthBarInstance.GetComponent<Canvas>();
            healthBarSlider = healthBarInstance.GetComponentInChildren<UnityEngine.UI.Slider>();
            healthBarInstance.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onDealDamage += OnDealDamage;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.playerEvents.onDealDamage -= OnDealDamage;     
    }

    private void OnDealDamage(GameObject target, int amount)
    {
        if (target == this.gameObject)
        {
            Debug.Log($"Enemy {gameObject.name} took {amount} damage.");
            TakeDamage(amount);
        }
    }

    // Call this from your player's attack logic (e.g., via collision, trigger, or raycast)
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (animator != null)
            animator.SetTrigger("GetHit"); // Optional: play hit animation
        // Show and update health bar
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(true);
            healthBarVisible = true;
            lastDamageTime = Time.time;
            if (healthBarSlider != null)
                healthBarSlider.value = (float)currentHealth / maxHealth;
        }
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
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        // Gravity and ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
        else
            verticalVelocity -= gravity * Time.deltaTime;
        controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);

        // Position health bar above head
        if (healthBarInstance != null)
        {
            Vector3 worldPos = transform.position + Vector3.up * healthBarYOffset;
            healthBarInstance.transform.position = worldPos;
            // Optionally, face camera
            if (Camera.main != null)
                healthBarInstance.transform.LookAt(Camera.main.transform);
            // Hide after delay
            if (healthBarVisible && Time.time - lastDamageTime > healthBarHideDelay)
            {
                healthBarInstance.SetActive(false);
                healthBarVisible = false;
            }
        }
    }
}