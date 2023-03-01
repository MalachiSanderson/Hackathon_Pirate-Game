using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    //private Vector3 boatVelocity;
    private Rigidbody boatBody;
    private Transform playerLocation;
    private float xMoveInput;
    private float zMoveInput;

    //private Rigidbody playerBody;

    public bool isConstantlyMoving;
    public bool isPilotingBoat;
    public bool dragsPlayerAlong;
    public float translateControlledVelocity;
    private float boatMaxSpeed; //NA
    public float constantVelocity;
    public bool usesRigidbody;
    public float boatForceApplied;
    
    //public float boatDecceleration;
    //public float walkAcceleration;
    //public float walkDecceleration;

    // Start is called before the first frame update
    void Start()
    {

        if(GetComponent<Rigidbody>() != null && usesRigidbody)
        {
            usesRigidbody = true;
            print("Using Rigidbody to control the boat: " + gameObject.name);
            boatBody = GetComponent<Rigidbody>();
        }
        else if(usesRigidbody)
        {
            print("[Error] Trying to use rigidbody without an attached Rigidbody compnent.");
            usesRigidbody = false;
        }

        playerLocation = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        xMoveInput = Input.GetAxisRaw("Horizontal");
        zMoveInput = Input.GetAxisRaw("Vertical");
        if(xMoveInput != 0 && zMoveInput != 0)
        {
           xMoveInput = (float)(xMoveInput * 0.5);
           zMoveInput = (float)(zMoveInput * 0.5);
        }
        if (isPilotingBoat)
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                isConstantlyMoving = !isConstantlyMoving;
            }

            if (!usesRigidbody)
            {
                /*
                if (xMoveInput != 0)
                {
                    transform.position = new Vector3(transform.position.x + (boatMaxSpeed * Time.deltaTime * xMoveInput), transform.position.y, transform.position.z);
                }

                if (zMoveInput != 0)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (boatMaxSpeed * Time.deltaTime * xMoveInput));

                }
                */
            }
            else
            {
                if (xMoveInput != 0)
                {

                    //Vector3 boatOgPos = new Vector3(boatBody.transform.position.x, boatBody.transform.position.y, boatBody.transform.position.z);
                    //print("Should move on xAxis" + xMoveInput);
                    boatBody.AddForce(new Vector3(boatForceApplied * xMoveInput, 0, 0), ForceMode.Impulse);
                    //boatVelocity.x = Mathf.MoveTowards(boatVelocity.x, boatMaxSpeed * xMoveInput, boatForceApplied * Time.deltaTime);

                    //playerBody.transform.position = new Vector3(playerBody.transform.position.x, playerBody.transform.position.y (), playerBody.transform.position.z ());
                }
                else if (xMoveInput == 0)
                {
                    //boatVelocity.x = Mathf.MoveTowards(boatVelocity.x, 0, boatDecceleration * Time.deltaTime);
                }

                if (zMoveInput != 0)
                {
                    boatBody.AddForce(new Vector3(0, 0, boatForceApplied * zMoveInput), ForceMode.Impulse);

                }

                //boatBody.velocity = new Vector3(boatVelocity.x, boatVelocity.y, boatVelocity.z);
            }
        }
        
    }
    private void FixedUpdate()
    {

        if (isConstantlyMoving)
        {
            if(usesRigidbody)
            {
                boatBody.velocity = new Vector3(constantVelocity, 0, 0);
            }
            else
            {
                //transform.position = new Vector3(transform.position.x + (constantVelocity * Time.deltaTime), transform.position.y, transform.position.z);
                //transform.Translate(new Vector3((constantVelocity * Time.deltaTime ), 0, 0));
                transform.Translate(transform.forward*constantVelocity*Time.deltaTime, Space.World);
                if (dragsPlayerAlong)
                {
                    playerLocation.Translate(new Vector3((constantVelocity * Time.deltaTime), 0, 0));

                }
                

            }

        }
        if (isPilotingBoat && !usesRigidbody)
        {
            if (!usesRigidbody)
            {
                if (xMoveInput != 0)
                {
                    //transform.position = new Vector3(transform.position.x + (translateControlledVelocity * Time.deltaTime * xMoveInput), transform.position.y, transform.position.z);
                    transform.Translate(new Vector3((translateControlledVelocity * Time.deltaTime * xMoveInput), 0, 0));
                    if (dragsPlayerAlong)
                    {
                        playerLocation.Translate(new Vector3((translateControlledVelocity * Time.deltaTime * xMoveInput), 0, 0));

                    }
                }

                if (zMoveInput != 0)
                {
                    //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (translateControlledVelocity * Time.deltaTime * zMoveInput));
                    transform.Translate(new Vector3(0, 0, (translateControlledVelocity * Time.deltaTime * zMoveInput)));
                    if (dragsPlayerAlong)
                    {
                        playerLocation.Translate(new Vector3(0, 0, (translateControlledVelocity * Time.deltaTime * zMoveInput)));

                    }
                }
            }
        }
        if (usesRigidbody)
        {

            if (boatBody.velocity.x > boatMaxSpeed)
            {
                boatBody.velocity = new Vector3(boatMaxSpeed, boatBody.velocity.y, boatBody.velocity.z);
            }
            if (boatBody.velocity.x < -boatMaxSpeed)
            {
                boatBody.velocity = new Vector3(-boatMaxSpeed, boatBody.velocity.y, boatBody.velocity.z);
            }
            if (boatBody.velocity.z > boatMaxSpeed)
            {
                boatBody.velocity = new Vector3(boatBody.velocity.x, boatBody.velocity.y, boatMaxSpeed);
            }
            if (boatBody.velocity.z < -boatMaxSpeed)
            {
                boatBody.velocity = new Vector3(boatBody.velocity.x, boatBody.velocity.y, -boatMaxSpeed);
            }
        }
    }
}
