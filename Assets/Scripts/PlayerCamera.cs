using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [Header("Control")]
    public bool canScroll = true;
    public bool canRotate = true;
    public bool canOrbit = true;
    public bool acceptsWASDInput = true;

    [Header("Target")]
    public Transform target = null;
    public float heightOffset = 1.0f;

    [Header("Collision")]
    public LayerMask collisionLayers = -1;
    public float offsetFromCollision = 0.2f;

    [Header("Speeds")]
    public float sensitivityHorizontal = 50;
    public float sensitivityVertical = 50;
    public float zoomScrollSpeed = 75;
    public float easeFollowSpeed = 2;

    [Header("Limits")]
    public float verticalMinLimit = -40;
    public float verticalMaxLimit = 80;

    [Header("Distances")]
    public float initialDistance = 10;
    public float minDistance = 2;
    public float maxDistance = 20;
    public float distanceSmoothTime = 0.35f;

    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;
    private float distanceSmoothVelocity;
    private Vector3 m_shakeOffset = Vector3.zero;

    private void Start()
    {
        currentDistance = initialDistance;
        desiredDistance = initialDistance;
        correctedDistance = initialDistance;
#if !UNITY_EDITOR
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#endif

        if (target == null)
        {
            var playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
                target = playerController.transform;
        }

        if (target == null)
            Debug.LogError("Tsk Tsk.. The transform parameter in PlayerCamera is unassigned. No PlayerController was found in the scene either. - Mohammad", gameObject);
    }

    private void LateUpdate()
    {
        if (!target)
            return;

        float horizontalAngle = transform.eulerAngles.y;
        float verticalAngle = transform.eulerAngles.x;
        float targetHorizontalAngle = target.transform.eulerAngles.y;
        float targetVerticalAngle = target.transform.eulerAngles.x;

        if (!IsFocusingUI())
        {
            bool lookAroundInput = Input.GetMouseButton(0);
            bool rotateInput = Input.GetMouseButton(1);
            float rotationX = Input.GetAxis("Mouse X");
            float rotationY = Input.GetAxis("Mouse Y");

            // LMB Hold - Rotate camera around player
            if (lookAroundInput && canOrbit)
            {
                horizontalAngle += rotationX * sensitivityHorizontal * Time.deltaTime;
                verticalAngle -= rotationY * sensitivityVertical * Time.deltaTime;
            }

            // RMB Hold - Rotate player. If big difference, adjust camera.
            if (rotateInput && canRotate)
            {
                // Rotate target to camera's forward
                if (Mathf.Abs(FixAngle(horizontalAngle - targetHorizontalAngle)) > 4)
                {
                    targetVerticalAngle = 0;
                    targetHorizontalAngle = FixAngle(horizontalAngle);
                }
                // Rotate player and camera to face mouse deltas
                else
                {
                    verticalAngle -= rotationY * sensitivityVertical * Time.deltaTime;
                    targetHorizontalAngle += rotationX * sensitivityHorizontal * Time.deltaTime;
                    horizontalAngle = targetHorizontalAngle;
                }
            }

            // Ease behind the target if target is moving
            var isMoving = acceptsWASDInput && ((Math.Abs(Input.GetAxisRaw("Horizontal")) > Mathf.Epsilon) || (Math.Abs(Input.GetAxisRaw("Vertical")) > Mathf.Epsilon));
            if (!lookAroundInput && isMoving)
            {
                horizontalAngle = Mathf.LerpAngle(horizontalAngle, target.transform.eulerAngles.y, easeFollowSpeed * Time.deltaTime);
            }
        }

        // Calculate desired distance
        if (canScroll)
        {
            float zoomInput = Input.mouseScrollDelta.y;
            desiredDistance -= zoomInput * Time.deltaTime * zoomScrollSpeed;
        }
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);

        // Impose angle limits
        verticalAngle = ClampAngle(verticalAngle, verticalMinLimit, verticalMaxLimit);

        // Create updated rotation
        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
        Quaternion targetRotation = Quaternion.Euler(targetVerticalAngle, targetHorizontalAngle, 0);

        // Calculate target position
        Vector3 targetPosition = target.transform.position + heightOffset * Vector3.up;

        // Calculate desired camera position
        Vector3 position = targetPosition - (rotation * Vector3.forward * desiredDistance);

        // Check for collision
        correctedDistance = desiredDistance;
        var isColliding = false;
        if (Physics.SphereCast(targetPosition, 0.1f, position - targetPosition, out var raycastHit, Mathf.Infinity, collisionLayers))
        {
            correctedDistance = Vector3.Distance(targetPosition, raycastHit.point) - offsetFromCollision;
            isColliding = true;
        }

        //  Smooth current distance
        if (!isColliding || correctedDistance > currentDistance)
        {
            currentDistance = Mathf.SmoothDamp(currentDistance, correctedDistance, ref distanceSmoothVelocity, distanceSmoothTime);
        }
        else // collided, instant update
        {
            currentDistance = correctedDistance;
        }

        // Impose distance limits
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Recalculate position base on new current distance
        position = targetPosition - (rotation * Vector3.forward * currentDistance);

        // Update pos/rot
        transform.position = position + m_shakeOffset;
        transform.rotation = rotation;
        target.transform.rotation = targetRotation;
    }

    private float FixAngle(float angle)
    {
        return (angle > 180) ? angle - 360 : angle;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        angle = FixAngle(angle);
        while (angle < -360)
            angle += 360;
        while (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    private bool IsFocusingUI()
    {
        if (EventSystem.current == null)
            return false;

        if (EventSystem.current.currentSelectedGameObject == null && !EventSystem.current.IsPointerOverGameObject())
            return false;

        return true;
    }

    public void Shake(float strength, float duration)
    {
        StartCoroutine(ShakeImpl(strength, duration));
    }

    private IEnumerator ShakeImpl(float strength, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            m_shakeOffset.x = Random.Range(-1.0f, 1.0f) * strength;
            m_shakeOffset.y = Random.Range(-1.0f, 1.0f) * strength;

            elapsed += Time.deltaTime;
            yield return 0;
        }
        m_shakeOffset = Vector3.zero;
    }
}