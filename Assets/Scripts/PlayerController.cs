using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Control")]
    public bool isControlAllowed = true;
    public bool isDead = false;
    public bool isDebug = false;

    [Header("Speeds")]
    public float forwardSpeed = 100;
    public float backwardSpeed = 100;
    public float strafeSpeed = 100;
    public float sprintMultiplier = 2f;
    public float jumpSpeed = 10;
    public float terminalSpeed = 20;
    public float gravity = 40;
    public float pushPower = 8;

    [Header("Collision")]
    public bool visualizeFeet = false;
    public LayerMask feetLayerMask = ~0;
    public Vector3 feetOffset = Vector3.zero;
    public float groundedMargin = 0.02f;

    [Header("Swimming")]
    public LayerMask waterLayerMask = 0;
    public float gravityMultiplierWhileSwimming = 0.25f;
    public float jumpMultiplierWhileSwimming = 0.25f;
    public float movementMultiplierWhileSwimming = 0.35f;

    [Header("Swinging")]
    public float movementMultiplierWhileSwinging = 0.25f;

    [Header("Audio")]
    public PlayAudio walkingAudio;
    public PlayAudio deathAudio;

    private Animator animator;
    private OceanChunkSystem oceanChunkSystem = null;
    private Vector3 vel = Vector3.zero;
    private CharacterController characterController;
    private PlayerCamera playerCamera;

    private string GetCurrentAnimationName()
    {
        try
        {
            return animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        }
        catch (Exception)
        {
            return "";
        }
    }

    public bool IsTaunting => GetCurrentAnimationName() == "Taunt";
    public bool IsDancing => GetCurrentAnimationName() == "Dance";
    public bool IsSwinging => GetCurrentAnimationName() == "Swing";
    public bool IsMoving => isControlAllowed && !IsTaunting && !isDead && (Math.Abs(Input.GetAxisRaw("Vertical")) > float.Epsilon || Math.Abs(Input.GetAxisRaw("Horizontal")) > float.Epsilon);

    public bool IsSprinting
    {
        get
        {
            try
            {
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool IsSwimming
    {
        get
        {
            var waterCollider = oceanChunkSystem.GetWaterCollider();
            var inBounds = (waterCollider != null) && characterController.bounds.Intersects(waterCollider.bounds);
            var waterBelow = Physics.Linecast(transform.position, transform.position + Vector3.down * 0.1f, out var waterHit, waterLayerMask, QueryTriggerInteraction.Collide);
            var landBelow = Physics.Linecast(transform.position, transform.position + Vector3.down * 0.1f, out var landHit, feetLayerMask, QueryTriggerInteraction.Collide);

            if (waterBelow && landBelow)
                return waterHit.distance < landHit.distance;

            return inBounds;
        }
    }

    public bool IsAboveWater
    {
        get
        {
            var waterCollider = oceanChunkSystem.GetWaterCollider();
            return (waterCollider != null) && characterController.bounds.max.y >= waterCollider.bounds.max.y;
        }
    }

    public bool IsGrounded => Physics.CheckSphere(transform.position + Vector3.up * characterController.radius + feetOffset, characterController.radius + groundedMargin, feetLayerMask);

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = FindObjectOfType<PlayerCamera>();

        if (playerCamera == null)
            Debug.LogError("Tsk tsk... The PlayerController failed to locate a PlayerCamera in the scene. -Mohammad", gameObject);

        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogError("Tsk tsk.. The PlayerController has no Animator attached in any of its hierarchy structure. -Mohammad", gameObject);

        oceanChunkSystem = FindObjectOfType<OceanChunkSystem>();
        if (oceanChunkSystem == null)
            Debug.LogError("Tsk tsk... The PlayerController could not find the scene's ocean chunk system -Mohammad", gameObject);
    }

    private void HandleMovement()
    {
        var delta = Vector3.zero;

        // Apply gravity
        if (IsGrounded)
            vel.y = 0;
        else
        {
            if (IsSwimming)
                vel.y -= gravity * gravityMultiplierWhileSwimming * Time.deltaTime;
            else
                vel.y -= gravity * Time.deltaTime;
        }

        // Get user input
        if (isControlAllowed && !IsTaunting && !isDead)
        {
            /////////////////// XZ-axis Movement ///////////////////
            // Get input
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");

            // Forward
            if (vertical > 0)
                delta += vertical * transform.forward * forwardSpeed;
            // Backward
            else
                delta += vertical * transform.forward * backwardSpeed;

            // Strafe
            delta += horizontal * transform.right * strafeSpeed;

            // Sprint
            if (IsSprinting && !IsSwimming)
                delta *= sprintMultiplier;

            // Swimming multiplier
            if (IsSwimming)
                delta *= movementMultiplierWhileSwimming;

            // Swinging multiplier
            if (IsSwinging)
                delta *= movementMultiplierWhileSwinging;

            // Move in XZ plane
            vel.x = delta.x;
            vel.z = delta.z;

            /////////////////// Y-axis Movement ///////////////////
            if (Input.GetButton("Jump"))
            {
                if (IsGrounded && !IsSwimming)
                {
                    animator.SetTrigger("Jump");
                    vel.y = jumpSpeed;
                }
                else if (IsSwimming && !IsAboveWater)
                {
                    vel.y = Mathf.Lerp(vel.y, jumpSpeed * jumpMultiplierWhileSwimming, 0.1f);
                }
            }

            // Animator
            animator.SetBool("IsMoving", IsMoving);
            animator.SetFloat("InputHorizontal", Input.GetAxis("Horizontal") * (0.5f + (IsSprinting ? 0.5f : 0)), 0.25f, Time.smoothDeltaTime * 10);
            animator.SetFloat("InputVertical", Input.GetAxis("Vertical") * (0.5f + (IsSprinting ? 0.5f : 0)), 0.25f, Time.smoothDeltaTime * 10);
        }
        else
        {
            vel.x = 0;
            vel.z = 0;

            // Animator
            animator.SetBool("IsMoving", false);
            animator.SetFloat("InputHorizontal", 0, 0.5f, Time.smoothDeltaTime * 10);
            animator.SetFloat("InputVertical", 0, 0.5f, Time.smoothDeltaTime * 10);
        }

        // Terminal speed
        vel.y = Mathf.Clamp(vel.y, -terminalSpeed, terminalSpeed);

        // Animator
        animator.SetFloat("VerticalVelocity", vel.y);
        animator.SetBool("IsGrounded", IsGrounded && Mathf.Approximately(vel.y, 0));
        animator.SetBool("IsSwimming", IsSwimming);
        animator.SetBool("IsDead", isDead);

        // Apply movement
        characterController.Move(vel * Time.deltaTime);
    }

    public void Die()
    {
        isDead = true;
        walkingAudio.Play();
    }

    public void Revive()
    {
        isDead = false;
        walkingAudio.Stop();
    }

    private void Update()
    {
        // Handle all of movement (WASD, jumping, gravity)
        HandleMovement();

        // Taunt
        if (Input.GetKeyDown(KeyCode.T) && !IsTaunting)
            animator.SetTrigger("Taunt");

        // Dance
        if (Input.GetKeyDown(KeyCode.R) && !IsDancing)
            animator.SetTrigger("Dance");

        // Swinging
        if (Input.GetKeyDown(KeyCode.F) && !IsSwinging)
            animator.SetTrigger("Swing");

        // Disable camera's rotation capability upon death
        playerCamera.canRotate = !isDead;
        playerCamera.acceptsWASDInput = !isDead;

        // Walking Sound
        if (IsMoving && !walkingAudio.IsPlaying)
            walkingAudio.Play();
        else if (!IsMoving && walkingAudio.IsPlaying)
            walkingAudio.Stop();
        if (walkingAudio.source != null)
            walkingAudio.source.pitch = 1f + (IsSprinting ? 0.5f : 0);

        if (!isControlAllowed && Input.GetKey(KeyCode.G))
        {
            BoatController boatController = GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatController>();
            if (boatController.isPilotingBoat)
            {
                CaptianSeat.exitDriveMode();
            }

            
            else
            {
                GunningPosition.exitGunningMode();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!visualizeFeet || !characterController) return;

        Gizmos.color = (IsGrounded) ? Color.red : Color.white;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * characterController.radius + feetOffset, characterController.radius + groundedMargin);
        Gizmos.color = Color.white;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;
        var dir = (hit.point - transform.position).normalized;
        body.AddForce(dir * pushPower * vel.magnitude, ForceMode.Force);
    }

    private void OnGUI()
    {
        if (!isDebug) return;

        GUILayout.Box("IsGrounded " + IsGrounded);
        GUILayout.Box("Swinging " + IsSwinging);
        GUILayout.Box("IsDancing " + IsDancing);
        GUILayout.Box("IsTaunting " + IsTaunting);
        GUILayout.Box("IsSwimming " + IsSwimming);
        GUILayout.Box("IsAboveWater " + IsAboveWater);
        GUILayout.Box("Velocity = " + vel);

        if (isDead && GUILayout.Button("Revive"))
            Revive();
        else if (!isDead && GUILayout.Button("Die"))
            Die();
    }
}