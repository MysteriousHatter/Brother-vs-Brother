// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactFX : MonoBehaviour
{
    //Have the impact FXs persist after the projectile is destroyed. Then destroy the FX themselves once they are complete.
    [SerializeField] float destroyInterval = 1f;

    void Start()
    {
        StartCoroutine("DestroyAfterInterval");
    }

    IEnumerator DestroyAfterInterval()
    {
        yield return new WaitForSeconds(destroyInterval);
        Destroy(gameObject);
    }
}
