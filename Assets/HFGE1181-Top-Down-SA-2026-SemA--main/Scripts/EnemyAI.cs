using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attackRange = 3f;

    public GameObject projectile;
    public Transform firePoint;
    public float fireRate = 1.5f;

    private Transform player;
    private float fireTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > attackRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            Instantiate(projectile, firePoint.position, firePoint.rotation);
            fireTimer = fireRate;
        }
    }
    public void ApplyKnockback(Vector2 direction, float force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb == null) return;

        rb.linearVelocity = direction.normalized * force;
    }
}