using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] PlayerHealth player;
    [SerializeField] Image healthBar;
    float startingHealth;

    void Start()
    {
        startingHealth = player.GetHealthPoints();
    }

    void Update()
    {
        healthBar.fillAmount = player.GetHealthPoints()/startingHealth;
    }
}
