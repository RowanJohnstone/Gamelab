using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");
        bool jumpPressed = Input.GetKey("space");
        bool crouchPressed = Input.GetKey("c");
        bool slidePressed = Input.GetKey("left alt");
        bool ledgeHere = true;

      if(!isWalking && forwardPressed)
      {
          // then set the isWalking boolean to be true
        animator.SetBool("isWalking", true);
      }   

      //if player is not pressing w key
      if (isWalking && !forwardPressed)
      {
        // then set the isWalking boolean to be false
          animator.SetBool("isWalking", false);
      }

      // if player is walking and presses left shift
      if (forwardPressed && runPressed)
      {
        // then set the isRunning boolean to be true
        animator.SetBool("isRunning", true);
      }

      if(!forwardPressed || !runPressed)
      {
        // then set the isRunning boolean to be false
        animator.SetBool("isRunning", false);
      }

        // if running and space is pressed set the isJumping boolean to be true
      if(runPressed && jumpPressed)
      {
        // then set the isJumping boolean to be true
        animator.SetBool("isJumping", true);
      }

        // if walking and space is pressed then set isJumping to be true
      if(forwardPressed && jumpPressed)
      {
        // then set the isJumping boolean to be true
        animator.SetBool("isJumping", true);
      }
      
      if(forwardPressed && jumpPressed && ledgeHere == true)
      {
        // then set the isJumping boolean to be true
        animator.SetTrigger("LedgeGrab");
      }

      if(crouchPressed)
      {
        // then set the isCrouching boolean to be true
          animator.SetBool("isCrouching", true);
      }
      else if (animator.GetBool("isCrouching"))
      {
        animator.SetBool("isCrouching", false);
      }

      if(crouchPressed && forwardPressed)
      {
        // then set the isCrouchWalking boolean to be true
        animator.SetBool("isCrouchWalking", true);
      }

      if(!crouchPressed && !forwardPressed)
      {
        // then set the isCrouchWalking boolean to be true
        animator.SetBool("isCrouchWalking", false);
      }
      
      if(runPressed && slidePressed)
      {
        // then set the isSliding boolean to be true
        animator.SetBool("isSliding", true);
      }

      //if(runPressed && !slidePressed)
      //{
      //  // then set the isSliding boolean to be true
      //  animator.SetBool("isSliding", false);
      //}
    }

    public void Landing()
    {
      animator.SetBool("HardLanding", true);
      animator.SetBool("isJumping", false);
      
    }

    public void LandtoRunning()
    {
      animator.SetBool("isRunning", true);
    }

    public void disableMovement()
    {
       animator.SetBool("isRunning", false);
      animator.SetBool("isWalking", false);
    }

    public void stopJumping()
    {
      animator.SetBool("isJumping", false);
    }
}
