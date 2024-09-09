// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] KeyCode jumpButton;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float speed = 400f;
    [SerializeField] float jumpForce = 350f;
    [SerializeField] float groundDistance = 1.2f;
    Rigidbody2D playerBody;
    float horizontalInput;
    bool isControllerEnabled = true;
    bool isGrounded = false;

    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime;
        IsGrounded();
        if(Input.GetKeyDown(jumpButton))
        {
            Jump();
        }
    }

    private void FixedUpdate() 
    {
        // Only one player can move at a time.
        if(!isControllerEnabled){return;}

        playerBody.velocity = new Vector2(horizontalInput, playerBody.velocity.y);

    }

    private void Jump()
    {
        // CHALLENGE, give the player some lift when they jump, but make sure they are on the ground first. The jump key is serialized, try changing it in the inspector.
        if (isControllerEnabled)
        {
            if (isGrounded && Input.GetKeyDown(jumpButton)) 
            {
                playerBody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }
        }

    }

    private void IsGrounded()
    {
        //Check if the player is on the ground.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundLayer);

        if(hit)
        {
            isGrounded = true;
            //Debug.Log( name + " is Grounded");
        }
        else
        {
            isGrounded = false;
        }
    }

    public void ToggleControl(bool controlEnabled)
    {
        // Have controls toggle on and off depending on whose turn it is.
        isControllerEnabled = controlEnabled;
    }

    public bool GetControlsEnabled()
    {
        return isControllerEnabled;
    }
    
}
