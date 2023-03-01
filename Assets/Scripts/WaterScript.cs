using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public float leakChance;
    public bool can_put_out = false;
    public GameObject image;
    public bool leak_happening = false;

    float fillAmount = 0.0f;
    public float plug_hole_time;


    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && leak_happening)
        {
            gameObject.transform.GetChild(0).GetComponent<Canvas>().enabled = true;
            can_put_out = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameObject.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
            can_put_out = false;
        }
    }



    private void Start()
    {
        //image.transform.parent.gameObject.SetActive(false);
        image.GetComponent<Image>().fillAmount = fillAmount;

        //for (int i = 0; i < gameObject.transform.childCount; i++)
        //    gameObject.transform.GetChild(i).gameObject.SetActive(false);

        gameObject.transform.GetChild(0).GetComponent<Canvas>().enabled = false;

    }

    void Update()
    {
        if (can_put_out && Input.GetKey(KeyCode.E) )
        {

            gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount += plug_hole_time;
            fillAmount = gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount;

            if (fillAmount >= 1)
            {
                fillAmount = 0;
                gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 0;
                gameObject.transform.GetChild(0).GetComponent<Canvas>().enabled = false;
                can_put_out = false;
                gameObject.GetComponent<ParticleSystem>().Stop();
            }
        }


        if (leakChance > Random.value )
        {
            gameObject.GetComponent<ParticleSystem>().Play();
            leak_happening = true;
        }

        if(leak_happening)
        {
            GameObject.FindGameObjectWithTag("Boat").transform.Translate(0, -.000001f, 0);
        }
    }
}
