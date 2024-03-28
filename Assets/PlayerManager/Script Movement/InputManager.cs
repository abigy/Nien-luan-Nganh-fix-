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
    EnemyState enemyState;

    [SerializeField]
    [Header("Slash kill")]
    private GameObject bulletSlash;
    public float SlashSpeed;
    public Transform playerLook;
    public float SlashForce;
    public float fireRate;
    private float FireDelay = 0f;
    public bool Slash;
    public float range = 100f;
    public float maxDistance = 1;

    [Header("Storm Skill")]
    public float gravityStorm;
    Rigidbody playerRb;
    public float gravityMultiplier;
    public bool Storm;
    public bool smash;
    public float explosionForce;
    public float explosionRadius;
    public int damgeStorm;
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
        }
        playerControl.Enable();
    }

    private void OnDisable()
    {
        playerControl.Disable();
    }

    public void HandleAllInputs()
    {
        SmashSkill();
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
        if (Slash == true && Time.time >= FireDelay && Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out hit, range))
        {
            Slash = false;
            //Debug.Log("bang");
            FireDelay = Time.time + 1f / fireRate;

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * SlashForce);
                //Debug.Log("Va cham");
            }
            GameObject SlashSkill = Instantiate(bulletSlash, playerLook.transform.position, transform.rotation);
            SlashSkill.GetComponent<Rigidbody>().AddForce(transform.forward * SlashSpeed);
            //Destroy(SlashSkill, 2);
        }

    }

    
    public void SmashSkill()
    {
        playerControl.Playeraction.Storm_skill.performed += i => Storm = true;

        var enemies = FindObjectsOfType<EnemyState>();
        if (playerLocomotion.isGround == false && Storm && !smash)
        {
            smash = true;
            /*if()
            {
                GameObject stormSkill = Instantiate(stormEffect, CrackPos.transform.position, transform.rotation);
            }*/

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityMultiplier * gravityStorm);
            Vector3 playerVelocity = direction;
            playerVelocity.y = -jumpingVelocity;
            playerRb.velocity = playerVelocity;

            for(int i = 0; i < enemies.Length; i++)
            {
                if(enemies != null)
                {
                    enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce,
                                transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
                    enemyState.TakeDame(damgeStorm);
                }
            }

            //smash = false;
            Storm = false;
        }

        if(playerLocomotion.isGround == true && smash == true)
        {
            GameObject stormSkill = Instantiate(stormEffect, CrackPos.transform.position, transform.rotation);
            smash = false;
            Destroy(stormSkill,1);
        }
    }

   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Default" && collided == true)
        {
            collided = false;
            GameObject stormSkill = Instantiate(stormEffect, playerLook.transform.position, transform.rotation);
            Destroy(stormSkill,1);
        }
        collided = true;
    }*/
}