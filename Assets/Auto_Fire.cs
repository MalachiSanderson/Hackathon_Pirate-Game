using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto_Fire : MonoBehaviour
{
    public float fire_Time = 4;
    float time;

    private void Start()
    {
        time = fire_Time;
    }
    void Update()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            time = fire_Time;

            shootCannons();
        }
    }

    void shootCannons()
    {
        gameObject.GetComponent<ParticleSystem>().Play();
        for(int i = 0; i < transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).transform.GetChild(0).GetComponent<CannonController>().fireCannonBall();
        }
    }
}
