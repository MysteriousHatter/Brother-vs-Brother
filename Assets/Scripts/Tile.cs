// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] bool isDestroyable;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        // Some tiles should be permanent to create obstacles.
        if(!isDestroyable) {return;}

        // CHALLENGE - add logic to destroy this tile when a projectile hits.
        if(other.gameObject.layer == 6)
        {
            Destroy(this.gameObject);
        }
        
    }
}
