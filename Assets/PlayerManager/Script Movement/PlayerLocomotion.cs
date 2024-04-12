using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    // Start is called before the first frame update

    AnimationManager animationManager;
    PlayerManager playerManager;
    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObj;
    public Rigidbody playerRb;
    PlayerState playerState;

    public Transform myTransform;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGround;
    public bool isJumping;
    public bool readyDBJump = false;

    /*[Header("Handle Dash")]
    public float dash_force;
    public bool readyToDash = true;
    public float dash_coolDown;*/

    [Header("Power Up System")]
    public bool hasPowerUp = false;
    private Coroutine powerUpCountDown;
    public GameObject heightEffect;
    public float currentJump;
    public int secondCountDown;
    public PowerUpType currentPower = PowerUpType.None;

    [Header("Falling")]
    public float inAirTime;
    public float leapingVelocity;
    public float fallingSpeed;
    public LayerMask groundLayer;
    public float maxDistance = 1;
    public float raycastHeightOffset = 0.5f;//Bắn raycast từ vị trí chân player

    [Header("Movement speed")]
    public float walkingSpeed = 1.5f;
    public float sprintingSpeed = 7;
    public float movementSpeed = 7; //running speed
    public float rotationSpeed = 15;

    [Header("Jump Speeds and Double jump")]
    public float jumpingHeight = 3;
    public float gravityIntensity = -15;
    public float numOfDBJump;

    private void Awake()
    {
        isGround = true;
        //readyToDash = true;
        currentJump = jumpingHeight;
        playerState = GetComponent<PlayerState>();
        animationManager = GetComponent<AnimationManager>();
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRb = GetComponent<Rigidbody>();
        cameraObj = Camera.main.transform;
    }

    private void HandleMovement()
    {
        if (isJumping)
            return;

        moveDirection = cameraObj.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObj.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * movementSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed;
            }
        }

        Vector3 moveVelocity = moveDirection;
        playerRb.velocity = moveVelocity;
    }

    public void HandleAllMovement()
    {

        HandFallingAndLanding();
        if (playerManager.isInteracting) //check player đang Falling nếu player đang Falling thì return lại câu lệnh
            return;                              

        HandleMovement();
        HandleRotation();

        if (isJumping)
            return;

    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameraObj.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObj.right * inputManager.horizontalInput; // Cộng tiếp tục cho camera chiều ngang,
                                                                                            // nếu không gọi lại biến targetDirection thì sẽ được hiểu như gán giá trị mới
                                                                                            // và camera forward sẽ bị ghi đè
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); //Nội suy vòng xoay giữa điểm A và B

        transform.rotation = playerRotation;
    }

    private void HandFallingAndLanding()
    {
        RaycastHit hit; //Tạo một đường raycast để check player có đang ở mặt đất không
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += raycastHeightOffset;
        Vector3 targetPosition;
        targetPosition = transform.position;

        if (!isGround && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animationManager.PlayerTargetAnimation("Falling", true);
            }

            inAirTime *= Time.deltaTime;
            playerRb.AddForce(transform.forward * leapingVelocity); //Nếu người chơi nhảy khỏi bờ vực sẽ tăng một lượng nhỏ tốc độ khi nhảy ra
            playerRb.AddForce(-Vector3.up * fallingSpeed * inAirTime); // Trọng lực kéo người chơi rơi xuống
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, maxDistance, groundLayer))
        {
            if (!isGround)
            {
                animationManager.PlayerTargetAnimation("Land", true);
            }

            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTime = 0;
            isGround = true;
            playerManager.isInteracting = false;
        }
        else
        {
            isGround = false;
        }

        if(isGround && !isJumping)
        {
            if(playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            } else
            {
                transform.position = targetPosition;
            }
        }
    }

    /*public void HandleJumping()
    {
        if (isGround)
        {
            animationManager.animator.SetBool("isJumping", true);
            animationManager.PlayerTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpingHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRb.velocity = playerVelocity;
            readyDBJump = true;

        } else if(isGround == false && readyDBJump == true)
        {
            animationManager.animator.SetBool("isJumping", true);
            animationManager.PlayerTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * numOfDBJump);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRb.velocity = playerVelocity;
            readyDBJump = false;
        }
    }*/

    public void HandleJumping()
    {
        if (isGround)
        {
            animationManager.animator.SetBool("isJumping", true);
            animationManager.PlayerTargetAnimation("Jump", false);
            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpingHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRb.velocity = playerVelocity;
            // Lấy vị trí và hướng quay của player
            moveDirection = cameraObj.forward * inputManager.verticalInput;
            moveDirection += cameraObj.right * inputManager.horizontalInput;
            //animationManager.PlayerTargetAnimation("Jump", true);
            moveDirection.y = 0;
            //Lấy hướng xoay (hướng di chuyển) LookRotation
            Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
            myTransform.rotation = jumpRotation;

            readyDBJump = true;

        }
        else if (isGround == false && readyDBJump == true)
        {
            animationManager.animator.SetBool("isJumping", true);
            animationManager.PlayerTargetAnimation("Jump", false);
            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * numOfDBJump);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRb.velocity = playerVelocity;

            moveDirection = cameraObj.forward * inputManager.verticalInput;
            moveDirection += cameraObj.right * inputManager.horizontalInput;
            //animationManager.PlayerTargetAnimation("Jump", true);
            moveDirection.y = 0;
            Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
            myTransform.rotation = jumpRotation;

            readyDBJump = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            currentPower = other.gameObject.GetComponent<PowerUp>().PowerType;

            if(currentPower == PowerUpType.HeightJump)
            {
                heightEffect.gameObject.SetActive(true);
                jumpingHeight *= 2;
                Debug.Log("Jump power");
            }

            Destroy(other.gameObject);
            
            if (powerUpCountDown != null)
            {
               Debug.Log("Has Power Jump");
               StartCoroutine(CountDownPower());
            }
            powerUpCountDown = StartCoroutine(CountDownPower());
        }
        //Bonus mana or heath (Not powerup)
        if (other.CompareTag("Bonus"))
        {
            currentPower = other.gameObject.GetComponent<PowerUp>().PowerType;
            if (currentPower == PowerUpType.bonusMana)
            {
                Debug.Log("Has full mana");
                playerState.recoverMana(playerState.maxMana);
            }
            Destroy(other.gameObject);
        }

    }

    /*public void HanldeDashing()
    {
        if (playerManager.isInteracting)
            return;

        if (readyToDash == true)
        {
            Debug.Log("Dash");
            readyToDash = false;
            animationManager.PlayerTargetAnimation("flash", true);
            Vector3 playerLook = new Vector3(transform.forward.x * dash_force, 0f, transform.forward.z * dash_force);
            playerRb.AddForce(playerLook, ForceMode.Impulse);
            StartCoroutine(HandleCountDownDash());
        }
    }*/

    IEnumerator CountDownPower()
    {
        yield return new WaitForSeconds(secondCountDown);
        hasPowerUp = false;
        currentPower = PowerUpType.None;
        jumpingHeight = currentJump;
        heightEffect.gameObject.SetActive(false);
        Debug.Log("End power");
    }

    /*IEnumerator HandleCountDownDash()
    {
        yield return new WaitForSeconds(dash_coolDown);
        readyToDash = true;
    }*/
}
