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

            animator.SetBool("Climb", true);
            playerController.climbing = false;
        }

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Braced Hang To Crouch"))
        {

            animator.SetBool("Climb", false);
            
        }


        if (playerController.IsVault == true)
        {
            Debug.Log("nob");
            animator.SetTrigger("Vault");
            playerController.IsVault = false;
        }


        if (rbfps.IsAir == true) 
        {
            
            StartCoroutine(falling());
        }

        

        if (rbfps.IsAir == false)
        {
            StopCoroutine(falling());
            
            animator.SetBool("Air", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Hardland", false);
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
            print("kik");
            playerController.IsJummp = false;
            //velocity -= Time.deltaTime * airDrag;
            //_rigidbody.AddForce(Vector3.up * 10 * jumpHeight);
        }

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping Up"))
            {
            animator.SetBool("Jump", false);
        }





        if (playerController.IsVault == true)
        {
            Debug.Log("nob");
            animator.SetTrigger("Vault");
            playerController.IsVault = false;
        }

        if (Input.GetKeyDown("q")) // && velocity > 0.5f
        {
            animator.SetTrigger("runJump");
            playerController.IsJummp = false;
            // velocity -= Time.deltaTime * airDrag;
            //_rigidbody.AddForce(Vector3.up * 10 * jumpHeight);
        }



        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Falling Idle 0") && rbfps.m_IsGrounded == true)
        {
            animator.SetBool("Hardland", true);
            print("ayy");
            animator.SetBool("Air2", false);

            rbfps.IsAir = false;
            
            
        }

        if (idlestopper == true && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping up") == false)
        {
            animator.SetBool("Air2", true);
            idlestopper = false;
        }



    }
        



        IEnumerator falling()
        {


      
        yield return new WaitForSeconds(0.7f);

        if (rbfps.IsAir == true)
        {

            idlestopper = true;
            
        }
        
         else
        {
            animator.SetBool("Air2", false);
        }

          
    }

   


















            internal static void SetTrigger(string v)
    {
        throw new NotImplementedException();
    }
}
