// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] ParticleSystem impactFX;
    public static event Action Impact;
    CinemachineTargetGroup cinemachineTargetGroup;

    private void Start() 
    {
        //Find the cinemachine target group
        cinemachineTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
        // Add to Camera Target Group
        cinemachineTargetGroup.AddMember(this.transform, 2, 5);
        GameManager.Instance.AddProjectile(this.gameObject);
        
    }


    private void OnTriggerEnter2D(Collider2D other) 
    {
        // Exclude Impact with the wind collider
        if(other.tag == "Wind") {return;}
        else if(other.tag == "Projectile") { return; }

        //Check for collision with the player or opponent.
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if(health != null)
        {
            health.TakeDamage(damage);    
        }

        //Trigger impact event for the turn system.
        Impact.Invoke();

        //Destroy the projectile.
        Instantiate(impactFX, transform.position, Quaternion.identity);
        cinemachineTargetGroup.RemoveMember(this.transform);
        Destroy(gameObject);
    }

    public static void TriggerImpact()
    {
        Impact.Invoke();
    }

    public event Action<GameObject> OnProjectileDestroyed;  // Event to notify when projectile is destroyed
    private void OnDestroy()
    {
        // Notify the GameManager that the projectile was destroyed
        if (OnProjectileDestroyed != null)
        {
            OnProjectileDestroyed(gameObject);
        }
    }

}
