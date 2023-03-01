using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_boyScript_water : MonoBehaviour
{
    private Camera cam;

    public void Start()
    {
        //gameObject.GetComponent<Canvas>().enabled = false;
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
