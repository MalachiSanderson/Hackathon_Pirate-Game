using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoPlayer : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1;
    [SerializeField] private float _rotateSpeed = 30;
    private Rigidbody _rigidbody;
    private readonly Queue<InputState> _inputStates = new Queue<InputState>();
    //*********************************************
    public bool isControlAllowed;
    public bool usesSimplifiedControls;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Store input state of current frame.
        // Reason:  Physics must be handled in FixedUpdate, but in FixedUpdate the input may be missed.
        //          So, we store the input states and use them up in FixedUpdate.
        var inputState = new InputState()
        {
            Horizontal = Input.GetAxisRaw("Horizontal"),
            Vertical = Input.GetAxisRaw("Vertical"),
            Jump = Input.GetButton("Jump")
        };

        _inputStates.Enqueue(inputState);

        //**************************************
        if (!isControlAllowed && Input.GetKey(KeyCode.G))
        {
            BoatController BC = GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatController>();
            if (BC.isPilotingBoat)
            {
                CaptianSeat.exitDriveMode();
            }
        }
    }

    private void FixedUpdate()
    {
        var targetPosition = _rigidbody.position;
        var targetRotation = _rigidbody.rotation;

        // Calculate new position/rotation based on the inputs
        // (ideally, only 1 input state. However, if we missed a frame, this will handle ALL input states we may have missed)
        while(_inputStates.Count > 0)
        {
            var inputState = _inputStates.Dequeue();

            var movement = transform.forward * inputState.Vertical;
            var rotation = Quaternion.Euler(0, inputState.Horizontal * _rotateSpeed * Time.fixedDeltaTime, 0);
            if(movement.sqrMagnitude > 0)
                movement = movement.normalized * _moveSpeed * Time.fixedDeltaTime;
            targetPosition += movement;
            targetRotation *= rotation;
        }
        //*************************************************
        if(isControlAllowed)
        {
            _rigidbody.MovePosition(targetPosition);
            _rigidbody.MoveRotation(targetRotation);
        }
        
    }

    private struct InputState
    {
        public float Horizontal { get; set; }
        public float Vertical { get; set; }
        public bool Jump { get; set; }
    }
}