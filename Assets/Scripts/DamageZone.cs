using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damage;
    public bool canConstantlyDamage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ShipStats>() != null)
        {
            //print("Hit a ship");
            other.GetComponent<ShipStats>().dealHullDamage(damage);
            if (this.CompareTag("Cannonball") )
            {
                //this.GetComponent<MeshCollider>().enabled = false;
                //GetComponent<CannonBall>().speed = 0;
                //Destroy(this.gameObject);
                
            }
            
            //this.GetComponent<SphereCollider>().enabled = false;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<ShipStats>() != null && canConstantlyDamage)
        {
            //print("Hit a ship");
            other.GetComponent<ShipStats>().dealHullDamage(damage);
            
        }


    }


}
