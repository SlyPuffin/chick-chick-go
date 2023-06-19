using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuteCircle : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        GuardMovement gMovement = collision.gameObject.GetComponent<GuardMovement>();
        if (gMovement != null)
        {
            gMovement.changeStatus("ENAMORED");
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        GuardMovement gMovement = collision.gameObject.GetComponent<GuardMovement>();
        if (gMovement != null)
        {
            gMovement.changeStatus("PATROL");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
