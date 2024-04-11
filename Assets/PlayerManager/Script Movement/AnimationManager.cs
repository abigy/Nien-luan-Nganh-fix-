using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator animator;
    public InputManager inputManager;
    int horizontal;
    public PlayerManager playerManager;
    public PlayerLocomotion playerLocomotion;
    int vertical;
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent <PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();    
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void PlayerTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting); //khóa hoạt ảnh, để khi phát không để hoạt ảnh khác đè lên
        animator.CrossFade(targetAnimation, 0.45f);  // Làm mờ dần hoạt ảnh
    }
    public void UpdateAnimationValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }
        #endregion
        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }
        #endregion

        if (isSprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2;
        }

        if (horizontalMovement > 0 || verticalMovement > 0)
        {
            snappedVertical = 0.55f;

        }
        else if (horizontalMovement < 1.5 || verticalMovement < 1.5)
        {
            snappedVertical = 0;

        }
        if (isSprinting)
        {
            snappedVertical = 1;

        }


        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1F, Time.deltaTime);
    }

    public void EnableCombo()
    {
        animator.SetBool("canDoCombo", true);
    }


    public void DisableCombo()
    {
        animator.SetBool("canDoCombo", false);
    }
}
