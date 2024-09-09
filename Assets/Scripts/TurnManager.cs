// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private enum Turn{Player1, Player2, Firing};
    [SerializeField] PlayerMovement player1;
    [SerializeField] PlayerMovement player2;
    [SerializeField] Image timerImage;
    [SerializeField] float movementTime = 10f;
    WeaponController player1Weapon;
    WeaponController player2Weapon;
    Turn turn = Turn.Player1;
    Turn lastTurn = Turn.Player1;
    bool timerRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get a hold of player weapons
        player1Weapon = player1.GetComponent<WeaponController>();
        player2Weapon = player2.GetComponent<WeaponController>();

        //Player 1 goes first
        player1.ToggleControl(true);
        player1Weapon.ToggleFiring(true);
        player2.ToggleControl(false);
        player2Weapon.ToggleFiring(false);

        //Subscribe to events
        WeaponController.Fired += Firing;
        Projectile.Impact += NextTurn;
    }

    private void FixedUpdate() 
    {
        if (GameManager.Instance.gameHasStarted) { UpdatedControls(); }    
    }

    public void NextTurn()
    {
        //Check who had the last turn and make it the next persons turn.
        Debug.Log("Next Turn");
        if(lastTurn == Turn.Player1)
        {
            Debug.Log("Player 2's turn");
            turn = Turn.Player2;
        }
        else if(lastTurn == Turn.Player2)
        {
            Debug.Log("Player 1's turn");
            turn = Turn.Player1;
        }
    }

    public void Firing()
    {
        //Between turns is the firing phase during which the projectile is flying.
        lastTurn = turn;
        ResetMovementTimer();
        turn = Turn.Firing;
    }


    private void UpdatedControls()
    {
        //Enable the controls for whoever's turn it is. If it's the firing phase, all controls are disabled.
        if(turn == Turn.Player1)
        {
            if(!timerRunning)
            {
                //StopCoroutine("MovementTimer");
                player1.ToggleControl(true);
                player1Weapon.ToggleFiring(true);
                player2.ToggleControl(false);
                player2Weapon.ToggleFiring(false);
                StartCoroutine("MovementTimer", player1);
            }
        }
        else if(turn == Turn.Player2)
        {
            if(!timerRunning)
            {
                //StopCoroutine("MovementTimer");
                player1.ToggleControl(false);
                player1Weapon.ToggleFiring(false);
                player2.ToggleControl(true);
                player2Weapon.ToggleFiring(true);
                StartCoroutine("MovementTimer", player2);
            }
        }
        else
        {
            //Firing Phase 
            player1.ToggleControl(false);
            player1Weapon.ToggleFiring(false);
            player2.ToggleControl(false);
            player2Weapon.ToggleFiring(false);
            timerRunning = false;
        }
    }

    public void ResetMovementTimer()
    {
        Debug.Log("Timer reset due to projectile hit");

        // Stop the current timer and restart it
        StopCoroutine("MovementTimer");

        // Optionally, reset the fillAmount of the timer UI
        timerImage.fillAmount = 0f;

    }


    IEnumerator MovementTimer(PlayerMovement player)
    {
        // CHALLENGE, add a timer here for limitted amount of movement time. 
        // HINT - Turn off the movement controls when the timer finishes to end the movement phase of the turn.
        float elaspedTime = 0f;

        timerImage.fillAmount = 0f;
        timerRunning = true;

        while(elaspedTime < movementTime)
        {
            Debug.Log("Current elapsed time " + elaspedTime);
            elaspedTime += Time.deltaTime;

            timerImage.fillAmount = Mathf.Clamp01(elaspedTime / movementTime);

            yield return null;
        }
        Debug.Log("Did we reach this part");
        //timerRunning = false;
        timerImage.fillAmount = 1f;

        EndTurn();

    }

    private void EndTurn()
    {
        Firing();
        NextTurn();
        timerRunning = false;
        UpdatedControls();

    }
}
