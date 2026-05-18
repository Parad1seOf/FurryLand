using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerWeapon : MonoBehaviour
{
    

    [Header("References")]
    [SerializeField] private GunSystem gun;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera fpsCam;

    [Header("Spread")]
    public float walkSpread = 0.02f;
    public float runSpread = 0.05f;

    [Header("Recoil")]
    public CharacterController characterController;
    public float recoilForce = 25f;
    private Vector3 impactVelocity;

    private bool trigger;

    public void GrabMagazine(int amount)
    {
        gun.AddMagazines(amount);
    }

    public void Start()
    {
        fpsCam = Camera.main;
        audioManager = AudioManager.Instance;

        characterController = GetComponent<CharacterController>();
    }

    public void Update()
    {
        HandleInput();

        if (impactVelocity.magnitude > 0.2f)
        {
            characterController.Move(impactVelocity * Time.deltaTime);
            impactVelocity = Vector3.Lerp(impactVelocity, Vector3.zero, 5f * Time.deltaTime);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gun.TryReload();
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        trigger = gun.allowHoldToFire
            ? Input.GetKey(KeyCode.Mouse0)
            : Input.GetKeyDown(KeyCode.Mouse0);
        if (trigger) TryShoot();
        
    }

    private void TryShoot()
    {
        
        float spreadIncrease = 0;
        if (playerController != null && playerController.IsRunning) spreadIncrease = runSpread;
        else if (playerController != null && playerController.IsWalking) spreadIncrease = walkSpread;
        gun.IncreaseSpread(spreadIncrease);

        Vector3 direction = fpsCam.transform.forward;

        if (!gun.TryShoot(fpsCam.transform.position, direction)) return;
        audioManager?.Shooting();
        if (characterController != null)
        {
            if (!characterController.isGrounded)
            {
                impactVelocity += -direction.normalized * recoilForce;
            }
        }

        AlertSystem.Instance.TriggerAlert();
    }
}
