// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int healthPoints = 100;

    private void CheckHealth()
    {
        if(healthPoints <= 0)
        {
            // If player has no health, trigger the Game Over UI and freeze the game controls.
            FindObjectOfType<UI>().GameOver();
            Time.timeScale = 0f;
        }
    }

    public void TakeDamage(int damage)
    {
        healthPoints = healthPoints - damage;
        CheckHealth();
    }

    public int GetHealthPoints()
    {
        return healthPoints;
    }
}
