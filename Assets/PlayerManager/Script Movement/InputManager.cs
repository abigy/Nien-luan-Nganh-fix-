using SG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerControl playerControl;
    PlayerLocomotion playerLocomotion;
    PlayerAttack playerAttack;
    PlayerManager playerManager;
    PlayerInventory playerInventory;
    PlayerState playerState;
    EnemyState enemyState;

    [SerializeField]
    [Header("Slash kill")]
    private GameObject bulletSlash;
    public float SlashSpeed;
    public Transform playerLook;
    public float SlashForce;
    public float fireRate;
    public bool Slash;
    public float range = 100f;
    public bool readyToSlash = true;
    public int SlashCount;
    public int manaToSlash;
    private Coroutine CountDownShoot;
    public float maxDistance = 1;

    [Header("Flash")]
    public bool dash_input;
    

    [Header("Stomp Skill")]
    public float gravityStorm;
    Rigidbody playerRb;
    public float gravityMultiplier;
    public bool Storm;
    public bool readyToStomp;
    public float explosionForce;
    public float explosionRadius;
    public int damgeStorm;
    public int manaToStomp;
    public Transform CrackPos;
    public GameObject stormEffect;
    public bool collided;
    Vector3 direction;

    [Header("Another variable")]
    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool comboFlag;

    public bool b_Input;
    public bool jump_Input;

    public bool mouse_L;
    public bool mouse_R;

    AnimationManager animationManager;

    private void Awake()
    {
        playerState = GetComponent<PlayerState>();
        enemyState = FindObjectOfType<EnemyState>();
        playerRb = GetComponent<Rigidbody>();
        playerAttack = GetComponent<PlayerAttack>();
        animationManager = GetComponent<AnimationManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void OnEnable()
    {
        if (playerControl == null)
        {
            playerControl = new PlayerControl();
            playerControl.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>(); // ghi lại chuyển động của người chơi
            playerControl.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>(); //ghi lại chuyển động chuột hoặc (Tất cả thiết bị liên quan đến Rotate)

            playerControl.Playeraction.B.performed += i => b_Input = true;
            playerControl.Playeraction.B.canceled += i => b_Input = false;
            playerControl.Playeraction.Jump.performed += i => jump_Input = true;
            playerControl.Playeraction.flash.performed += i => dash_input = true;
        }
        playerControl.Enable();
    }

    private void OnDisable()
    {
        playerControl.Disable();
    }

    public void HandleAllInputs()
    {
        dashDame();
        StompSkill();
        ShootInput();
        HandleMovementInput();
        HandlePlayerSprinting();
        HandleJumpInput();
        HandleAttackInput();
    }
    private void HandleMovementInput()
    {
        //if (playerLocomotion.isGround == false)
            //return;

        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput)); //cho ra giá trị luôn dương
        animationManager.UpdateAnimationValues(0, moveAmount, playerLocomotion.isSprinting);
    }

    private void HandlePlayerSprinting()
    {
        if (b_Input && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jump_Input)
        {
            jump_Input = false;
            playerLocomotion.HandleJumping();
        }
    }

    private void HandleAttackInput()
    {
       /* if(playerManager.isInteracting == true)     
            return;*/
        playerControl.Playeraction.Attack_left.performed += i => mouse_L = true;
        playerControl.Playeraction.Attack_right.performed += i => mouse_R = true;

        if (mouse_L)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttack.HandleWeaponCombo(playerInventory.RightWeapon);
                comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;

                if (playerManager.canDoCombo)
                    return;

                playerAttack.HandleLightAttack(playerInventory.RightWeapon);

            }
        }
        if (mouse_R)
        {
            playerAttack.HandleHeavyAttack(playerInventory.LeftWeapon);
        }      
    }

    public void ShootInput()
    {
        playerControl.Playeraction.Skill.performed += i => Slash = true;
        RaycastHit hit;
        if (Slash == true && playerState.hasMana && readyToSlash == true && playerLocomotion.isGround && Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out hit, range))
        {
            Slash = false;
            readyToSlash = false;
            playerState.manaSkill(manaToSlash);
            animationManager.PlayerTargetAnimation("Slash", true);
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * SlashForce);
            }
            GameObject SlashSkill = Instantiate(bulletSlash, playerLook.transform.position, transform.rotation);
            SlashSkill.GetComponent<Rigidbody>().AddForce(transform.forward * SlashSpeed);
            CountDownShoot = StartCoroutine(CountDownSlash());
        }
        //readyToSlash = true;
        Slash = false;
    }

    
    public void StompSkill()
    {
        playerControl.Playeraction.Storm_skill.performed += i => Storm = true;
        var enemies = FindObjectsOfType<EnemyState>();
        if (playerLocomotion.isGround == false && Storm && !readyToStomp && playerState.Ultimate == true)
        {
            Storm = false;
            playerState.Ultimate = false;
            float jumpingVelocity = Mathf.Sqrt(-2 * gravityMultiplier * gravityStorm);
            Vector3 playerVelocity = direction;
            playerVelocity.y = -jumpingVelocity;
            playerRb.velocity = playerVelocity;
            playerState.manaSkill(manaToStomp);

            for (int i = 0; i < enemies.Length; i++)
            {
                if(enemies != null)
                {
                    readyToStomp = true;
                    //enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce,
                     //transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
                    enemies[i].GetComponent<Rigidbody>().AddForce(transform.up * explosionForce);
                    //enemyState.TakeDame(damgeStorm);
                    enemies[i].TakeDame(damgeStorm);
                }
                Debug.Log("Enemy: " + enemies[i]);
            }
            //smash = false;
           // readyToStomp = false;
        }
        Storm = false;
        if(playerLocomotion.isGround == true && readyToStomp == true)
        {
            GameObject stormSkill = Instantiate(stormEffect, CrackPos.transform.position, transform.rotation);
            readyToStomp = false;
            Destroy(stormSkill,1);
        }

    }

    public void dashDame()
    {
        playerControl.Playeraction.flash.performed += i => dash_input = true;
        if (dash_input == true && playerLocomotion.isGround == true)
        {
            dash_input = false;
            playerLocomotion.HanldeDashing();
        }
    }


    IEnumerator CountDownSlash()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("Already Slash");
        readyToSlash = true;
        Slash = false;
    }
}