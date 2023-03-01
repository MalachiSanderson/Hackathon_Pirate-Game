using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eboy_script : MonoBehaviour
{
    private Camera cam;
    public GameObject image;

    public void Start()
    {
       gameObject.GetComponent<Canvas>().enabled = false;

    }

    public void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("I WORK");
        if(collision.tag == "Player")
        {
            gameObject.GetComponent<Canvas>().enabled = true;
            transform.parent.gameObject.GetComponent<FireScript>().can_put_out = true;
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if(collision.tag == "Player")
        {
            gameObject.GetComponent<Canvas>().enabled = false;
        }
    }



    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (cam == null) return;
        transform.rotation = cam.transform.rotation;
    }
}
