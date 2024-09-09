using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("GameManager is Null");
            }
            return instance;
        }
    }

    [SerializeField] private GameObject startMenuUI;  // The UI GameObject for the start menu
    [SerializeField] private GameObject weaponSelectScreen;  // Weapon select screen UI panel
    [SerializeField] private GameObject ClearWeaponsBackButton;
    [SerializeField] private GameObject InstructionsUI;
    [SerializeField] private GameObject GameOverUI;



    [SerializeField] private List<BaseWeapon> availableWeapons;  // List of available weapons
    [SerializeField] private TMP_Dropdown player1MainWeaponDropdown;
    [SerializeField] private TMP_Dropdown player1SubWeaponDropdown;
    [SerializeField] private TMP_Dropdown player2MainWeaponDropdown;
    [SerializeField] private TMP_Dropdown player2SubWeaponDropdown;
    [SerializeField] private Button player1ConfirmButton;
    [SerializeField] private Button player2ConfirmButton;

    private List<GameObject> activeProjectiles = new List<GameObject>();  // List to track active projectiles
    [SerializeField] private TurnManager turnManager;


    [SerializeField] private GameObject player1WeaponHeader;
    [SerializeField] private GameObject player2WeaponHeader;


    private GameObject player1MainWeapon;
    private GameObject player1SubWeapon;
    private GameObject player2MainWeapon;
    private GameObject player2SubWeapon;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    

    private bool isPaused = false;  // Track the current pause state
    [SerializeField] private GameObject pauseMenuUI;  // Optional: UI to show when the game is paused

    public bool gameHasStarted = false;

    private void Awake()
    {
        instance = this;  // Assign instance for singleton
    }

    void Start()
    {
        // Pause the game when the start menu is shown
        StartMenu();
        // Ensure the start screen is visible and weapon select screen is hidden
        startMenuUI.SetActive(true);
        weaponSelectScreen.SetActive(false);
        InstructionsUI.SetActive(false);

        // Populate dropdowns with weapon names
        PopulateWeaponDropdown(player1MainWeaponDropdown, availableWeapons);
        PopulateWeaponDropdown(player1SubWeaponDropdown, availableWeapons);
        PopulateWeaponDropdown(player2MainWeaponDropdown, availableWeapons);
        PopulateWeaponDropdown(player2SubWeaponDropdown, availableWeapons);

        // Set up button listeners
        player1ConfirmButton.onClick.AddListener(ConfirmPlayer1Selection);
        player2ConfirmButton.onClick.AddListener(ConfirmPlayer2Selection);

        gameHasStarted = false;
    }

    private void Update()
    {
        if (gameHasStarted)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseMenu();
                }
            }
        }
    }

    private void PauseMenu()
    {
        Time.timeScale = 0f;  // Freeze the game
        isPaused = true;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);  // Show the pause menu UI if it exists
        }

        Debug.Log("Game Paused");
    }

    // Populate the TMP_Dropdown with weapon options
    private void PopulateWeaponDropdown(TMP_Dropdown dropdown, List<BaseWeapon> weapons)
    {
        List<string> weaponNames = new List<string>();
        foreach (var weapon in weapons)
        {
            weaponNames.Add(weapon.weaponName);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(weaponNames);
    }

    public void AddProjectile(GameObject projectile)
    {
        // Add projectile to the active list
        activeProjectiles.Add(projectile);

        // Subscribe to the projectile's OnDestroy event
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.OnProjectileDestroyed += HandleProjectileDestroyed;
        }
    }

    private void HandleProjectileDestroyed(GameObject projectile)
    {
        // Remove projectile from the active list
        activeProjectiles.Remove(projectile);

        // Check if all projectiles are destroyed
        if (activeProjectiles.Count == 0)
        {
            // Reset the movement timer
            Debug.Log("Reset Timer");
            turnManager.ResetMovementTimer();
        }
    }

    // Confirm selection for Player 1 and instantiate the selected weapons
    public void ConfirmPlayer1Selection()
    {
        BaseWeapon mainWeapon = availableWeapons[player1MainWeaponDropdown.value];
        BaseWeapon subWeapon = availableWeapons[player1SubWeaponDropdown.value];

        int player1WeaponCount = player1.GetComponent<WeaponController>().GetBaseWeaponsCount();
        Debug.Log("The player weapon counbbt " + player1WeaponCount);

        if (player1WeaponCount < 2)
        {
            // Instantiate and parent the weapons under Player 1's Weapon Header
            player1MainWeapon = Instantiate(mainWeapon.weaponPrefab, player1WeaponHeader.transform);
            player1SubWeapon = Instantiate(subWeapon.weaponPrefab, player1WeaponHeader.transform);

            player1MainWeapon.SetActive(false);
            player1SubWeapon.SetActive(false);

            player1.GetComponent<WeaponController>().setAvailaibleWeapons(player1MainWeapon.GetComponent<BaseWeapon>());
            player1.GetComponent<WeaponController>().setAvailaibleWeapons(player1SubWeapon.GetComponent<BaseWeapon>());
            player1.GetComponent<WeaponController>().StartGame();
        }
        Debug.Log("Player 1 selected main weapon: " + mainWeapon.weaponName + ", sub-weapon: " + subWeapon.weaponName);
    }

    // Confirm selection for Player 2 and instantiate the selected weapons
    public void ConfirmPlayer2Selection()
    {
        BaseWeapon mainWeapon = availableWeapons[player2MainWeaponDropdown.value];
        BaseWeapon subWeapon = availableWeapons[player2SubWeaponDropdown.value];

        int player2WeaponCount = player2.GetComponent<WeaponController>().GetBaseWeaponsCount();

        if (player2WeaponCount < 2)
        {
            // Instantiate and parent the weapons under Player 2's Weapon Header
            player2MainWeapon = Instantiate(mainWeapon.weaponPrefab, player2WeaponHeader.transform);
            player2SubWeapon = Instantiate(subWeapon.weaponPrefab, player2WeaponHeader.transform);

            player2MainWeapon.SetActive(false);
            player2SubWeapon.SetActive(false);

            player2.GetComponent<WeaponController>().setAvailaibleWeapons(player2MainWeapon.GetComponent<BaseWeapon>());
            player2.GetComponent<WeaponController>().setAvailaibleWeapons(player2SubWeapon.GetComponent<BaseWeapon>());
            player2.GetComponent<WeaponController>().StartGame();
        }
        Debug.Log("Player 2 selected main weapon: " + mainWeapon.weaponName + ", sub-weapon: " + subWeapon.weaponName);
    }

    // Method to transition from the start screen to the weapon select screen
    public void TransitionToWeaponSelectScreen()
    {
        // Hide the start screen and show the weapon select screen
        startMenuUI.SetActive(false);
        weaponSelectScreen.SetActive(true);

        Debug.Log("Transitioned to Weapon Select Screen");
    }


    // Pauses the game by setting time scale to 0
    private void StartMenu()
    {
        Time.timeScale = 0f;

        if (startMenuUI != null)
        {
            startMenuUI.SetActive(true);  // Show the start menu UI
            gameHasStarted = true;
        }
    }

    // Unpauses the game by setting time scale back to 1 and hiding the menu
    public void UnpauseGame()
    {

        int player1WeaponCount = player1.GetComponent<WeaponController>().GetBaseWeaponsCount();
        int player2WeaponCount = player2.GetComponent<WeaponController>().GetBaseWeaponsCount();

        Debug.Log("Player one's weapon " + player1WeaponCount);
        if (startMenuUI != null && (player1WeaponCount >= 2 && player2WeaponCount >= 2))
        {
            Time.timeScale = 1f;
            startMenuUI.SetActive(false);  // Hide the start menu UI
            weaponSelectScreen.SetActive(false);
            gameHasStarted = true;
            
        }
    }

    public void ClearCurrentWeaponsForAllPlayers()
    {
        // Clear weapons for Player 1
        ClearCurrentWeapons(player1WeaponHeader);
        player1.GetComponent<WeaponController>().ClearWeapons(); 

        // Clear weapons for Player 2
        ClearCurrentWeapons(player2WeaponHeader);
        player2.GetComponent<WeaponController>().ClearWeapons();

        Debug.Log("Weapons cleared for both Player 1 and Player 2.");
    }


    private void ClearCurrentWeapons(GameObject weaponHeader)
    {
        foreach (Transform child in weaponHeader.transform)
        {
            if (child.gameObject.tag == "Weapon") { Destroy(child.gameObject); }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void TurnOnInstructions()
    {
        startMenuUI.SetActive(false);
        InstructionsUI.SetActive(true);
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameOverUI.SetActive(false);

    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;  // Resume the game
        isPaused = false;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);  // Hide the pause menu UI if it exists
        }

        Debug.Log("Game Resumed");
    }
}


