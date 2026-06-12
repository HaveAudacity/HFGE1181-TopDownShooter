using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    private Rigidbody2D rb;
    private Camera cam;
    private PlayerWeaponManager weaponManager;
    private PlayerHealth health;

    private Vector2 moveInput;
    private Vector2 mousePos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        weaponManager = GetComponent<PlayerWeaponManager>();
        health = GetComponent<PlayerHealth>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext ctx)
    {
        mousePos = ctx.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            weaponManager.OnShoot(ctx);
    }

    private void FixedUpdate()
    {
        Vector2 dir = moveInput.normalized * moveSpeed;
        rb.linearVelocity = dir;

        Rotate();
    }

    private void Rotate()
    {
        Vector2 world = cam.ScreenToWorldPoint(mousePos);
        Vector2 dir = world - rb.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }
}