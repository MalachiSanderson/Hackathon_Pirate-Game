using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{

    private float aimInput;

    public bool isGunning;
    public bool canFire;
    //public double damagePerShot;
    public float reloadTime;
    public bool canChangeAngle;
    public float rotationSpeed;
    public float maxRotationAngle;
    public float cannonAngle;
    public GameObject cannonBallObject;
    public Transform cannonBallSpawnLoc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isGunning)
        {
            changeAngleInputs();
            shootInput();
        }


    }

    void changeAngleInputs()
    {
        aimInput = Input.GetAxisRaw("Horizontal");
        if (canChangeAngle && aimInput != 0 && canFire)
        {

            
            if (aimInput > 0 && (transform.localEulerAngles.y < maxRotationAngle) || transform.localEulerAngles.y >= (360 - maxRotationAngle))
            {
                transform.Rotate(new Vector3(0, rotationSpeed * aimInput * Time.deltaTime, 0));
            }
            else if(aimInput > 0 && (transform.localEulerAngles.y >= maxRotationAngle && transform.localEulerAngles.y < (180) ))
            {
                transform.Rotate(new Vector3(0, (rotationSpeed * aimInput * Time.deltaTime) - 3, 0));
            }
            if(aimInput < 0 && (transform.localEulerAngles.y <= maxRotationAngle || transform.localEulerAngles.y > (360- maxRotationAngle)))
            {
                transform.Rotate(new Vector3(0, rotationSpeed * aimInput * Time.deltaTime, 0));

            }
            else if(aimInput < 0 && (transform.localEulerAngles.y > 0 && transform.localEulerAngles.y <= (360 - maxRotationAngle)))
            {
                transform.Rotate(new Vector3(0, (rotationSpeed * aimInput * Time.deltaTime) + 3, 0));
            }

            //cannonBallSpawnLoc.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        }
    }

    void shootInput()
    {
        if(canFire && Input.GetMouseButtonDown(0) )
        {
            
            fireCannonBall();


            canFire = false;
            StartCoroutine(cannonReloadTime());
        }
    }

    public IEnumerator cannonReloadTime()
    {
        yield return new WaitForSeconds(reloadTime);
        canFire = true;
    }

    public void fireCannonBall()
    {
       // print("FIRE CANNON BALL FROM: " + gameObject.name);
        Instantiate(cannonBallObject, cannonBallSpawnLoc.position, cannonBallSpawnLoc.transform.rotation);
        FindObjectOfType<PlayerCamera>().Shake(0.03f, 0.4f);
    }
}
