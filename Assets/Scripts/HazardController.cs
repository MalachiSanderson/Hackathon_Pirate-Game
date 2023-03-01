using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour
{
    [Header("Random Spawns")]
    public float random_leak_SpawnChance = 0.00005f;
    public float random_fire_SpawnChance = 0.00005f;

    [Header("Distinguish/Plug time")]
    public float Distinguish_fire_time = 0.001f;
    public float Plug_leak_hole_time = 0.005f;

    List<GameObject> leaks = new List<GameObject>();
    List<GameObject> fires = new List<GameObject>();

    void Start()
    {
        Debug.Log(gameObject.transform.GetChild(0).gameObject.name);
        //Adding leak gameobjects to leaks list
        for (int j = 0; j < gameObject.transform.GetChild(0).gameObject.transform.childCount; j++)
        {
            leaks.Add(gameObject.transform.GetChild(0).gameObject.transform.GetChild(j).gameObject);
            leaks[j].GetComponent<WaterScript>().leakChance = random_leak_SpawnChance;
            leaks[j].GetComponent<WaterScript>().plug_hole_time = Plug_leak_hole_time;

        }

        //Adding fire gameobjects to leaks list
        for (int j = 0; j < gameObject.transform.GetChild(1).gameObject.transform.childCount; j++)
        {
            fires.Add(gameObject.transform.GetChild(1).gameObject.transform.GetChild(j).gameObject);
            fires[j].GetComponent<FireScript>().fireChance = random_fire_SpawnChance;
            fires[j].GetComponent<FireScript>().fillspeed = Distinguish_fire_time;

        }
    }


    public void spawnFire()
    {
        fires[(int)(Random.value * fires.Count)].GetComponent<FireScript>().itsFireTime();
    }

    public void spawnLeak()
    {
        leaks[(int)(Random.value * leaks.Count)].GetComponent<WaterScript>().leak_happening = true;
    }

    public int getNumberOfLeaks()
    {
        int count = 0 ;
        for(int i = 0; i < leaks.Count; i++)
            if (leaks[i].GetComponent<WaterScript>().leak_happening)
                count++;
        return count;
    }

    public int getNumberOfFires()
    {
        int count = 0 ;
        for(int i = 0; i < leaks.Count; i++)
            if (fires[i].GetComponent<FireScript>().onFire)
                count++;
        return count;
    }

    void Update()
    {
        
    }
}
