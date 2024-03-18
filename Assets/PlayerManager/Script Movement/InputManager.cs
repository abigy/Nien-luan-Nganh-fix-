using SG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerControl playerControl;
    PlayerLocomotion playerLocomotion;
    PlayerAttack playerAttack;
    PlayerManager playerManager;
    PlayerInventory playerInventory;

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
}
