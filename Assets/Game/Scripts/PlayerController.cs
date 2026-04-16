// Controlador principal del jugador. Gestiona la cámara, el movimiento, la stamina y la salud.
// Implementa IDamageable para recibir daño de GunSystem. Notifica su muerte via evento OnDeath
// y llama a AudioManager y GameManager para feedback de audio y pantalla.
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageable
{
    #region Inspector Fields

    [Header("References")]
    public GameManager  s_gameManager;
    public AudioManager s_audioManager;
    public Transform    pitchController;

    [Header("Mouse Look")]
    public float yawSpeed   = 120f;
    public float pitchSpeed = 120f;
    public float minPitch   = -80f;
    public float maxPitch   =  80f;

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed  = 12f;

    [Header("Jump")]
    public bool  canJump   = true;
    public float jumpSpeed = 8f;

    [Header("Stamina")]
    public float maxStamina           = 100f;
    public float staminaDrainRate     = 20f;
    public float staminaRechargeRate  = 15f;
    public float staminaRechargeDelay = 2f;

    [Header("Keybindings")]
    public KeyCode keyLeft  = KeyCode.A;
    public KeyCode keyRight = KeyCode.D;
    public KeyCode keyUp    = KeyCode.W;
    public KeyCode keyDown  = KeyCode.S;
    public KeyCode keyRun   = KeyCode.LeftShift;
    public KeyCode keyJump  = KeyCode.Space;

    [Header("Stats")]
    public float maxHealth = 100f;

    #endregion

    #region Public State

    public float Health  { get; private set; }
    public float Stamina { get; private set; }

    public float StaminaNormalised => maxStamina > 0f ? Stamina / maxStamina : 0f;
    public float HealthNormalised  => maxHealth  > 0f ? Health  / maxHealth  : 0f;

    public bool IsWalking { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsAlive   => Health > 0f;

    #endregion

    #region Private State

    private CharacterController cc;
    private float yaw;
    private float pitch;
    private float verticalSpeed;
    private float staminaRechargeTimer;
    private bool  isPlayingSteps;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Health  = maxHealth;
        Stamina = maxStamina;

        yaw   = transform.eulerAngles.y;
        pitch = pitchController != null ? pitchController.localEulerAngles.x : 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void Update()
    {
        if (!IsAlive) return;

        HandleLook();
        HandleMovement();
        HandleStamina();
        HandleFootsteps();
    }

    #endregion

    #region Look

    private void HandleLook()
    {
        yaw   += Input.GetAxis("Mouse X") * yawSpeed   * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * pitchSpeed * Time.deltaTime;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (pitchController != null)
            pitchController.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    #endregion

    #region Movement

    private void HandleMovement()
    {
        bool movingHorizontally = Input.GetKey(keyUp)   || Input.GetKey(keyDown) ||
                                  Input.GetKey(keyLeft) || Input.GetKey(keyRight);

        bool wantsToRun = Input.GetKey(keyRun) && movingHorizontally;
        IsRunning = wantsToRun && Stamina > 0f;
        IsWalking = movingHorizontally && !IsRunning;

        float   yawRad  = yaw * Mathf.Deg2Rad;
        Vector3 forward = new Vector3( Mathf.Sin(yawRad), 0f,  Mathf.Cos(yawRad));
        Vector3 right   = new Vector3( Mathf.Cos(yawRad), 0f, -Mathf.Sin(yawRad));

        Vector3 move = Vector3.zero;
        if (Input.GetKey(keyUp))    move += forward;
        if (Input.GetKey(keyDown))  move -= forward;
        if (Input.GetKey(keyRight)) move += right;
        if (Input.GetKey(keyLeft))  move -= right;
        move.Normalize();

        if (canJump && cc.isGrounded && Input.GetKeyDown(keyJump))
            verticalSpeed = jumpSpeed;

        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        float speed = IsRunning ? runSpeed : walkSpeed;
        move        *= speed * Time.deltaTime;
        move.y       = verticalSpeed * Time.deltaTime;

        CollisionFlags flags = cc.Move(move);
        if      ((flags & CollisionFlags.Below) != 0)                       verticalSpeed = 0f;
        else if ((flags & CollisionFlags.Above) != 0 && verticalSpeed > 0f) verticalSpeed = 0f;
    }

    #endregion

    #region Stamina

    private void HandleStamina()
    {
        if (IsRunning)
        {
            Stamina = Mathf.Max(Stamina - staminaDrainRate * Time.deltaTime, 0f);
            staminaRechargeTimer = staminaRechargeDelay;
        }
        else
        {
            if (staminaRechargeTimer > 0f)
                staminaRechargeTimer -= Time.deltaTime;
            else
                Stamina = Mathf.Min(Stamina + staminaRechargeRate * Time.deltaTime, maxStamina);
        }
    }

    #endregion

    #region Footsteps

    private void HandleFootsteps()
    {
        if (s_audioManager == null) return;

        if (cc.isGrounded && (IsWalking || IsRunning))
        {
            if (!isPlayingSteps)
            {
                s_audioManager.Walking();
                isPlayingSteps = true;
            }
        }
        else
        {
            s_audioManager.StopWalking();
            isPlayingSteps = false;
        }
    }

    #endregion

    #region IDamageable / Health

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        s_audioManager?.PlayerHit();
        s_gameManager?.ShowHitFlash();

        Health = Mathf.Max(Health - amount, 0f);

        if (!IsAlive) Die();
    }

    public void RestoreHealth(float amount)  => Health  = Mathf.Min(Health  + amount, maxHealth);
    public void RestoreStamina(float amount) => Stamina = Mathf.Min(Stamina + amount, maxStamina);

    public void FullHeal()
    {
        Health  = maxHealth;
        Stamina = maxStamina;
    }

    #endregion

    #region Respawn

    public void Respawn(Vector3 position, Quaternion rotation)
    {
        FullHeal();
        cc.enabled         = false;
        transform.position = position;
        transform.rotation = rotation;
        cc.enabled         = true;
    }

    #endregion

    #region Death

    public event System.Action OnDeath;

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log("[PlayerController] Player died.");
    }

    #endregion

    #region Utilities

    public Vector3 Position => transform.position;

    #endregion
}