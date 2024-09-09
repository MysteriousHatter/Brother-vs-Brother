using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostWeapon : BaseWeapon
{
    [SerializeField] private float boostMultiplier = 3f;  // How much the speed increases during the boost

    private GameObject currentProjectile;  // Track the projectile currently in flight
    private bool canBoost = false;         // Flag to allow boosting
    private bool hasBoosted = false;       // Ensure the boost can only be triggered once
    private Rigidbody2D projectileRb2D;    // Reference to the Rigidbody2D component
    private Rigidbody projectileRb;        // Reference to the Rigidbody component (for 3D)

    public override void Fire(Transform launchPoint, Vector2 direction, float power)
    {
        // Check if the projectile prefab is assigned before firing
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab not assigned to the weapon!");
            return;
        }

        // Fire the projectile
        currentProjectile = Instantiate(projectilePrefab.gameObject, launchPoint.position, Quaternion.identity);

        // Check for Rigidbody2D (2D physics) or Rigidbody (3D physics)
        projectileRb2D = currentProjectile.GetComponent<Rigidbody2D>();
        projectileRb = currentProjectile.GetComponent<Rigidbody>();

        // Apply initial force to the projectile
        if (projectileRb2D != null)
        {
            projectileRb2D.AddForce(direction * power, ForceMode2D.Impulse);  // Use 2D physics
        }
        else if (projectileRb != null)
        {
            projectileRb.AddForce(direction * power, ForceMode.Impulse);      // Use 3D physics
        }
        else
        {
            Debug.LogError("Projectile prefab is missing a Rigidbody or Rigidbody2D component.");
            Destroy(currentProjectile);  // Destroy the projectile if it can't be moved
            return;
        }

        // Allow the boost to be activated
        canBoost = true;
        hasBoosted = false;  // Reset boost status for new projectile
    }

    private void Update()
    {
        // Check if the player presses the boost key and hasn't boosted yet
        if (canBoost && !hasBoosted && Input.GetButtonDown("Jump"))
        {
            Debug.Log("Boost projectile");
            BoostProjectile();
        }
    }

    private void BoostProjectile()
    {
        if (currentProjectile == null) return;

        // Apply the speed boost by increasing the velocity
        if (projectileRb2D != null)
        {
            projectileRb2D.velocity *= boostMultiplier;  // Multiply the current velocity by the boost factor (for 2D)
        }
        else if (projectileRb != null)
        {
            projectileRb.velocity *= boostMultiplier;    // Multiply the current velocity by the boost factor (for 3D)
        }

        // Start generating afterimages
        StartCoroutine(GenerateAfterimages());

        hasBoosted = true;  // Ensure the boost can only be used once
    }

    private IEnumerator GenerateAfterimages()
    {
        while (currentProjectile != null && hasBoosted)
        {
            // Create an afterimage at the current position of the projectile
            GameObject afterimage = new GameObject("Afterimage");
            SpriteRenderer sr = afterimage.AddComponent<SpriteRenderer>();
            SpriteRenderer originalSprite = currentProjectile.GetComponent<SpriteRenderer>();

            if (originalSprite != null)
            {
                sr.sprite = originalSprite.sprite;  // Use the same sprite as the projectile
                sr.color = new Color(1, 1, 0, 0.5f);  // Semi-transparent yellow color
            }

            afterimage.transform.position = currentProjectile.transform.position;
            afterimage.transform.rotation = currentProjectile.transform.rotation;

            // Fade out and destroy the afterimage over time
            StartCoroutine(FadeAndDestroyAfterimage(sr));

            // Wait before generating the next afterimage
            yield return new WaitForSeconds(0.05f);  // Adjust interval for afterimage spawn rate
        }
    }

    private IEnumerator FadeAndDestroyAfterimage(SpriteRenderer sr)
    {
        float duration = 0.5f;  // Duration for afterimage to fade
        float elapsedTime = 0f;

        Color initialColor = sr.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(initialColor.a, 0, elapsedTime / duration);  // Gradually fade out
            sr.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            yield return null;
        }

        Destroy(sr.gameObject);  // Destroy after fading
    }
}
