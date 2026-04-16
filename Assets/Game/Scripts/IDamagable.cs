// Interfaz comun de daño. GunSystem la usa para impactar a Enemy y PlayerController
// sin necesidad de conocer sus tipos concretos.
public interface IDamageable
{
    void TakeDamage(float amount);
}