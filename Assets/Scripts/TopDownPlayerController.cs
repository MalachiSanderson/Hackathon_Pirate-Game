using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPlayerController : MonoBehaviour
{
    public float playerMoveSpeed;
    public float rotateSpeed = 75;
    public bool isControlAllowed;
    

    private float horizontal;
    private float vertical;

    Vector3 acc;
    Vector3 vel;


    private void Start()
    {
        acc = new Vector3(0, 0, 0);
        vel = new Vector3(0, 0, 0);
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void movement()
    {

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.transform.eulerAngles.y;
        //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);
        //transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        transform.Rotate(Vector3.up, horizontal * rotateSpeed * Time.deltaTime);

        if (vertical != 0)
            gameObject.GetComponent<CharacterController>().SimpleMove(transform.forward * (Input.GetKey(KeyCode.LeftShift) ? playerMoveSpeed * 2 : playerMoveSpeed * vertical));

    }

    void Update()
    {
        quitGameInput();

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (isControlAllowed)
        {
            movement();
        }

        if (!isControlAllowed && Input.GetKey(KeyCode.G))
        {

            BoatController BC = GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatController>();
            if (BC.isPilotingBoat)
            {
                CaptianSeat.exitDriveMode();
            }

            CannonController CC = GameObject.FindGameObjectWithTag("Cannon").GetComponent<CannonController>();
            if (CC.isGunning)
            {
                GunningPosition.exitGunningMode();
            }

        }


        float gravity = -9.81f * Time.deltaTime;
        gameObject.GetComponent<CharacterController>().Move(new Vector3(0, gravity, 0));
        if (gameObject.GetComponent<CharacterController>().isGrounded) gravity = 0;

    }

    void quitGameInput() //************LETS PLAYER QUIT GAME...
    {
        if (Input.GetKey(KeyCode.Escape))
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
              Application.Quit();
#endif
            Debug.Log("TODO: MAKE PAUSE MENU...");
        }


    }
}