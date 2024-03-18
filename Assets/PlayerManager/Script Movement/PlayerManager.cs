using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;
    DameCollider dameCollider;

    [Header("Combo")]
    public bool canDoCombo = true;

    [Header("Is Interacting")]
    public bool isInteracting;
    private void Awake()
    {
        dameCollider = GetComponentInChildren<DameCollider>();
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindObjectOfType<CameraManager>();
    }

    private void Update()
    {
        canDoCombo = animator.GetBool("canDoCombo");
        inputManager.HandleAllInputs();
        isInteracting = animator.GetBool("isInteracting");
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        inputManager.mouse_L = false;
        inputManager.mouse_R = false;

       //isInteracting = animator.GetBool("isInteracting");
        cameraManager.HandleCameraMovement();
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGround", playerLocomotion.isGround);

    }
    //Hàm bật/tắt damecollider
    public void EnableDameColl()
    {
        dameCollider.EnableDamageCollider();
    }

    public void DisableDameColl()
    {
        dameCollider.DisableDamageCollider();
    }
}
