using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFire : MonoBehaviour
{
    private bool canFire;

    private void Start()
    {
        canFire = true;
    }

    public IEnumerator cannonReloadTime()
    {
        yield return new WaitForSeconds(GetComponent<CannonController>().reloadTime);
        canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(canFire)
        {
            this.GetComponent<CannonController>().fireCannonBall();

            canFire = false;
            StartCoroutine(cannonReloadTime());
            
        }
    }
}
