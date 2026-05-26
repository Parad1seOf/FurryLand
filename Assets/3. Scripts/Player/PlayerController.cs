using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputReader))]
[RequireComponent(typeof(FirstPersonLook))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(StaminaSystem))]
[RequireComponent(typeof(FootstepController))]
[RequireComponent(typeof(InventorySystem))]
[RequireComponent(typeof(GroundCheck))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameManager  s_gameManager;
    public AudioManager s_audioManager;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed  = 12f;

    private float currentWalkSpeed;
    private float currentRunSpeed;

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
    private GroundCheck         groundCheck;
    private float               verticalSpeed;

    private List<ISpeedModifier> speedModifiers = new();

    private void Awake()
    {
        cc        = GetComponent<CharacterController>();
        input     = GetComponent<InputReader>();
        look      = GetComponent<FirstPersonLook>();
        health    = GetComponent<HealthSystem>();
        stamina   = GetComponent<StaminaSystem>();
        footsteps = GetComponent<FootstepController>();
        inventory = GetComponent<InventorySystem>();
        groundCheck = GetComponent<GroundCheck>();

        health.OnDamaged     += () => { s_audioManager?.PlayerHit(); s_gameManager?.ShowHitFlash(); };
        health.OnDeath       += OnDeath;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        currentWalkSpeed = walkSpeed;
        currentRunSpeed = runSpeed;

        StartCoroutine(ShowStartComicPanel());
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

        if (canJump && groundCheck.IsGrounded() && input.JumpPressed)
            verticalSpeed = jumpSpeed;

        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        move   *= (IsRunning ? currentRunSpeed : currentWalkSpeed) * Time.deltaTime;
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

    public void AddSpeedModifier(ISpeedModifier speedModifier)
    {
        if (!speedModifiers.Any(m => m.GetType() == speedModifier.GetType()))
            speedModifiers.Add(speedModifier);

        ModifySpeed();
    }

    public void RemoveSpeedModifier(ISpeedModifier speedModifier)
    {
        speedModifiers.RemoveAll(m => m.GetType() == speedModifier.GetType());

        ModifySpeed();
    }

    private void ModifySpeed()
    {
        float modifier = 0;

        foreach (ISpeedModifier speedModifier in speedModifiers)
        {
            modifier += speedModifier.GetValue();
        }

        currentWalkSpeed = Mathf.Max(walkSpeed + modifier, 0);
        currentRunSpeed = Mathf.Max(runSpeed + modifier, 0);
    }

    private System.Collections.IEnumerator ShowStartComicPanel()
    {
        yield return new WaitForSeconds(0.1f);

        if (ComicPanelManager.Instance != null)
            ComicPanelManager.Instance.ShowPhraseByID("Start_Game", 5f);
    }
}