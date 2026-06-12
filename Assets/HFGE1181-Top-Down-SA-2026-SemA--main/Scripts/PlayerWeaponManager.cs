using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon Transforms")]
    public Transform weaponParent;
    [HideInInspector] public Transform weaponFirePoint;

    [Header("Current Weapon Info")]
    public GameObject currentWeaponGameObject;
    public GameObject currentPickup;

    [Header("Drop Settings")]
    public float dropForce = 5f;
    public float randomSpinForce = 300f;
    public float pickupDelay = 10f;

    [Header("Reload UI")]
    [SerializeField] private Slider reloadSlider;

    private WeaponBase currentWeaponScript;
    private PlayerHealth playerHealth;
    private Animator playerAnimator;

    // 🔄 Reload System
    private float reloadTime = 2f;
    private float reloadTimer;
    private bool isReloading;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimator = GetComponentInParent<Animator>();

        // Setup reload UI
        if (reloadSlider != null)
        {
            reloadSlider.minValue = 0;
            reloadSlider.maxValue = reloadTime;
            reloadSlider.value = 0;
        }

        if (currentWeaponGameObject != null && weaponParent != null)
        {
            currentWeaponGameObject = Instantiate(
                currentWeaponGameObject,
                weaponParent.position,
                weaponParent.rotation,
                weaponParent
            );

            EquipWeapon(currentWeaponGameObject);
        }
    }

    private void Update()
    {
        HandleReload();
    }

    // SHOOT INPUT
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (playerHealth != null && playerHealth.isPlayerDead)
            return;

        if (isReloading) return;

        if (context.performed && currentWeaponScript != null)
        {
            currentWeaponScript.TryShoot();

            // OPTIONAL: start reload after shot (if your design wants it)
            // StartReload();
        }
    }

    // 🔄 RELOAD INPUT (CALL THIS FROM INPUT SYSTEM - KEY R)
    public void OnReload(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        StartReload();
    }

    private void StartReload()
    {
        if (isReloading) return;
        if (reloadSlider == null) return;

        isReloading = true;
        reloadTimer = 0f;
    }

    private void HandleReload()
    {
        if (!isReloading) return;

        reloadTimer += Time.deltaTime;

        if (reloadSlider != null)
            reloadSlider.value = reloadTimer;

        if (reloadTimer >= reloadTime)
        {
            FinishReload();
        }
    }

    private void FinishReload()
    {
        isReloading = false;
        reloadTimer = 0f;

        if (reloadSlider != null)
            reloadSlider.value = 0f;
    }

    // WEAPON SWAP
    public void SwapWeapon(GameObject newWeaponPrefab, GameObject newPickupPrefab)
    {
        if (newWeaponPrefab == null || weaponParent == null)
            return;

        if (currentWeaponGameObject != null)
        {
            WeaponData data = currentWeaponGameObject.GetComponent<WeaponData>();

            if (data != null && data.pickupPrefab != null)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                Vector3 dropPos = transform.position + (Vector3)dir * 2f;

                GameObject drop = Instantiate(
                    data.pickupPrefab,
                    dropPos,
                    Quaternion.identity
                );

                Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(dir * dropForce, ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-randomSpinForce, randomSpinForce), ForceMode2D.Impulse);
                }

                StartCoroutine(EnablePickupAfterDelay(drop.GetComponent<Collider2D>()));
            }

            Destroy(currentWeaponGameObject);
        }

        currentWeaponGameObject = Instantiate(
            newWeaponPrefab,
            weaponParent.position,
            weaponParent.rotation,
            weaponParent
        );

        EquipWeapon(currentWeaponGameObject);
    }

    // EQUIP WEAPON
    private void EquipWeapon(GameObject weapon)
    {
        if (weapon == null) return;

        currentWeaponScript = weapon.GetComponent<WeaponBase>();

        WeaponData data = weapon.GetComponent<WeaponData>();
        if (data != null)
        {
            currentPickup = data.pickupPrefab;

            if (playerAnimator != null && !string.IsNullOrEmpty(data.idleTrigger))
            {
                playerAnimator.SetTrigger(data.idleTrigger);
            }
        }

        Transform firePointTransform = weapon.transform.Find("WeaponFirePoint");
        if (firePointTransform != null)
        {
            weaponFirePoint = firePointTransform;
            currentWeaponScript.firePoint = weaponFirePoint;
        }
    }

    // DELAY PICKUP ENABLE
    private IEnumerator EnablePickupAfterDelay(Collider2D col)
    {
        if (col == null) yield break;

        col.enabled = false;
        yield return new WaitForSeconds(pickupDelay);

        if (col != null)
            col.enabled = true;
    }
}