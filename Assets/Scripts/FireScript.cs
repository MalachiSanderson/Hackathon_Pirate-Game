using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    public float fireChance;
    public bool can_put_out = false;
    public GameObject eboy;
    public bool onFire;
    float fillAmount = 0.0f;
    public float fillspeed;


    private void Start()
    {
        onFire = false;
        eboy.GetComponent<Eboy_script>().image.GetComponent<Image>().fillAmount = fillAmount;

        for (int i = 0; i < gameObject.transform.childCount; i++)
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
    }

    void Update()
    {
        if(can_put_out && Input.GetKey(KeyCode.E))
        {

            fillAmount += fillspeed;
            eboy.GetComponent<Eboy_script>().image.GetComponent<Image>().fillAmount = fillAmount;

            if (fillAmount >= 1)
            {
                fillAmount = 0;
                eboy.GetComponent<Eboy_script>().image.GetComponent<Image>().fillAmount = 0;
                can_put_out = false;
                for (int i = 0; i < gameObject.transform.childCount; i++)
                    gameObject.transform.GetChild(i).gameObject.SetActive(false);

                gameObject.GetComponent<ParticleSystem>().Stop();
            }
        }


        if (fireChance > Random.value && !gameObject.transform.GetChild(0).gameObject.active)
        {
            itsFireTime();
        }
    }

    public void itsFireTime()
    {
        gameObject.GetComponent<ParticleSystem>().Play();

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
        onFire = true;
    }
}
