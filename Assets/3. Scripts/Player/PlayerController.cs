using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputReader))]
[RequireComponent(typeof(FirstPersonLook))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(StaminaSystem))]
[RequireComponent(typeof(FootstepController))]
[RequireComponent(typeof(InventorySystem))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameManager  s_gameManager;
    public AudioManager s_audioManager;

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed  = 12f;

    [Header("Jump")]
    public bool  canJump   = true;
    public float jumpSpeed = 8f;

    // Propiedades públicas para HUD y otros sistemas
    public bool  IsWalking        { get; private set; }
    public bool  IsRunning        { get; private set; }
    public bool  IsAlive          => health.IsAlive;
    public float HealthNormalised  => health.HealthNormalised;
    public float StaminaNormalised => stamina.StaminaNormalised;
    public float Health            => health.Health;
    public float Stamina           => stamina.Stamina;
    public float maxHealth         => health.maxHealth;
    public float maxStamina        => stamina.maxStamina;
    public Vector3 Position        => transform.position;

    private CharacterController cc;
    private InputReader         input;
    private FirstPersonLook     look;
    private HealthSystem        health;
    private StaminaSystem       stamina;
    private FootstepController  footsteps;
    private InventorySystem     inventory;
    private float               verticalSpeed;

    private void Awake()
    {
        cc        = GetComponent<CharacterController>();
        input     = GetComponent<InputReader>();
        look      = GetComponent<FirstPersonLook>();
        health    = GetComponent<HealthSystem>();
        stamina   = GetComponent<StaminaSystem>();
        footsteps = GetComponent<FootstepController>();
        inventory = GetComponent<InventorySystem>();

        health.OnDamaged     += () => { s_audioManager?.PlayerHit(); s_gameManager?.ShowHitFlash(); };
        health.OnDeath       += OnDeath;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void Update()
    {
        if (!IsAlive) return;

        HandleMovement();
        stamina.Tick(IsRunning);
        footsteps.Tick(IsWalking || IsRunning);

        if (Time.timeScale == 0f)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
    }

    private void HandleMovement()
    {
        bool moving = input.MoveInput != Vector2.zero;
        bool wantsRun = input.WantsToRun && moving;
        IsRunning = wantsRun && stamina.HasStamina;
        IsWalking = moving && !IsRunning;

        float   yawRad  = look.Yaw * Mathf.Deg2Rad;
        Vector3 forward = new Vector3( Mathf.Sin(yawRad), 0f,  Mathf.Cos(yawRad));
        Vector3 right   = new Vector3( Mathf.Cos(yawRad), 0f, -Mathf.Sin(yawRad));

        Vector3 move = (forward * input.MoveInput.y + right * input.MoveInput.x).normalized;

        if (canJump && cc.isGrounded && input.JumpPressed)
            verticalSpeed = jumpSpeed;

        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        move   *= (IsRunning ? runSpeed : walkSpeed) * Time.deltaTime;
        move.y  = verticalSpeed * Time.deltaTime;

        CollisionFlags flags = cc.Move(move);
        if      ((flags & CollisionFlags.Below) != 0)                       verticalSpeed = 0f;
        else if ((flags & CollisionFlags.Above) != 0 && verticalSpeed > 0f) verticalSpeed = 0f;
    }

    // IDamageable para que GunSystem funcione
    public void TakeDamage(float amount)  => health.TakeDamage(amount);
    public void RestoreHealth(float amount) => health.Restore(amount);
    public void RestoreStamina(float amount) => stamina.Restore(amount);

    public void FullHeal()
    {
        health.SetFull();
        stamina.SetFull();
    }

    public void Respawn(Vector3 position, Quaternion rotation)
    {
        FullHeal();
        cc.enabled         = false;
        transform.position = position;
        transform.rotation = rotation;
        cc.enabled         = true;
    }

    private void OnDeath()
    {
        Debug.Log("[PlayerController] Player died.");

        GameResultUI resultsUI = FindFirstObjectByType<GameResultUI>();

        if (resultsUI != null) resultsUI.ShowResults(false);
    }

    public InventorySystem GetInventory() { return inventory; }
}