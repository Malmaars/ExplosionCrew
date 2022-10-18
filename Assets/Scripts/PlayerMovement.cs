using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public Animator playerAnimator;

    [Header("Air Boost")]
    public float boostStrength;
    public int airBoostAmount;
    int airBoostCount;
    public ParticleSystem airBoostParticles;
    public Speedometer boostAmountUI;
    public GameObject[] directions;
    public Material transparent;
    public Material colored;
    public ParticleSystem rightParticle, leftParticle, backParticle, forwardParticle;

    [Header("Ground Boost")]
    public int groundBoostPower;
    public float groundBoostTimer;
    public float groundBoostSlowdown;
    public ParticleSystem groundBoostParticles;
    CinemachineBasicMultiChannelPerlin cameraNoise;

    [Header("Focus")]
    public Transform locBehindPlayer;
    public Transform target;
    public CinemachineVirtualCamera lockCamera;
    public GameObject reticle;
    public CinemachineDollyCart dollyCart;
    public CinemachineSmoothPath dollyPath;

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
    InputAction move, fire, attack, jump, aim, focus;
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

        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += FireOnTarget;


        attack = playerControls.Player.Attack;
        attack.Enable();
        attack.performed += MovementBurst;

        jump = playerControls.Player.Jump;
        jump.Enable();

        //here we add a function to a button event. I think
        jump.performed += Jump;

        aim = playerControls.Player.Aim;
        aim.Enable();

        focus = playerControls.Player.Focus;
        focus.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        attack.Disable();
        jump.Disable();
        aim.Disable();
        fire.Disable();
        focus.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        AimDirection();
        if (IsGrounded())
        {
            airBoostCount = airBoostAmount;
        }

        Move();
        boostAmountUI.UpdateText(airBoostCount.ToString());
        playerAnimator.SetFloat("MoveSpeed", movementSpeed);
        FocusCameraScript.UpdatePositionBehindPlayer(this.transform, target, locBehindPlayer);

        if (focus.ReadValue<float>() > 0)
        {
            if (!reticle.activeSelf)
                reticle.SetActive(true);

            FocusOnTarget();
            lockCamera.Priority = 12;
            FocusCameraScript.UpdatePositionBehindPlayer(this.transform, target, locBehindPlayer);
        }

        else
        {
            lockCamera.Priority = 1;
            reticle.SetActive(false);
        }
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

            if(movementSpeed < 4)
            {
                movementSpeed = 4;
            }
        }

        if(movementSpeed > maxMovementSpeed + 1)
        {
            cameraNoise.m_AmplitudeGain = movementSpeed / 10;
            movementSpeed -= Time.deltaTime;
        }

        else
        {
            cameraNoise.m_AmplitudeGain = 0;
        }


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

        //Debug.Log(movement);


        transform.position += movement * movementSpeed;
        //Debug.Log(movementSpeed);

        speedometer.UpdateText(((int)(movementSpeed * 6)).ToString());
    }

    //I believe the parameter is to assign a button? I can assign functions to button events now
    void Jump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            Debug.Log("jump");
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            playerAnimator.SetTrigger("Jump");
        }

        //When we get input when the player isn't grounded
        //double jump if there's boost count to do so
        if (!IsGrounded() && airBoostCount > 0)
        {
            airBoostCount--;
            airBoostParticles.Play();

            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            playerAnimator.SetTrigger("Jump");
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

    void MovementBurst(InputAction.CallbackContext context)
    {
        playerAnimator.SetTrigger("Dash");
        groundBoostParticles.Play();

        movementSpeed = 15;
    }

    void AimDirection()
    {
        Vector2 aimDirection = aim.ReadValue<Vector2>();

        directions[0].transform.position = transform.position + Camera.main.transform.right.normalized * 1.5f - transform.up;
        directions[1].transform.position = transform.position - Camera.main.transform.right.normalized * 1.5f - transform.up;
        directions[2].transform.position = transform.position + Camera.main.transform.forward.normalized * 1.5f - transform.up;
        directions[3].transform.position = transform.position - Camera.main.transform.forward.normalized * 1.5f - transform.up;

        foreach (GameObject direction in directions)
        {
            direction.GetComponent<MeshRenderer>().material = transparent; 
        }

        if(aimDirection.x > 0.7)
        {
            //aim right
            directions[0].GetComponent<MeshRenderer>().material = colored;
        }
        if (aimDirection.x < -0.7)
        {
            //aim left
            directions[1].GetComponent<MeshRenderer>().material = colored;
        }
        if (aimDirection.y > 0.7)
        {
            //aim forward
            directions[2].GetComponent<MeshRenderer>().material = colored;
        }
        if (aimDirection.y < -0.7)
        {
            //aim back
            directions[3].GetComponent<MeshRenderer>().material = colored;
        }
    }

    void DirectionBoost(InputAction.CallbackContext context)
    {
        Vector2 aimDirection = aim.ReadValue<Vector2>();
        Camera cam = Camera.main;

        if (aimDirection.x > 0.7)
        {
            //boost right
            rb.AddForce(boostStrength * Time.deltaTime * cam.transform.right, ForceMode.Impulse);
            rightParticle.Play();
        }
        if (aimDirection.x < -0.7)
        {
            //boost left
            rb.AddForce(boostStrength * Time.deltaTime * -cam.transform.right, ForceMode.Impulse);
            leftParticle.Play();
        }
        if (aimDirection.y > 0.7)
        {
            //boost forward
            rb.AddForce(boostStrength * Time.deltaTime * cam.transform.forward, ForceMode.Impulse);
            forwardParticle.Play();
        }
        if (aimDirection.y < -0.7)
        {
            //boost back
            rb.AddForce(boostStrength * Time.deltaTime * -cam.transform.forward, ForceMode.Impulse);
            backParticle.Play();
        }
    }

    void FocusOnTarget()
    {
        reticle.transform.up = (this.transform.position - reticle.transform.position).normalized;

        reticle.transform.position = target.position - (target.position - transform.position).normalized;
    }

    void FireOnTarget(InputAction.CallbackContext context)
    {
        if(focus.ReadValue<float>() > 0)
        {
            //launch yourself to target
            dollyPath.transform.position = transform.position;
            //dollyPath.m_Waypoints[1].position = target.position - (target.position - transform.position) / 2;
            dollyPath.m_Waypoints[1].position = target.position - transform.position;

        }
    }
}
