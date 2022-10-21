using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerV2 : MonoBehaviour
{
    [Header("Air Boost")]
    public int airBoostAmount;
    int airBoostCount;
    public float boostSpeed;
    public float boostDrag;
    Vector3 boostVelocity;
    float boostVelocityX;
    float boostVelocityZ;
    public ParticleSystem particleJump, particleLeft, particleRight, particleForward, particleBackward;

    [Header("Base Movement Variables")]
    public float maxMovementSpeed;
    public float movementSpeed;
    public float jumpSpeed;
    public float jumpDrag;
    float jumpVelocity;
    float minMaxMoveSpeed;

    [Header ("Simple Physics")]
    float distToGround;
    CapsuleCollider playerCollider;
    public float gravity;
    float playerWidth;
    public LayerMask ignoreMe;
    Rigidbody rb;

    [Header("Combat")]
    public GameObject punchHitBox;

    [Header ("Input")]
    PlayerInputActions playerControls;
    InputAction move, fire, jump, aim, focus, boost, attack;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        minMaxMoveSpeed = maxMovementSpeed;
        distToGround = playerCollider.bounds.extents.y;
        playerWidth = playerCollider.radius;
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        fire = playerControls.Player.Fire;
        fire.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        aim = playerControls.Player.Aim;
        aim.Enable();

        focus = playerControls.Player.Focus;
        focus.Enable();

        boost = playerControls.Player.Boost;
        boost.Enable();
        boost.performed += BoostForward;

        attack = playerControls.Player.Attack;
        attack.Enable();
        attack.performed += Punch;
        //boost.performed += BoostToDirection;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        aim.Disable();
        fire.Disable();
        focus.Disable();
        boost.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (IsGrounded())
        { 
            airBoostCount = airBoostAmount;
            //pop the player up if they are INSIDE the ground
            RaycastHit groundHit;
            Physics.Raycast(transform.position, -Vector3.up, out groundHit);
            //if (groundHit.distance < distToGround)
            //{
            //    transform.position += new Vector3(0, distToGround - groundHit.distance, 0);
            //}

            if (jumpVelocity != jumpSpeed)
            {
                jumpVelocity = 0;
            }

            movementSpeed = minMaxMoveSpeed;
        }

        if (!IsGrounded())
        {
            transform.position += new Vector3(0, gravity * Time.deltaTime, 0);

            if (jumpVelocity > 0)
            {
                jumpVelocity -= Time.deltaTime * jumpDrag;
            }
            if(jumpVelocity < 0)
            {
                jumpVelocity = 0;
            }
            movementSpeed = minMaxMoveSpeed / 1.5f;
        }

        if(boostVelocity != Vector3.zero)
        {
            if(boostVelocityX < 0)
            {
                boostVelocity = new Vector3(boostVelocity.x + Time.deltaTime * boostDrag, 0, boostVelocity.z);


                if (boostVelocity.x > -0.1f)
                {
                    boostVelocity = new Vector3(0, 0, boostVelocity.z);
                }
            }

            else if(boostVelocityX > 0)
            {
                boostVelocity = new Vector3(boostVelocity.x - Time.deltaTime * boostDrag, 0, boostVelocity.z);

                if (boostVelocity.x < 0.1f)
                {
                    boostVelocity = new Vector3(0, 0, boostVelocity.z);
                }
            }

            if(boostVelocityZ < 0)
            {
                boostVelocity = new Vector3(boostVelocity.x, 0, boostVelocity.z + Time.deltaTime * boostDrag);

                if (boostVelocity.z > -0.1f)
                {
                    boostVelocity = new Vector3(boostVelocity.x, 0, 0);
                }
            }

            else if (boostVelocityZ > 0)
            {
                boostVelocity = new Vector3(boostVelocity.x, 0, boostVelocity.z - Time.deltaTime * boostDrag);

                if (boostVelocity.z < 0.1f)
                {
                    boostVelocity = new Vector3(boostVelocity.x, 0, 0);
                }
            }
        }

        transform.position += new Vector3(0, jumpVelocity * Time.deltaTime, 0);
        rb.velocity += boostVelocity;


        //transform.position += new Vector3(0, jumpVelocity * Time.deltaTime, 0);
        //transform.position += boostVelocity * Time.deltaTime;

        Debug.Log(playerWidth);
        //CheckCollission();
    }

    private void FixedUpdate()
    {
        
    }

    void Move()
    {
        Camera cam = Camera.main;


        Vector2 moveDirection = move.ReadValue<Vector2>();

        //change the placement of the values for 3d use.

        float Horizontal = moveDirection.x * movementSpeed;
        float Vertical = moveDirection.y * movementSpeed;


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


        //shoot out a raycast to the wanted location, if it hits a wall, stop moving.
        if(!Physics.Raycast(transform.position, movement.normalized, playerWidth + 0.5f))
        {
            rb.velocity += movement * movementSpeed;
        }
        //Debug.Log(movementSpeed);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround);
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
            jumpVelocity = jumpSpeed;

        //When we get input when the player isn't grounded
        //double jump if there's boost count to do so
        if (!IsGrounded() && airBoostCount > 0)
        {
            airBoostCount--;
            particleJump.Play();

            jumpVelocity = jumpSpeed;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void BoostToDirection(InputAction.CallbackContext context)
    {
        //Do the same as jump. Effect the boostvelocity and slow it down in time.

        if (airBoostCount > 0)
        {
            airBoostCount--;
            //Depend the boostspeed on input direction;
            Vector2 moveDirection = boost.ReadValue<Vector2>();
            Debug.Log(moveDirection);
            //Vector3 boostDirection = new Vector3(moveDirection.x * boostSpeed, 0, moveDirection.y * boostSpeed);
            Vector3 boostDirection = transform.forward * moveDirection.y * boostSpeed + transform.right * moveDirection.x * boostSpeed;

            switch (moveDirection.x)
            {
                case 1:
                    particleLeft.Play();
                    break;

                case -1:
                    particleRight.Play();
                    break;

            }

            switch (moveDirection.y)
            {
                case 1:
                    particleForward.Play();
                    break;

                case -1:
                    particleBackward.Play();
                    break;

            }

            //It needs to be based on either the cameraview, or the player model

            boostVelocity = boostDirection;
            boostVelocityX = boostDirection.x;
            boostVelocityZ = boostDirection.z;
        }
    }

    void BoostForward(InputAction.CallbackContext context)
    {
        if (airBoostCount > 0)
        {
            airBoostCount--;
            Vector3 boostDirection = transform.forward * boostSpeed;
            particleForward.Play();
            boostVelocity = boostDirection;
            boostVelocityX = boostDirection.x;
            boostVelocityZ = boostDirection.z;
        }
    }

    void Punch(InputAction.CallbackContext context)
    {
        Vector3 boostDirection = -transform.forward * boostSpeed;
        particleBackward.Play();
        boostVelocity = boostDirection;
        boostVelocityX = boostDirection.x;
        boostVelocityZ = boostDirection.z;

        //create hitbox in front of the player, which hits enemies, and perhaps leaves marks on walls or objects in front of you
        punchHitBox.SetActive(true);
        StartCoroutine(DisablePunch());
    }

    void CheckCollission()
    {
        //send out raycast in a couple directions and move the player if it collides with something;
        for(float i = -1; i <= 1; i += 0.25f)
        {
            for (float k = -1; k <= 1; k += 0.25f)
            {
                if (Physics.Raycast(transform.position, new Vector3(i,0,k).normalized, playerWidth, ~ignoreMe))
                {
                    RaycastHit hit;
                    Physics.Raycast(transform.position, new Vector3(i, 0, k).normalized, out hit, playerWidth, ~ignoreMe);

                    //move player away from the raycasthit.
                    Vector3 hitDirection = (hit.point - transform.position).normalized;
                    float distanceToHit = Vector3.Distance(transform.position, hit.point);
                    transform.position += -hitDirection * (playerWidth - distanceToHit);
                    //transform.position += hitDirection - (transform.position - hit.point);

                }

                Debug.DrawRay(transform.position, new Vector3(i, 0, k).normalized * playerWidth, Color.green);
            }
        }
    }

    IEnumerator DisablePunch()
    {
        yield return new WaitForSeconds(0.1f);
        punchHitBox.SetActive(false);
    }
}
