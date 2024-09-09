// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wind : MonoBehaviour
{
    [SerializeField] float windMultiplier = 1.0f;
    float windFactor = 0f;
    [SerializeField] Image windPositive;
    [SerializeField] Image windNegative;

    // Start is called before the first frame update
    void Start()
    {
        Projectile.Impact += UpdateWind;
        UpdateWind(); 
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            collision.GetComponent<Rigidbody2D>().AddForce(Vector2.right * windFactor * windMultiplier);
        }
    }

    private void UpdateWind()
    {
        windFactor = Random.Range(-1.0f,1.0f);
        // CHALLENGE - Apply a force to the projectile in some way.

        
        if(windFactor > 0)
        {
            windPositive.fillAmount = windFactor;
            windNegative.fillAmount = 0;
        }
        else
        {
            windPositive.fillAmount = 0;
            windNegative.fillAmount = -windFactor;
        }
    }

}
