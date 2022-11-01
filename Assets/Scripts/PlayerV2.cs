using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using FMODUnity;

public class PlayerV2 : MonoBehaviour
{
    [Header("Taking Damage and such")]
    public int playerHealth;
    public int maxHealth;
    public float knockback;
    Vector3 forceBackVelocity;
    public float forceBackDrag;
    bool stunLock;
    public ParticleSystem DamageEffect;
    bool death;
    public ParticleSystem playerDeathParticles;
    public GameObject GameOverScreen;
    public StudioEventEmitter mainMusic, gameOverMusic;
    public EventReference hurtSound;


    [Header("Air Boost")]
    public int airBoostAmount;
    public int airBoostCount;
    public float boostSpeed;
    float baseBoostSpeed;
    public float maxBoostSpeed;
    public float boostDrag;
    public float boostChargeSpeed;
    bool isChargingBoost;
    float baseBoostDrag;
    Vector3 boostVelocity;
    float boostVelocityX;
    float boostVelocityZ;
    public ParticleSystem particleJump, particleLeft, particleRight, particleForward, particleBackward;
    public Transform EnergyVisual;
    public float rechargeSpeed;
    float baseRechargeSpeed;
    public EventReference explosion;

    [Header("Base Movement Variables")]
    public Animator playerAnimator;
    public float maxMovementSpeed;
    public float movementSpeed;
    public float airMovementSpeed;
    public float jumpSpeed;
    float baseJumpSpeed;
    public float maxJumpSpeed;
    bool isChargingJump;
    public float chargeSpeed;
    public float jumpDrag;
    Vector3 jumpVelocity;
    float minMaxMoveSpeed;
    Vector3 nextLocation;

    [Header ("Simple Physics")]
    float distToGround;
    CapsuleCollider playerCollider;
    public float gravity;
    float playerWidth;
    public LayerMask ignoreMe;
    Rigidbody rb;

    [Header("Combat")]
    public GameObject punchHitBox;
    public GameObject jumpHitBox;
    public GameObject backHitBox;

    [Header ("Input")]
    PlayerInputActions playerControls;
    InputAction move, fire, jump, aim, focus, boost, attack, cameraTurn, start;

    [Header("Camera")]
    public Transform cameraFollow;
    Vector3 newCameraPosition;
    public CinemachineVirtualCamera playerCam;
    public float cameraTurnSpeed;
    bool movingCamera;
    public bool invertedCameraControls;
    public bool intro;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        minMaxMoveSpeed = maxMovementSpeed;
        distToGround = playerCollider.bounds.extents.y;
        playerWidth = playerCollider.radius;
        playerControls = new PlayerInputActions();
        baseBoostDrag = boostDrag;
        baseJumpSpeed = jumpSpeed;
        baseBoostSpeed = boostSpeed;
        baseRechargeSpeed = rechargeSpeed;
        maxHealth = playerHealth;
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
        //jump.canceled += Jump;

        aim = playerControls.Player.Aim;
        aim.Enable();

        focus = playerControls.Player.Focus;
        focus.Enable();

        boost = playerControls.Player.Boost;
        boost.Enable();
        boost.performed += BoostForward;
        //boost.canceled += BoostForward;

        attack = playerControls.Player.Attack;
        attack.Enable();
        attack.performed += Punch;

        cameraTurn = playerControls.Player.DPad;
        cameraTurn.Enable();
        cameraTurn.performed += MoveCamera;

        start = playerControls.Player.Start;
        start.Enable();
        start.performed += Respawn;
        //boost.performed += BoostToDirection;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        jump.performed -= Jump;
        aim.Disable();
        fire.Disable();
        focus.Disable();
        boost.Disable();
        boost.performed -= BoostForward;
        attack.Disable();
        attack.performed -= Punch;
        cameraTurn.Disable();
        cameraTurn.performed -= MoveCamera;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (death || intro)
            return;

        playerAnimator.SetBool("Grounded", IsGrounded());
        if (!movingCamera)
            Move();

        //if (airBoostCount < airBoostAmount)
        //{
        //    rechargeSpeed -= Time.fixedDeltaTime;

