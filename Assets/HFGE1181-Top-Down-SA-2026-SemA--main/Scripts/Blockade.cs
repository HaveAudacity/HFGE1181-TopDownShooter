using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Blockade : MonoBehaviour
{
    [SerializeField] private GameObject crate;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damageTick = 5f;

    private float health;
    private bool playerInRange;
    private bool crateActive;

    private void Start()
    {
        health = maxHealth;
        SetCrateState(false);
    }

    private void Update()
    {
        if (!crateActive) return;

        if (health > 0f)
        {
            health -= damageTick * Time.deltaTime;

            if (health <= 0f)
            {
                BreakCrate();
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange) return;
        if (!context.performed) return;

        RepairCrate();
    }

    private void RepairCrate()
    {
        health = maxHealth;
        SetCrateState(true);
    }

    private void BreakCrate()
    {
        SetCrateState(false);
    }

    private void SetCrateState(bool state)
    {
        crateActive = state;
        crate.SetActive(state);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        UIManager.Instance?.UpdateInteractText("E");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        UIManager.Instance?.UpdateInteractText("");
    }
}