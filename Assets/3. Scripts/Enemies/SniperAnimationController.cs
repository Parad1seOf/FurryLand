using UnityEngine;

public class SniperAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void SetAiming(bool isAiming)
    {
        if (animator == null) return;
        animator.SetBool("IsAiming", isAiming);
    }

    public void Shoot()
    {
        if (animator == null) return;
        animator.SetTrigger("Shoot");
    }
}