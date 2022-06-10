using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class NewAnimationController : MonoBehaviour
{
    public Animator animator;
    int fallcounter = 1;

    // Movement Speed Parameters
   public float velocity = 0.0f;
    public float acceleration = 1.0f;
    public float deceleration = 2.0f;
    public float slideDeceleration = 2.5f;
    public float jumpHeight = 7f;
    public float airDrag = 1.5f;
    private int VelocityHash;
    PlayerController playerController;
    public GameObject player;
    private RigidbodyFirstPersonController rbfps;
    private Rigidbody rb;
    public bool idlestopper = false;
    public bool stop;
    public bool roll;
    public DetectObs detectVaultObject;
    public float speedo;

    private IEnumerator fallingCoroutine = null;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        rbfps = GetComponentInParent<RigidbodyFirstPersonController>();
        rb = GetComponent<Rigidbody>();
        //_rigidbody = GetComponentInChildren<Rigidbody>();
        // increases performance
        VelocityHash = Animator.StringToHash("Velocity");

    }

    // Update is called once per frame
    void Update()
    {
        speedo = rbfps.movementSettings.ForwardSpeed;
        speedo = speedo / 110.0f;
        
        
        //Animation Inputs & Parameters

        bool forwardPressed = Input.GetKey("w");
        bool backwardsPressed = Input.GetKey("s");
        bool jumpPressed = Input.GetKeyDown("space");
        bool slidePressed = Input.GetKeyDown("left alt");

        //Forward movement Walk -> Run
        if (forwardPressed && velocity < 1.0f)
        {
            velocity += Time.deltaTime * acceleration;
        }

        if (velocity > 0.1f  && rbfps.IsAir == false)
        {
            animator.SetBool("Running", true);
        }

        if (velocity < 0.1f && rbfps.IsAir == false)
        {
            animator.SetBool("Running", false);
        }



        if (rbfps.IsAir == true && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk to Run"))
        {
            print("air");
            animator.SetBool("Running", false);
            animator.speed = 0.3f;
            animator.speed = 0.3f;
        }

        else
        {
            animator.speed = speedo;
        }

        //Backwards Walk
        if (backwardsPressed)
        {
            // Set boolean true
            animator.SetBool("backwardsWalk", true);
        }
        else if (!backwardsPressed)
        {
            // Set boolean false
            animator.SetBool("backwardsWalk", false);
        }

        // Reduce velocity by deceleration when not pressing forward
        if (!forwardPressed && velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration;
        }

        // In case velocity ever goes below zero
        if (!forwardPressed && velocity < 0.0f)
        {
            velocity = 0.0f;
        }

        if (velocity == 0)
        {
            animator.SetTrigger("Idle");
        }

        animator.SetFloat(VelocityHash, velocity);





        if (playerController.climbing == true)
        {
            animator.speed = speedo;
            animator.SetBool("Climb", true);
            animator.SetBool("Vault2", false);
            playerController.climbing = false;
            if (fallingCoroutine != null)
            {
                StopCoroutine(fallingCoroutine);
                fallingCoroutine = null;
            }

        }

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Braced Hang To Crouch"))
        {

            animator.SetBool("Climb", false);
            
        }


        
        

        if (rbfps.IsAir == true) 
        {
            fallingCoroutine = falling();

            StartCoroutine(fallingCoroutine);
        }

        

        if (rbfps.IsAir == false)
        {
            animator.speed = speedo;
            if (fallingCoroutine != null)
            {
                StopCoroutine(fallingCoroutine);
                fallingCoroutine = null;
            }
            idlestopper = false;
            animator.SetBool("Air2", false);
            animator.SetBool("Air", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Hardland", false);
            animator.SetBool("Roll", false);
            animator.SetBool("runJump 0", false);
            animator.SetBool("Vault2", false);
        }

        //Slide Parameter

          if (slidePressed && velocity > 0.5f)

        {
                animator.SetTrigger("Slide");
            velocity -= Time.deltaTime * slideDeceleration;

            // Add Reference to Mouse Sensitivity to reduce the it when the player is sliding
        }

        
       

        // if (jumpPressed )
        if ((Input.GetKeyDown("space") && velocity < 0.5))
        {
            animator.SetBool("Jump", true);
            playerController.IsJummp = false;
            //velocity -= Time.deltaTime * airDrag;
            //_rigidbody.AddForce(Vector3.up * 10 * jumpHeight);
        }

        if ((Input.GetKeyDown("space") && velocity > 0.5) && detectVaultObject.Obstruction == false)
        {
            animator.SetBool("runJump 0", true);
            print("kik");
            

        }


        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping Up"))
            {
            animator.SetBool("Jump", false);
        }

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("mixamo_com"))
        {
            if (fallingCoroutine != null)
            {
                StopCoroutine(fallingCoroutine);
                fallingCoroutine = null;
            }

            playerController.IsVault = false;
            if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                animator.SetBool("Vault2", false);
            }


        }

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Hard Landing"))
        {
            stop = true;
            playerController.rb.velocity = new Vector3(0, 0, 0);
        }

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Hard Landing") == false)
        {
            stop = false ;
        }





        if (playerController.IsVault == true)
        {

            animator.speed = speedo;
            animator.SetBool("Vault2", true);
            if (fallingCoroutine != null)
            {
                StopCoroutine(fallingCoroutine);
                fallingCoroutine = null;
            }


        }

        

        if (rbfps.IsAir == true &&   Input.GetKeyDown("f")) // && velocity > 0.5f
        {
            roll = true;
            StartCoroutine(Roll());
            playerController.IsJummp = false;
            if (fallingCoroutine != null)
            {
                StopCoroutine(fallingCoroutine);
                fallingCoroutine = null;
            }
            // velocity -= Time.deltaTime * airDrag;
            //_rigidbody.AddForce(Vector3.up * 10 * jumpHeight);
        }

        if(playerController.WallrunningLeft == true)
        {
            animator.speed = speedo;

            // animator.SetBool("Wall Run L", true);

            animator.SetBool("Air2", false);
            animator.SetBool("Air", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Hardland", false);
            animator.SetBool("Roll", false);
            animator.SetBool("runJump 0", false);
            animator.SetBool("Vault2", false);
            if (fallingCoroutine != null)
            {
                StopCoroutine(fallingCoroutine);
                fallingCoroutine = null;
            }


        }

        if (playerController.WallrunningLeft == false)
        {
            animator.SetBool("Wall Run L", false);
            
            
        }

        if (playerController.WallrunningRight == true)
        {
            animator.speed = speedo;

            //animator.SetBool("Wall Run R", true);

            animator.SetBool("Air2", false);
            animator.SetBool("Air", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Hardland", false);
            animator.SetBool("Roll", false);
            animator.SetBool("runJump 0", false);
            animator.SetBool("Vault2", false);
            if (fallingCoroutine != null)
            {
                StopCoroutine(fallingCoroutine);
                fallingCoroutine = null;
            }


        }

        if (playerController.WallrunningRight == false)
        {
            animator.SetBool("Wall Run R", false);


        }


        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Falling Idle 0") && rbfps.m_IsGrounded == true)
        {
            if (roll == true)
            {
                animator.SetBool("Roll", true);
                animator.SetBool("Air2", false);
            }

            else
            {
                animator.SetBool("Hardland", true);
                print("ayy");
                animator.SetBool("Air2", false);

                rbfps.IsAir = false;
            }
            
        }

        if (idlestopper == true && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping up") == false && velocity > 1f)
        {
            animator.SetBool("Air2", true);
            idlestopper = false;
        }

        

    }



    



    IEnumerator falling()
        {

        yield return new WaitForSeconds(2.0f);
        
            
        if (rbfps.IsAir == true)
        {
            animator.speed = speedo;
            idlestopper = true;
            
        }
        
         else
        {
            animator.SetBool("Air2", false);
            idlestopper = false;
        
        }

        yield return idlestopper;



        fallingCoroutine = null;
    }

    























    internal static void SetTrigger(string v)
    {
        throw new NotImplementedException();
    }

    IEnumerator Roll()
    {
        yield return new WaitForSeconds(1f);
        roll = false;

    }
}
