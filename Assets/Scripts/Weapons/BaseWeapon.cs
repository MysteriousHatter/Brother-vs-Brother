using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] protected float maxPowerMultiplier = 1.5f;
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected Color weaponColor = Color.white;
    public GameObject weaponPrefab;
    public string weaponName;

    // Method to fire the weapon (to be overridden by subclasses)
    public abstract void Fire(Transform launchPoint, Vector2 direction, float power);

    // Optional methods for shared behavior among all weapons
    public virtual float GetMaxWeaponPower()
    {
        return maxPowerMultiplier;
    }

    public virtual Projectile GetProjectilePrefab()
    {
        return projectilePrefab;
    }

    public virtual Color GetWeaponColor()
    {
        return weaponColor;
    }
}
