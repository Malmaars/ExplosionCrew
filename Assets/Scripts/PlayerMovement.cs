using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Cinemachine;

public class PlayerMovement : MonoBehaviour 
{
    [Header ("Air Boost")]
    public int airBoostAmount;
    int airBoostCount;
    public ParticleSystem airBoostParticles;
    public Speedometer boostAmountUI;

    [Header("Ground Boost")]
    public int groundBoostPower;
    public float groundBoostTimer;
    public float groundBoostSlowdown;
    public ParticleSystem groundBoostParticles;
    CinemachineBasicMultiChannelPerlin cameraNoise;

    [Header("Base Movement Variables")]
    public float maxMovementSpeed;
    float minMaxMoveSpeed;
    public float movementSpeed;
    public float jumpSpeed;
    Vector3 lastMovementInputDirection;
    public Speedometer speedometer;

    [Header("Camera")]
    public CinemachineVirtualCamera playerCamera;

    float distToGround;
    Rigidbody rb;
    Collider playerCollider;

    PlayerInputActions playerControls;
    InputAction move, attack, jump;
    // Start is called before the first frame update
    void Awake()
    {
        minMaxMoveSpeed = maxMovementSpeed;
        playerControls = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        cameraNoise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        distToGround = playerCollider.bounds.extents.y;
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        attack = playerControls.Player.Attack;
        attack.Enable();
        attack.performed += MovementBoost;

        jump = playerControls.Player.Jump;
        jump.Enable();

        //here we add a function to a button event. I think
        jump.performed += Jump;
    }

    private void OnDisable()
    {
        move.Disable();
        attack.Disable();
        jump.Disable(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded())
        {
            airBoostCount = airBoostAmount;
        }

        Move();
        boostAmountUI.UpdateText(airBoostCount.ToString());
    }
    bool IsGrounded() 
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void Move()
    {
        Camera cam = Camera.main;

        
        Vector2 moveDirection = move.ReadValue<Vector2>();

        if (moveDirection == Vector2.zero)
        {
            moveDirection = lastMovementInputDirection;
            movementSpeed -= Time.deltaTime * groundBoostSlowdown;
            maxMovementSpeed = minMaxMoveSpeed;

            if (movementSpeed <= 0)
            {
                movementSpeed = 0;
            }
        }

        else
        {
            lastMovementInputDirection = moveDirection;

            if (movementSpeed <= maxMovementSpeed)
                movementSpeed += Time.deltaTime;

            if(movementSpeed < 2)
            {
                movementSpeed = 2;
            }
        }

        cameraNoise.m_AmplitudeGain = movementSpeed / 10;


        //change the placement of the values for 3d use.

        float Horizontal = moveDirection.x * movementSpeed * Time.deltaTime;
        float Vertical = moveDirection.y * movementSpeed * Time.deltaTime;


        //move based on the camera view.
        Vector3 horizontalMovement = cam.transform.right;
        horizontalMovement = new Vector3(horizontalMovement.x, 0, horizontalMovement.z);
        horizontalMovement.Normalize();

        Vector3 verticalMovement = cam.transform.forward;
        verticalMovement = new Vector3(verticalMovement.x, 0, verticalMovement.z);
        verticalMovement.Normalize();

        Vector3 movement = horizontalMovement * Horizontal + verticalMovement * Vertical;

        //before moving, turn player to the right direction
        transform.forward = Vector3.Lerp(transform.forward, movement.normalized, 0.5f);

        Debug.Log(movement);


        transform.position += movement * movementSpeed;
        //Debug.Log(movementSpeed);

        speedometer.UpdateText(((int)(movementSpeed * 6)).ToString());
    }

    //I believe the parameter is to assign a button? I can assign functions to button events now
    void Jump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        //When we get input when the player isn't grounded
        //double jump if there's boost count to do so
        if (!IsGrounded() && airBoostCount > 0)
        {
            airBoostCount--;
            airBoostParticles.Play();

            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    void MovementBoost(InputAction.CallbackContext context)
    {
        Debug.Log(context.phase);
        //speed boost bb
        groundBoostParticles.Play();

        //instead of adding force to the rigidbody, I should boost up the max movementspeed;
        //rb.AddForce(transform.forward * groundBoostPower, ForceMode.Impulse);

        maxMovementSpeed += 2;
        movementSpeed += 1;

    }
}
