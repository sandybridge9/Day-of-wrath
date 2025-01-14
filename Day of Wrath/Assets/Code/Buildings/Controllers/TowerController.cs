using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerController : MonoBehaviour
{
    [Header("Tower Settings")]
    public float attackRange = 10f;
    public float attackRate = 1f;
    public float projectileSpeed = 10f;
    public float projectileDamage = 20f;

    public GameObject projectilePrefab;
    public Transform shootPoint;

    private BuildingBase thisBuilding;

    private List<UnitBase> enemiesInRange = new List<UnitBase>();

    private void Start()
    {
        thisBuilding = GetComponent<BuildingBase>();

        StartCoroutine(AttackRoutine());
    }

    private void Update()
    {
        DetectEnemies();
    }

    private void DetectEnemies()
    {
        enemiesInRange.Clear();
        var colliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<UnitBase>(out var unit)
                && thisBuilding.IsFromDifferentTeam(unit.Team))
            {
                enemiesInRange.Add(unit);
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (enemiesInRange.Count > 0)
            {
                AttackNearestEnemy();
            }

            yield return new WaitForSeconds(attackRate);
        }
    }

    private void AttackNearestEnemy()
    {
        if (enemiesInRange.Count == 0) return;

        var nearestEnemy = GetNearestEnemy();

        if (nearestEnemy != null && projectilePrefab != null && shootPoint != null)
        {
            // Get the enemy's head or chest position
            Vector3 targetPosition = GetTargetPosition(nearestEnemy);

            // Instantiate and initialize the projectile
            var projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            var projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(targetPosition, projectileSpeed, projectileDamage);
            }
        }
    }

    private UnitBase GetNearestEnemy()
    {
        UnitBase nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private Vector3 GetTargetPosition(UnitBase unit)
    {
        var enemyCollider = unit.GetComponent<Collider>();
        if (enemyCollider != null)
        {
            // Aim at the center of the enemy collider (chest or head)
            return enemyCollider.bounds.center;
        }

        // Fallback to the unit's transform position
        return unit.transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
