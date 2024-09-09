using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine;

public class SingleProjectileWeapon : BaseWeapon
{
    private GameObject currentProjectile;  // Track the original fired projectile
    public override void Fire(Transform launchPoint, Vector2 direction, float power)
    {
        // Check if the projectile prefab is assigned before firing
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab not assigned to the weapon!");
            return;
        }

        // Fire the initial projectile
        currentProjectile = Instantiate(projectilePrefab.gameObject, launchPoint.position, Quaternion.identity);

        // Check if the projectile has a Rigidbody2D or Rigidbody component to apply force
        Rigidbody2D rb2D = currentProjectile.GetComponent<Rigidbody2D>();
        Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();

        if (rb2D != null)
        {
            rb2D.AddForce(direction * power, ForceMode2D.Impulse);  // Use 2D physics
        }
        else if (rb != null)
        {
            rb.AddForce(direction * power, ForceMode.Impulse);      // Use 3D physics
        }
        else
        {
            Debug.LogError("Projectile prefab is missing a Rigidbody or Rigidbody2D component.");
            Destroy(currentProjectile); // Destroy the projectile if it can't be moved
            return;
        }
    }
}
