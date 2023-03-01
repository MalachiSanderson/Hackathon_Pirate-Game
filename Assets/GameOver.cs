using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private void Start()
    {
        Invoke("Changescene", 2);
    }

    private void Changescene()
    {
        SceneManager.LoadScene(0);
    }
}
