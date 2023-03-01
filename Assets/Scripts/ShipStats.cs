using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStats : MonoBehaviour
{
    public float maxHullHealth;
    public float maxFloodCapacity;
    public float fireSpawnDamageThreshold;
    public float fireSpawnChance;
    public float leakSpawnChance;
    public bool getsLeaksAndFires;
    public bool areThereAnyLeaks;
    public bool areThereAnyFires;
    public float floodDamagePerLeak;
    public float floodDamageTickTimer;
    public float hullDamagePerFire;
    public float fireDamageTickTimer;
    public float drainSpeed;
    public float repairSpeed;
    public bool isDead;

    public bool readyForFireTick;
    public bool readyForFloodTick;

    private float hullHealth;
    private float floodCapacity;

    private float numberOfLeaks;
    private float numberOfFires;

    public HazardController HZ;

    // Start is called before the first frame update
    void Start()
    {
        hullHealth = maxHullHealth;
        floodCapacity = 0;

        numberOfFires = 0;
        numberOfLeaks = 0;
        if(getsLeaksAndFires && GetComponentInChildren<HazardController>() != null )
        {
            print(gameObject.name + " has Hazards.");
           
        }
    }

    //any time hull damage is dealt above a certian amount there's a chance for fire or a leak to spawn.

    //[TODO]**************
    public void dealHullDamage(float damage)
    {
        hullHealth -= damage;
        print(gameObject.name + " has taken " + damage + " damage. \n Current Hull Health : " + hullHealth);
        checkIfDead();

        
        if( (damage >= fireSpawnDamageThreshold) && (fireSpawnChance > Random.value) )
        {
            //[TODO] Method that spawns fire...
            print("Spawn Fire!");
            HZ.spawnFire();
        }
        
        if (leakSpawnChance > Random.value)
        {
            //[TODO] Method that spawns leak...
            print("Spawn Leak!");
            HZ.spawnLeak();
        }

    }

    public void dealFloodDamage(float damage)
    {
        if(floodCapacity <maxFloodCapacity)
        {
            floodCapacity += damage;
            if(CompareTag("Boat"))
            {
                print(gameObject.name + " has taken on " + damage / 10 + "% of water. \n Current percentage of ship underwater: " + floodCapacity / 10);

            }

        }

    }

    //[TODO]
    public void removingWater()
    {
        

        floodCapacity = Mathf.MoveTowards((floodCapacity), 0, drainSpeed * Time.deltaTime);

    }


    //[TODO]
    public void repairingHull()
    {
        

        hullHealth = Mathf.MoveTowards((hullHealth), maxHullHealth, repairSpeed * Time.deltaTime);

    }

    public IEnumerator floodWait()
    {
        yield return new WaitForSeconds(floodDamageTickTimer);
        readyForFloodTick = true;
    }

    public IEnumerator fireWait()
    {
        yield return new WaitForSeconds(fireDamageTickTimer);
        readyForFireTick = true;
    }


    public void floodTick()
    {
        if(readyForFloodTick)
        {
            readyForFloodTick = false;
            dealFloodDamage(floodDamagePerLeak*numberOfLeaks);
            StartCoroutine(floodWait());
        }
    }


    public void fireTick()
    {
        if (readyForFireTick)
        {
            readyForFireTick = false;
            dealHullDamage(hullDamagePerFire * numberOfFires);
            StartCoroutine(fireWait());
        }
    }


    //[TODO] This needs to check how many leaks and fires there are each update frame...
    public void checkForLeaksAndFires()
    {
        if(getsLeaksAndFires && !isDead)
        {
            numberOfFires = HZ.getNumberOfFires();
            //numberOfFires == *.getNumberOfFires() ???
            if (numberOfFires != 0)
            {
                areThereAnyFires = true;
            }
            else
            {
                areThereAnyFires = false;
            }

            numberOfLeaks = HZ.getNumberOfLeaks();
            //numberOfLeaks ==  *.getNumberOfLeaks()???
            if (numberOfLeaks != 0)
            {
                areThereAnyLeaks = true;
            }
            else
            {
                areThereAnyLeaks = false;
            }
        }


    }

    public void checkIfDead()
    {
        if (!isDead)
        {


            if (hullHealth <= 0)
            {
                dealFloodDamage(maxFloodCapacity);
            }
            if (floodCapacity >= maxFloodCapacity)
            {
                isDead = true;
                if (!CompareTag("Boat"))
                {
                    transform.Translate(new Vector3(0, -(float)10, 0));
                    Destroy(this);
                }

            }

            if (isDead && this.CompareTag("Boat"))
            {
                var go = Instantiate(gameOver);
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().isControlAllowed = false;
                GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatController>().isConstantlyMoving = false;
                GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatController>().isPilotingBoat = false;

            }
        }
    }

    public GameOver gameOver;

    public void updateSinkLevel()
    {
        /*
         * Tier 0 : at default level while floodCapacity > 0.0 (maxFloodCapacity)
         * Tier 1 : level is shifted down by -0.1 while floodCapacity > 0.1 (maxFloodCapacity)
         * ...
         * Tier 10 : level is shifted down by -1.0 while floodCapacity >= (maxFloodCapacity) + is dead + control is lost and all interaction is disabled ([TODO]).
        */
        double sunkenPercentage = (floodCapacity / maxFloodCapacity);
        transform.Translate(new Vector3(0, -(float)sunkenPercentage*0.0001f, 0));
       
        

    }

    // Update is called once per frame
    void Update()
    {
        checkForLeaksAndFires();
        
        if (!areThereAnyLeaks && getsLeaksAndFires)
        {
            removingWater();
            if (!areThereAnyFires)
            {
                repairingHull();
            }
        }
        else
        {
            if(getsLeaksAndFires)
            {
                floodTick();
            }
            
            updateSinkLevel();
        }

        if(areThereAnyFires)
        {
            fireTick();
        }
        checkIfDead();

        //updateSinkLevel();
    }
}
