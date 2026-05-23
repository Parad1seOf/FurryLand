// PARCHE: al morir el conejo, clona su modelo y lo deja en ragdoll en la escena
// durante unos segundos. El conejo original sigue su flujo normal de pool/respawn
// gestionado por EnemyStateMachine + EnemyDeadState (no se toca nada de eso).
//
// Cómo funciona:
//   1. Nos suscribimos a HealthSystem.OnDeath en Awake (antes que EnemyStateMachine,
//      que se suscribe en Start). Así nuestro handler corre PRIMERO.
//   2. Al morir, clonamos el GameObject del modelo (Armature + SkinnedMesh + RagdollController).
//      Unity al hacer Instantiate copia las transforms actuales de los huesos,
//      por lo que el ragdoll empieza en la POSE EN LA QUE ESTABA EL CONEJO AL MORIR.
//   3. Desligamos el clon del padre, activamos ragdoll y programamos su destrucción.
//   4. El conejo original sigue: EnemyDeadState lo desactiva, vuelve al pool, todo igual.
//
// Configuración en Unity:
//   - Añadir este script al GameObject RAIZ del conejo (donde están HealthSystem,
//     EnemyStateMachine, NavMeshAgent, etc.).
//   - Asignar "modelRoot" al GameObject del modelo (el que contiene Armature,
//     SkinnedMeshRenderer y el componente RagdollController ya configurado).
//   - El RagdollController debe estar dentro de modelRoot (o el propio modelRoot),
//     porque Instantiate solo copia lo que cuelgue de ese transform.

using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class RabbitRagdollDeath : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("GameObject del modelo del conejo. Debe contener Armature + SkinnedMeshRenderer + RagdollController.")]
    [SerializeField] private GameObject modelRoot;

    [Header("Configuración")]
    [Tooltip("Segundos que el ragdoll permanece en la escena antes de destruirse.")]
    [SerializeField] private float lifetime = 8f;

    [Tooltip("Si está activo, al clonar se eliminan NavMeshAgent y otros componentes que pueden interferir con la física.")]
    [SerializeField] private bool sanitizeClone = true;

    private HealthSystem health;
    private bool subscribed = false;

    private void Awake()
    {
        // Awake corre antes que Start, así nos suscribimos antes que EnemyStateMachine
        // y nuestro handler se ejecuta primero en la cola de OnDeath.
        health = GetComponent<HealthSystem>();
        if (health != null)
        {
            health.OnDeath += HandleDeath;
            subscribed = true;
        }
    }

    private void OnDestroy()
    {
        if (subscribed && health != null)
        {
            health.OnDeath -= HandleDeath;
            subscribed = false;
        }
    }

    private void HandleDeath()
    {
        if (modelRoot == null)
        {
            Debug.LogWarning($"[RabbitRagdollDeath] '{name}' no tiene asignado modelRoot. No se generará ragdoll.");
            return;
        }

        // 1. Clonar el modelo en su pose actual.
        //    Instantiate copia las transforms vivas de los huesos en este frame,
        //    así que el ragdoll arranca exactamente donde estaba el conejo al morir.
        GameObject ragdollClone = Instantiate(
            modelRoot,
            modelRoot.transform.position,
            modelRoot.transform.rotation
        );

        // 2. Desparentar por si acaso (que el clon sea independiente del conejo original).
        ragdollClone.transform.SetParent(null, true);
        ragdollClone.name = modelRoot.name + "_Ragdoll";

        // 3. Limpiar el clon de componentes que estorban a la física.
        if (sanitizeClone)
        {
            // Desactivar NavMeshAgent si lo hubiera (a veces se cuela en el modelo).
            UnityEngine.AI.NavMeshAgent agent = ragdollClone.GetComponentInChildren<UnityEngine.AI.NavMeshAgent>(true);
            if (agent != null) agent.enabled = false;

            // Apagar el Animator del clon (luego SetRagdoll(true) también lo hace, pero por seguridad).
            Animator anim = ragdollClone.GetComponentInChildren<Animator>(true);
            if (anim != null) anim.enabled = false;
        }

        // 4. Activar el ragdoll del clon.
        RagdollController rc = ragdollClone.GetComponent<RagdollController>();
        if (rc == null) rc = ragdollClone.GetComponentInChildren<RagdollController>(true);
        if (rc != null)
        {
            rc.SetRagdoll(true);
        }
        else
        {
            Debug.LogWarning($"[RabbitRagdollDeath] El clon '{ragdollClone.name}' no tiene RagdollController. " +
                             "Añádelo al modelRoot del conejo y configura los huesos con el Ragdoll Wizard.");
        }

        // 5. Programar destrucción del clon.
        if (lifetime > 0f) Destroy(ragdollClone, lifetime);
    }
}