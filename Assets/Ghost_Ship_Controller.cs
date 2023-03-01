using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost_Ship_Controller : MonoBehaviour
{

    GameObject player;
    Vector3 playerPos;
    Vector3 myPos;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.gameObject.transform.position;
        myPos = gameObject.transform.position;
    }

    void Update()
    {
        if (myPos.z + 20 > playerPos.z || myPos.z-20 < playerPos.z)
        {
            gameObject.transform.Translate(Vector3.forward*0.01f);
            gameObject.transform.Rotate(0, 0.01f, 0);
        }
        else
        {
            gameObject.transform.Translate(Vector3.forward * 0.01f);
        }
    }
}
