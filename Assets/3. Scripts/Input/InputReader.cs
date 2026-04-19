using UnityEngine;
using System;

public class InputReader : MonoBehaviour
{
    [Header("Keybindings")]
    public KeyCode keyLeft     = KeyCode.A;
    public KeyCode keyRight    = KeyCode.D;
    public KeyCode keyUp       = KeyCode.W;
    public KeyCode keyDown     = KeyCode.S;
    public KeyCode keyRun      = KeyCode.LeftShift;
    public KeyCode keyJump     = KeyCode.Space;
    public KeyCode keyReload   = KeyCode.R;
    public KeyCode keyInteract = KeyCode.E;
    public KeyCode keyWeapon   = KeyCode.T;

    public Vector2 MoveInput       { get; private set; }
    public Vector2 LookInput       { get; private set; }
    public bool    WantsToRun      { get; private set; }
    public bool    JumpPressed     { get; private set; }
    public bool    FireHeld        { get; private set; }
    public bool    FirePressed     { get; private set; }
    public bool    ReloadPressed   { get; private set; }
    public bool    InteractPressed { get; private set; }
    public bool    InteractHeld    { get; private set; }
    public bool    WeaponPressed   { get; private set; }

    public event Action OnJump;
    public event Action OnReload;
    public event Action OnFirePressed;
    public event Action OnInteract;

    private void Update()
    {
        float h = 0f, v = 0f;
        if (Input.GetKey(keyRight)) h += 1f;
        if (Input.GetKey(keyLeft))  h -= 1f;
        if (Input.GetKey(keyUp))    v += 1f;
        if (Input.GetKey(keyDown))  v -= 1f;
        MoveInput = new Vector2(h, v);

        LookInput  = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        WantsToRun = Input.GetKey(keyRun);

        JumpPressed     = Input.GetKeyDown(keyJump);
        FireHeld        = Input.GetKey(KeyCode.Mouse0);
        FirePressed     = Input.GetKeyDown(KeyCode.Mouse0);
        ReloadPressed   = Input.GetKeyDown(keyReload);
        InteractPressed = Input.GetKeyDown(keyInteract);
        InteractHeld    = Input.GetKey(keyInteract);
        WeaponPressed   = Input.GetKeyDown(keyWeapon);

        if (JumpPressed)     OnJump?.Invoke();
        if (ReloadPressed)   OnReload?.Invoke();
        if (FirePressed)     OnFirePressed?.Invoke();
        if (InteractPressed) OnInteract?.Invoke();
    }
}