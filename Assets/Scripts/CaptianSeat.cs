using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptianSeat : MonoBehaviour
{
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            enterDriveMode();
            //Instantiate(GameAssets.i.enterDriveModeParticleEffect, other.transform.position, Quaternion.identity); //[TODO] WHY DOESN'T THIS WORK?
        }

    }

    public static void enterDriveMode()
    {   
        Debug.Log(" ----> [Make particle effects for activating an interaction point!]");
        PlayerController PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        

        PC.isControlAllowed = false;
        BoatController BC = GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatController>();
        BC.isPilotingBoat = true;
        BC.isConstantlyMoving = false; //remove if not desired.

        GameObject[] allInteractionPoints = GameObject.FindGameObjectsWithTag("Interaction Point");
        foreach (GameObject interactionPoint in allInteractionPoints)
        {
            interactionPoint.GetComponent<BoxCollider>().enabled = false;
            interactionPoint.GetComponent<SpriteRenderer>().enabled = false;
        }
        print("Deactivated all Interaction zones.");
    }

    public static void exitDriveMode()
    {
        Debug.Log(" ----> [Make particle effects for interaction points you can interact with!]");
        PlayerController PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
       

        PC.isControlAllowed = true;
        BoatController BC = GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatController>();
        BC.isPilotingBoat = false;
        //BC.isConstantlyMoving = true; //remove if not desired.
                                      //[TODO] Goal is to make it so that upon relinquishing control of boat it continues moving in the direction it was going!

        GameObject[] allInteractionPoints = GameObject.FindGameObjectsWithTag("Interaction Point");
        foreach (GameObject interactionPoint in allInteractionPoints)
        {
            interactionPoint.GetComponent<BoxCollider>().enabled = true;
            interactionPoint.GetComponent<SpriteRenderer>().enabled = true;
        }
        print("Reactivated all Interaction zones.");
    }
}
