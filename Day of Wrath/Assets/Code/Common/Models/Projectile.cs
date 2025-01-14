using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private float damage;
    private Vector3 direction;
    private bool hasHit = false;

    public void Initialize(Vector3 targetPosition, float speed, float damage)
    {
        this.speed = speed;
        this.damage = damage;

        // Calculate and lock the direction
        direction = (targetPosition - transform.position).normalized;

        // Set the initial forward direction
        transform.forward = direction;
    }

    private void Update()
    {
        if (hasHit) return;

        // Move the projectile in the locked direction
        var step = speed * Time.deltaTime;
        transform.position += direction * step;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.TryGetComponent<UnitBase>(out var unit))
        {
            // Hit a unit, apply damage
            unit.TakeDamage(damage);
            hasHit = true;
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            // Hit the ground, destroy the projectile
            hasHit = true;
            Destroy(gameObject);
        }
    }
}
