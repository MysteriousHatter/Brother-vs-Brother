using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
