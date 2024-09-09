// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    public static event System.Action Fired;

    [SerializeField] private List<BaseWeapon> availableWeapons = new List<BaseWeapon>();  // List of all weapons
    [SerializeField] private Transform launchPoint;
    [SerializeField] private GameObject aimingTarget;
    [SerializeField] private GameObject weaponPowerCanvas;
    [SerializeField] private Image weaponPowerIndicator;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;  // Weapon sprite renderer
    [SerializeField] private float weaponSpeed = 100f;
    [SerializeField] private float chargeRate = 0.02f;
    [SerializeField] private GameObject weaponHolder;  // The transform of the weapon holder for rotation

    [SerializeField] private BaseWeapon equippedWeapon;   // Currently equipped weapon
    private int currentWeaponIndex = 0; // Track which weapon is selected
    private float weaponCharge;
    private float maxWeaponPower = 1.5f;
    private bool firingEnabled = false;
    private float weaponInput;  // Input for weapon control (rotation, etc.)

    public void StartGame()
    {
        // Equip the first weapon in the list
        if (availableWeapons.Count > 0)
        {
            EquipWeapon(0);
        }
        else
        {
            Debug.LogError("No weapons found under the weaponHolder parent!");
        }
    }

    private void Update()
    {

        if (!firingEnabled) return;

        // Handle weapon switching
        if (Input.GetKeyDown(KeyCode.Q)) SwitchWeapon(-1); // Switch to previous weapon
        if (Input.GetKeyDown(KeyCode.E)) SwitchWeapon(1);  // Switch to next weapon

        HandleWeaponInput();  // Handle the aiming or movement of the weapon

        if (Input.GetButton("Jump"))
        {
            ChargeFire();  // Charge the weapon
        }

        if (Input.GetButtonUp("Jump"))
        {
            FireWeapon();  // Fire the weapon when the button is released
        }
    }

    private void HandleWeaponInput()
    {
        // Read the vertical input (for aiming up or down)
        weaponInput = Input.GetAxis("Vertical");

        // Rotate the weapon holder based on the weapon input
        if (weaponHolder != null)
        {
            weaponHolder.transform.Rotate(0, 0, weaponInput * Time.deltaTime * 50f); // Example rotation speed
        }
    }

    private void ChargeFire()
    {
        // Show weapon charge meter and fill it up
        weaponPowerCanvas.SetActive(true);
        weaponPowerIndicator.fillAmount = weaponCharge / equippedWeapon.GetMaxWeaponPower();

        // Charge the weapon over time until it reaches the maximum power
        if (weaponCharge < equippedWeapon.GetMaxWeaponPower())
        {
            weaponCharge += chargeRate * Time.deltaTime;
        }
        else
        {
            weaponCharge = equippedWeapon.GetMaxWeaponPower();
        }
    }

    private void FireWeapon()
    {
        if (equippedWeapon == null) return;

        Fired?.Invoke();

        // Fire the equipped weapon
        Vector2 direction = aimingTarget.transform.position - launchPoint.position;
        float powerUsed = weaponCharge / equippedWeapon.GetMaxWeaponPower();
        float totalPower = (weaponSpeed) * powerUsed;

        equippedWeapon.Fire(launchPoint, direction, totalPower);

        // Reset weapon charge and disable firing until the next turn
        weaponCharge = 0f;
        firingEnabled = false;

        // Hide weapon charge meter after firing
        weaponPowerCanvas.SetActive(false);
    }

    private void SwitchWeapon(int direction)
    {
        // Deactivate the currently equipped weapon
        if (equippedWeapon != null)
        {
            equippedWeapon.gameObject.SetActive(false);
        }

        currentWeaponIndex += direction;

        if (currentWeaponIndex < 0) currentWeaponIndex = availableWeapons.Count - 1;
        if (currentWeaponIndex >= availableWeapons.Count) currentWeaponIndex = 0;

        EquipWeapon(currentWeaponIndex);
    }

    private void EquipWeapon(int index)
    {
        equippedWeapon = availableWeapons[index];
        equippedWeapon.gameObject.SetActive(true);  // Activate the equipped weapon
        
        Debug.Log("Equipped Weapon: " + equippedWeapon.name);

        // Update the weapon sprite based on the equipped weapon (if any weapon has a unique sprite)
        if (weaponSpriteRenderer != null)
        {
            weaponSpriteRenderer.color = equippedWeapon.GetWeaponColor();  // Example: change weapon color
        }
    }

    public void setAvailaibleWeapons(BaseWeapon baseWeapon)
    {
        availableWeapons.Add(baseWeapon);

    }

    public int GetBaseWeaponsCount()
    {
        return availableWeapons.Count;
    }

    public void ClearWeapons()
    {
       availableWeapons.Clear();
    }    

    public void ToggleFiring(bool canFire)
    {
        firingEnabled = canFire;
    }
}
