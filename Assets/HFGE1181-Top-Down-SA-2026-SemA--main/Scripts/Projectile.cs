using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // -------------------------
        // PLAYER HIT (enemy bullet)
        // -------------------------
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        // -------------------------
        // ENEMY HIT (player bullet)
        // -------------------------
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                // ONE-SHOT KILL (assignment requirement)
                enemy.TakeDamage(enemy.MaxHealth);
            }

            Destroy(gameObject);
            return;
        }

        // -------------------------
        // ANY OTHER COLLISION
        // -------------------------
        Destroy(gameObject);
    }
}