        //    if (rechargeSpeed < 0)
        //    {
        //        airBoostCount++;
        //        rechargeSpeed = baseRechargeSpeed;
        //    }
        //}

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


        }

        if (!IsGrounded())
        {
            transform.position += new Vector3(0, gravity * Time.fixedDeltaTime, 0);

            //movementSpeed = minMaxMoveSpeed / 1.5f;
        }

        if (jumpVelocity.y > 0)
        {
            jumpVelocity = new Vector3(0, jumpVelocity.y - jumpVelocity.y * jumpDrag, 0);
        }
        else if (jumpVelocity.y < 0)
        {
            jumpVelocity = Vector3.zero;
        }

        if (boostVelocity != Vector3.zero)
        {
            if(boostVelocityX < 0)
            {
                boostVelocity = new Vector3(boostVelocity.x - boostVelocity.x * boostDrag, 0, boostVelocity.z);


                if (boostVelocity.x > -0.1f)
                {
                    boostVelocity = new Vector3(0, 0, boostVelocity.z);
                }
            }

            else if(boostVelocityX > 0)
            {
                boostVelocity = new Vector3(boostVelocity.x - boostVelocity.x * boostDrag, 0, boostVelocity.z);

                if (boostVelocity.x < 0.1f)
                {
                    boostVelocity = new Vector3(0, 0, boostVelocity.z);
                }
            }

            if(boostVelocityZ < 0)
            {
                boostVelocity = new Vector3(boostVelocity.x, 0, boostVelocity.z - boostVelocity.z * boostDrag);

                if (boostVelocity.z > -0.1f)
                {
                    boostVelocity = new Vector3(boostVelocity.x, 0, 0);
                }
            }

            else if (boostVelocityZ > 0)
            {
                boostVelocity = new Vector3(boostVelocity.x, 0, boostVelocity.z - boostVelocity.z * boostDrag);

                if (boostVelocity.z < 0.1f)
                {
                    boostVelocity = new Vector3(boostVelocity.x, 0, 0);
                }
            }
        }

        if(forceBackVelocity != Vector3.zero)
        {
            forceBackVelocity -= forceBackVelocity * forceBackDrag;
            forceBackVelocity = new Vector3(forceBackVelocity.x, forceBackVelocity.y * 0.5f, forceBackVelocity.z);

            if(forceBackVelocity.magnitude < 0.1f)
            {
                forceBackVelocity = Vector3.zero;
            }
        }

        //transform.position += new Vector3(0, jumpVelocity * Time.deltaTime, 0);
        //rb.velocity += boostVelocity;


        //transform.position += new Vector3(0, jumpVelocity * Time.deltaTime, 0);
        //transform.position += boostVelocity * Time.deltaTime;

        //CheckCollission();

        if (isChargingJump)
        {
            if (jumpSpeed < maxJumpSpeed)
                jumpSpeed += Time.deltaTime * chargeSpeed;

            else if (jumpSpeed > maxJumpSpeed)
                jumpSpeed = maxJumpSpeed;

            EnergyVisual.localScale = new Vector3(1, 0.1f + 0.9f / (maxJumpSpeed - baseJumpSpeed) * (jumpSpeed - baseJumpSpeed), 1);
        }

        if (isChargingBoost)
        {
            if (boostSpeed < maxBoostSpeed)
                boostSpeed += Time.deltaTime * boostChargeSpeed;

            else if (boostSpeed > maxBoostSpeed)
                boostSpeed = maxBoostSpeed;

            EnergyVisual.localScale = new Vector3(1, 0.1f + 0.9f / (maxBoostSpeed - baseBoostSpeed) * (boostSpeed - baseBoostSpeed), 1);
        }

        if (movingCamera)
        {
            //playerCam.ForceCameraPosition(Vector3.Lerp(playerCam.transform.position, newCameraPosition, cameraTurnSpeed), Quaternion.Euler(transform.position - playerCam.transform.position));
            cameraFollow.position = Vector3.Lerp(cameraFollow.position, newCameraPosition, cameraTurnSpeed);
            if (Vector3.Distance(playerCam.transform.position, newCameraPosition) < 0.1f)
            {
                movingCamera = false;
                playerCam.Follow = transform;
                playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, 8, -30);
                //playerCam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0.1f;
                //playerCam.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 1;
            }
        }

        rb.velocity += nextLocation;
        nextLocation = Vector3.zero;

        rb.AddForce(Physics.gravity * 4, ForceMode.Acceleration);
        rb.AddForce(boostVelocity, ForceMode.Acceleration);
        rb.AddForce(jumpVelocity, ForceMode.Acceleration);
        rb.AddForce(forceBackVelocity, ForceMode.Acceleration);

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


        if (moveDirection == Vector2.zero)
        {
            playerAnimator.SetBool("Running", false);
        }
        else
        {
            playerAnimator.SetBool("Running", true);
        }

        //shoot out a raycast to the wanted location, if it hits a wall, stop moving.
        if (!Physics.Raycast(transform.position - Vector3.up * distToGround / 2, movement.normalized, playerWidth + 0.5f) && !Physics.Raycast(transform.position + Vector3.up * distToGround / 2, movement.normalized, playerWidth + 0.5f))
        {
            if (IsGrounded())
            {
                nextLocation += movement * movementSpeed;
            }

            else
            {
                nextLocation += movement * airMovementSpeed;
            }
        }
        //Debug.Log(movementSpeed);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void ChargeJump(InputAction.CallbackContext context)
    {
        jumpSpeed = baseJumpSpeed;
        isChargingJump = true; 
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (death || intro)
            return;

        Debug.Log("jump");
        isChargingJump = false;
        EnergyVisual.localScale = new Vector3(1, 0.1f, 1);
        if (IsGrounded())
        {
            playerAnimator.SetTrigger("Jump");
            jumpVelocity = Vector3.up * jumpSpeed;

            //rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            //boostDrag = baseBoostDrag / 2;
        }
            

        //When we get input when the player isn't grounded
        //double jump if there's boost count to do so
        if (!IsGrounded() && airBoostCount > 0)
        {
            RuntimeManager.PlayOneShot(explosion);
            playerAnimator.SetTrigger("BoostUp");
            rechargeSpeed = baseRechargeSpeed;
            airBoostCount--;
            particleJump.Play();

            jumpVelocity = Vector3.up * jumpSpeed;
            //rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            //jumpVelocity = jumpSpeed;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            //boostDrag = baseBoostDrag / 2;

            jumpHitBox.SetActive(true);
            StartCoroutine(DisablePunch(jumpHitBox));
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

    void ChargeBoost(InputAction.CallbackContext context)
    {
        boostSpeed = baseBoostSpeed;
        isChargingBoost = true;
    }

    void BoostForward(InputAction.CallbackContext context)
    {
        if (death || intro)
            return;

        EnergyVisual.localScale = new Vector3(1, 0.1f, 1);
        isChargingBoost = false;
        if (airBoostCount > 0)
        {
            RuntimeManager.PlayOneShot(explosion);
            //boostDrag = baseBoostDrag;
            airBoostCount--;
            rechargeSpeed = baseRechargeSpeed;
            Vector3 boostDirection = transform.forward * boostSpeed;
            particleForward.Play();
            boostVelocity = boostDirection;
            boostVelocityX = boostDirection.normalized.x;
            boostVelocityZ = boostDirection.normalized.z;

            //rb.AddForce(boostDirection, ForceMode.Impulse);
            playerAnimator.SetTrigger("BoostForward");

            backHitBox.SetActive(true);
            StartCoroutine(DisablePunch(backHitBox));
        }
    }

    void Punch(InputAction.CallbackContext context)
    {
        if (death || intro)
            return;

        if (airBoostCount > 0)
        {
            RuntimeManager.PlayOneShot(explosion);
            airBoostCount--;
            //boostDrag = baseBoostDrag;
            Vector3 boostDirection = -transform.forward * boostSpeed;
            rechargeSpeed = baseRechargeSpeed;
            particleBackward.Play();
            boostVelocity = boostDirection;
            boostVelocityX = boostDirection.normalized.x;
            boostVelocityZ = boostDirection.normalized.z;

            //rb.AddForce(boostDirection, ForceMode.Impulse);
            playerAnimator.SetTrigger("BoostBackward");

            //create hitbox in front of the player, which hits enemies, and perhaps leaves marks on walls or objects in front of you
            punchHitBox.SetActive(true);
            StartCoroutine(DisablePunch(punchHitBox));
        }
    }

    void MoveCamera(InputAction.CallbackContext context)
    {
        if (death || intro)
            return;

        float distanceFromPlayer = Vector3.Distance(playerCam.transform.position, transform.position);
        Vector3 directionFromPlayer = (playerCam.transform.position - transform.position).normalized;
        Vector2 perpVector = Vector2.Perpendicular(new Vector2(directionFromPlayer.x, directionFromPlayer.z));

        Vector2 cameraTurnDirection = cameraTurn.ReadValue<Vector2>();

        if (movingCamera || cameraTurnDirection.x == 0)
            return;

        if (!invertedCameraControls)
        {
            if (cameraTurnDirection.x > 0)
            {
                newCameraPosition = new Vector3(transform.position.x - perpVector.x * distanceFromPlayer, playerCam.transform.position.y, transform.position.z - perpVector.y * distanceFromPlayer);
            }

            if (cameraTurnDirection.x < 0)
            {
                newCameraPosition = new Vector3(transform.position.x + perpVector.x * distanceFromPlayer, playerCam.transform.position.y, transform.position.z + perpVector.y * distanceFromPlayer);
            }
        }

        if (invertedCameraControls)
        {
            if (cameraTurnDirection.x < 0)
            {
                newCameraPosition = new Vector3(transform.position.x - perpVector.x * distanceFromPlayer, playerCam.transform.position.y, transform.position.z - perpVector.y * distanceFromPlayer);
            }

            if (cameraTurnDirection.x > 0)
            {
                newCameraPosition = new Vector3(transform.position.x + perpVector.x * distanceFromPlayer, playerCam.transform.position.y, transform.position.z + perpVector.y * distanceFromPlayer);
            }
        }


        cameraFollow.position = playerCam.transform.position;
        playerCam.Follow = cameraFollow;

        playerCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, 0, 0);
        playerCam.GetCinemachineComponent<CinemachineTransposer>().m_YDamping = 0;
        playerCam.GetCinemachineComponent<CinemachineTransposer>().m_ZDamping = 0;

        //playerCam.GetCinemachineComponent<CinemachineVirtualCamera>().
        movingCamera = true;
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

    public void TakeDamage(int AmountOfDamage, Vector3 pointOfCollision)
    {
        if (playerHealth > 0)
            playerHealth -= AmountOfDamage;

        RuntimeManager.PlayOneShot(hurtSound);
        rb.velocity = Vector3.zero;
        Vector3 forceBackDirection = (transform.position - pointOfCollision).normalized;
        forceBackDirection = new Vector3(forceBackDirection.x, 0.5f, forceBackDirection.z);
        Debug.Log(forceBackDirection);
        forceBackVelocity = forceBackDirection.normalized * knockback;
        Debug.Log("TAKE DAMAGE");

        DamageEffect.transform.position = pointOfCollision;
        DamageEffect.transform.forward = (transform.position - pointOfCollision).normalized;
        DamageEffect.Play();

        if(playerHealth <= 0)
        {
            Die();
        }
        //trigger damage animation maybe
    }

    public void Die()
    {
        death = true;
        //enable Game Over screen and particles
        mainMusic.Stop();
        gameOverMusic.Play();
        GameOverScreen.SetActive(true);
        playerDeathParticles.Play();
    }

    public void Respawn(InputAction.CallbackContext context)
    {
        if (!death)
            return;
        gameOverMusic.Stop();
        mainMusic.Play();
        death = false;
        playerHealth = 4;
        GameOverScreen.SetActive(false);
        playerDeathParticles.Stop();
        playerDeathParticles.Clear();
        BlackBoard.TeleportObject(BlackBoard.CurrentCheckPoint.position, transform);
    }

    IEnumerator DisablePunch(GameObject toDisable)
    {
        yield return new WaitForSeconds(0.1f);
        toDisable.SetActive(false);
    }
}
