using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float speed;
    public float shotLifeTime;
    private Rigidbody ball;
    public GameObject explode;
  
    // Start is called before the first frame update
    void Start()
    {
        ball = GetComponent<Rigidbody>();
        StartCoroutine(ballLifeTime());
        ball.velocity = transform.forward * speed;

    }

    // Update is called once per frame
    void Update()
    {
        ball.velocity = transform.forward * speed;
        //Debug.Log(ball.velocity);

    }

    private void OnTriggerEnter(Collider other)
    {
        explode.GetComponent<ParticleSystem>().Play();

    }


    public IEnumerator ballLifeTime()
    {
        yield return new WaitForSeconds(shotLifeTime);
        Destroy(this.gameObject);
    }
}
