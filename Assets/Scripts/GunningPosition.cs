using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GunningPosition : MonoBehaviour
{
    public CannonController thisCannon;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            enterGunningMode(other);
            //Instantiate(GameAssets.i.enterDriveModeParticleEffect, other.transform.position, Quaternion.identity); //[TODO] WHY DOESN'T THIS WORK?
        }

    }

    public void enterGunningMode(Collider other)
    {
        Debug.Log(" ----> [Make particle effects for activating an interaction point!]");
        PlayerController PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        PC.isControlAllowed = false;
        
        //CannonController CC = GameObject.FindGameObjectWithTag("Cannon").GetComponent<CannonController>();
        thisCannon.isGunning = true;

         GameObject[] allInteractionPoints = GameObject.FindGameObjectsWithTag("Interaction Point");
        foreach (GameObject interactionPoint in allInteractionPoints)
        {
            interactionPoint.GetComponent<BoxCollider>().enabled = false;
            interactionPoint.GetComponent<SpriteRenderer>().enabled = false;
        }
        print("Deactivated all Interaction zones.");
    }

    public static void exitGunningMode()
    {
        Debug.Log(" ----> [Make particle effects for interaction points you can interact with!]");
        PlayerController PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        
        PC.isControlAllowed = true;
        

        GameObject[] allCannons = GameObject.FindGameObjectsWithTag("Cannon");
        foreach (GameObject cannon in allCannons)
        {
            cannon.GetComponent<CannonController>().isGunning = false;
            
        }

        GameObject[] allInteractionPoints = GameObject.FindGameObjectsWithTag("Interaction Point");
        foreach (GameObject interactionPoint in allInteractionPoints)
        {
            interactionPoint.GetComponent<BoxCollider>().enabled = true;
            interactionPoint.GetComponent<SpriteRenderer>().enabled = true;
        }
        print("Reactivated all Interaction zones.");
    }
}
