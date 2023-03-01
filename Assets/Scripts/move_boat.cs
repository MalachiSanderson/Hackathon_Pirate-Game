using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_boat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(0.02f, 0, 0);
    }
}
