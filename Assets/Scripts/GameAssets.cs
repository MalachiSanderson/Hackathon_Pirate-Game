using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;
    //some god damn jackass made this shitty script necessacary for his version of damage popups.
    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }



    public Transform enterDriveModeParticleEffect;
    public GameObject cannonBall;
}
