using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

using UnityEngine;

public class MultiProjectileWeapon : BaseWeapon
{
    [SerializeField] private int scatterCount = 3;        // Number of projectiles to scatter
    [SerializeField] private float scatterAngle = 15f;    // Angle between scattered projectiles

    private GameObject currentProjectile;  // Track the original fired projectile
    private bool canDuplicate = false;     // Flag to check if the player can trigger duplication

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
            Debug.Log("SHoot");
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

        canDuplicate = true; // Allow duplication after firing
    }

    private void Update()
    {
        // Check if the player presses the key to trigger duplication
        if (canDuplicate && currentProjectile != null && Input.GetButtonDown("Jump"))
        {
            DuplicateProjectile();
        }
    }

    private void DuplicateProjectile()
    {
        if (currentProjectile == null) return;

        // Get the current velocity and position of the original projectile
        Rigidbody2D rb2D = currentProjectile.GetComponent<Rigidbody2D>();
        Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();

        Vector2 currentVelocity;
        if (rb2D != null)
        {
            currentVelocity = rb2D.velocity;  // Get velocity from Rigidbody2D
        }
        else if (rb != null)
        {
            currentVelocity = rb.velocity;   // Convert 3D velocity to 2D if using Rigidbody
        }
        else
        {
            currentVelocity = Vector2.zero;
        }

        Vector2 direction = currentVelocity.normalized;  // Use the direction of the projectile
        Vector3 projectilePosition = currentProjectile.transform.position;

        // Total angle spread for the projectiles
        float totalSpreadAngle = scatterAngle;
        float halfSpread = totalSpreadAngle / 2f;

        // Split into multiple projectiles in a fan-like manner
        for (int i = 0; i < scatterCount; i++)
        {
            // Calculate the angle for each projectile based on its position in the spread
            float angleOffset = Mathf.Lerp(-halfSpread, halfSpread, (float)i / (scatterCount - 1));
            Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);  // Apply rotation around the Z-axis
            Vector2 splitDirection = rotation * direction;  // Rotate the original direction

            // Instantiate the split projectiles
            GameObject splitProjectile = Instantiate(projectilePrefab.gameObject, projectilePosition, Quaternion.identity);
            Rigidbody2D splitRb2D = splitProjectile.GetComponent<Rigidbody2D>();
            Rigidbody splitRb = splitProjectile.GetComponent<Rigidbody>();

            if (splitRb2D != null)
            {
                splitRb2D.velocity = splitDirection * currentVelocity.magnitude;  // Maintain original speed with new direction
            }
            else if (splitRb != null)
            {
                splitRb.velocity = new Vector3(splitDirection.x, splitDirection.y, 0) * currentVelocity.magnitude;  // Use 3D physics
            }
        }

        // Destroy the original projectile after splitting
        Destroy(currentProjectile);
        currentProjectile = null;
        canDuplicate = false; // Prevent further splitting
    }

    void IgnoreProjectileCollisions(GameObject originalProjectile, GameObject newProjectile)
    {
        Collider2D originalCollider = originalProjectile.GetComponent<Collider2D>();
        Collider2D newCollider = newProjectile.GetComponent<Collider2D>();

        if (originalCollider != null && newCollider != null)
        {
            Physics2D.IgnoreCollision(originalCollider, newCollider);
        }
    }

}
