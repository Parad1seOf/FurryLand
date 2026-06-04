using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeleeAttack : MonoBehaviour
{
    [Header("Attack parameters")]
    [SerializeField] float damage = 100f;
    [SerializeField] float range = 2f;
    [SerializeField] float radius = 1.5f;
    [SerializeField] float coolDown = 1f;
    [SerializeField] LayerMask enemyLayer;
    private float nextAttackTime = 0f;

    [Header("Visual attack")]
    [SerializeField] GameObject attackObject;
    [SerializeField] Animation attackAnimation;
    private string clipName = "Swipe";
    [SerializeField] GameObject slashSprite;
    [SerializeField] float slashDuration = 0.2f;

    private void Start()
    {
       if(attackObject != null) attackObject.SetActive(false);
       if(slashSprite != null) slashSprite.SetActive(false);
    }

    void Update()
    {
      if(Input.GetMouseButtonDown(1) && Time.time > nextAttackTime)
      {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        
        Attack();
        nextAttackTime = Time.time + coolDown;
      }
    }

    private void Attack()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.MeleeSwipe();
        if (ScoreManager.instance != null) ScoreManager.instance.RegisterShot();

        StopAllCoroutines();
        StartCoroutine(ShowAndHideArm());

        Vector3 attackPoint = transform.position + transform.forward * range;
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint, radius, enemyLayer);

        bool hasHitEnemy = false;
        System.Collections.Generic.List<IDamageable> targetsDamaged = new System.Collections.Generic.List<IDamageable>();

        foreach (Collider enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                if (targetsDamaged.Contains(damageable)) continue;

                damageable.TakeDamage(damage);
                targetsDamaged.Add(damageable);
                hasHitEnemy = true;
            }
        }

        if (hasHitEnemy)
        {
            StartCoroutine(ShowAndHideSlash());
            //if (ScoreManager.instance != null) ScoreManager.instance.RegisterHit();
        }
    }

    private IEnumerator ShowAndHideArm()
    {
       if(attackObject == null || attackAnimation == null) yield break;

       attackObject.SetActive(true);
       attackAnimation.Play(clipName);

       yield return new WaitForSeconds(attackAnimation[clipName].length);

       if(attackObject != null) attackObject.SetActive(false);
    }

    private IEnumerator ShowAndHideSlash()
    {
        if (slashSprite == null) yield break;

        slashSprite.SetActive(true);
        yield return new WaitForSeconds(slashDuration);
        slashSprite.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 attackPoint = transform.position + transform .forward * range;
        Gizmos.DrawWireSphere(attackPoint, radius);
    }
}
