using System;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class PlayerController : MonoBehaviour
{


    

    public float drag_grounded;
    public float drag_inair;

    public DetectObs detectVaultObject; //checks for vault object
    public DetectObs detectVaultObstruction; //checks if theres somthing in front of the object e.g walls that will not allow the player to vault
    public DetectObs detectClimbObject; //checks for climb object
    public DetectObs detectClimbObstruction; //checks if theres somthing in front of the object e.g walls that will not allow the player to climb
    public DetectObs detectSkipObject; //checks for vault object
    public DetectObs detectSkipObstruction; //checks if theres somthing in front of the object e.g walls that will not allow the player to vault


    public DetectObs DetectWallL; //detects for a wall on the left
    public DetectObs DetectWallR; //detects for a wall on the right

    public Animator cameraAnimator;
   

    public float WallRunUpForce;
    public float WallRunUpForce_DecreaseRate;

    private float upforce;

    public float WallJumpUpVelocity;
    public float WallJumpForwardVelocity;
    public float drag_wallrun;
    public bool WallRunning;
    public bool WallrunningLeft;
    public bool WallrunningRight;
    private bool canwallrun; // ensure that player can only wallrun once before needing to hit the ground again, can be modified for double wallruns

    public bool hardlander = false;
    public bool IsParkour;
    private float t_parkour;
    private float chosenParkourMoveTime;

    public bool isSlope;
    public bool CanVault;
    public bool IsVault = false;
    public bool IsJummp;
    public bool climbing = false;
    public bool CanSkip;

    public float VaultTime;
    public float VaultTime1; //slow vault
    public float VaultTime2;//fast vault
    public Transform VaultEndPoint;
    public Transform VaultEndPoint2;
    public float VaultSpeedDecider = 125; //if above this speed, goes to fast vault

    public float SkipTime;//fast vault
    public Transform SkipEndPoint;

    float timeElapsed;
    float lerpDuration = 1;
    float startValue = 70;
    float endValue = 100;
    float valueToLerp;

    public Camera myCamera;

    public bool CanClimb;
    public float ClimbTime; //how long the vault takes
    public Transform ClimbEndPoint;

    
    private RigidbodyFirstPersonController rbfps; 
    public Rigidbody rb;
    private Vector3 RecordedMoveToPosition; //the position of the vault end point in world space to move the player to
    private Vector3 RecordedStartPosition; // position of player right before vault
    // Start is called before the first frame update
    void Start()
    {
        rbfps = GetComponent<RigidbodyFirstPersonController>();
        rb = GetComponent<Rigidbody>();
        myCamera.fieldOfView = 70f;
        


    }

    // Update is called once per frame
    void Update()
    {
      

        


            if (myCamera.fieldOfView >= 70f)
        {
            myCamera.fieldOfView -= 1;
        }

        if (rbfps.Grounded)
        {
            rb.drag = drag_grounded;
            canwallrun = true;
        }
        else
        {
            rb.drag = drag_inair;
        }
        

            if (WallRunning)
        {
            rb.drag = drag_wallrun;

        }

        //skip
        if (detectVaultObject.Obstruction && !detectSkipObstruction.Obstruction && !CanSkip && !IsParkour && !WallRunning
            && (Input.GetKey(KeyCode.Space) || !rbfps.Grounded))
        {
            CanSkip = true;
        }

        if (CanSkip)
            {
            CanSkip = false;
            rb.isKinematic = true;
            print("skip");
            RecordedMoveToPosition = SkipEndPoint.position; //move to low speed position
            timeElapsed = 0.9f;

            RecordedStartPosition = transform.position;
            IsParkour = true;

            chosenParkourMoveTime = SkipTime;


        }






            //vault
            if (detectVaultObject.Obstruction && !detectVaultObstruction.Obstruction && !CanVault && !IsParkour && !WallRunning
            && (Input.GetKey(KeyCode.Space) || !rbfps.Grounded) && Input.GetAxisRaw("Vertical") > 0f)
        // if detects a vault object and there is no wall in front then player can pressing space or in air and pressing forward
        {
            IsVault = true;
            CanVault = true;
            
           // StartCoroutine(Vaulting());
            


        }

        if (CanVault)
        {
            
            CanVault = false; // so this is only called once
            
            rb.isKinematic = true; //ensure physics do not interrupt the vault
            if (rbfps.movementSettings.ForwardSpeed <= VaultSpeedDecider) //If going high speed 
            {


                
                RecordedMoveToPosition = VaultEndPoint.position; //move to low speed position
                VaultTime = VaultTime1;
                timeElapsed = 0.9f;
                

            }

            

            

            else if (rbfps.movementSettings.ForwardSpeed >= VaultSpeedDecider) //if going low speed
            {
                RecordedMoveToPosition = VaultEndPoint2.position; //move to high speed location
                VaultTime = VaultTime2;
                
            }

            
            RecordedStartPosition = transform.position;
            IsParkour = true;
            
            chosenParkourMoveTime = VaultTime;
           

            cameraAnimator.CrossFade("Vault",0.1f);
            

        }

        
        
        //climb
        if (detectClimbObject.Obstruction && !detectClimbObstruction.Obstruction && !CanClimb && !IsParkour && !WallRunning
            && (Input.GetKey(KeyCode.Space) || !rbfps.Grounded) && Input.GetAxisRaw("Vertical") > 0f)
        {
            CanClimb = true;
        }

        if (CanClimb)
        {
            climbing = true;
            CanClimb = false; // so this is only called once
            rb.isKinematic = true; //ensure physics do not interrupt the vault
            RecordedMoveToPosition = ClimbEndPoint.position;
            RecordedStartPosition = transform.position;
            IsParkour = true;
            chosenParkourMoveTime = ClimbTime;

            cameraAnimator.CrossFade("Climb",0.1f);
        }


        //Parkour movement
        if (IsParkour && t_parkour < 1f)
        {
            t_parkour += Time.deltaTime / chosenParkourMoveTime;
            transform.position = Vector3.Lerp(RecordedStartPosition, RecordedMoveToPosition, t_parkour);

            if (t_parkour >= 1f)
            {
                IsParkour = false;
                t_parkour = 0f;
                rb.isKinematic = false;

            }
        }


        //Wallrun
        if (DetectWallL.Obstruction && !rbfps.Grounded && !IsParkour && canwallrun) // if detect wall on the left and is not on the ground and not doing parkour(climb/vault)
        {
            WallrunningLeft = true;
            canwallrun = false;
            upforce = WallRunUpForce; //refer to line 186
        }

        if (DetectWallR.Obstruction && !rbfps.Grounded && !IsParkour && canwallrun) // if detect wall on thr right and is not on the ground
        {
            WallrunningRight = true;
            canwallrun = false;
            upforce = WallRunUpForce;
        }
        if (WallrunningLeft && !DetectWallL.Obstruction || Input.GetAxisRaw("Vertical") <= 0f || rbfps.relativevelocity.magnitude < 1f) // if there is no wall on the lef tor pressing forward or forward speed < 1 (refer to fpscontroller script)
        {
            WallrunningLeft = false;
            WallrunningRight = false;
        }
        if (WallrunningRight && !DetectWallR.Obstruction || Input.GetAxisRaw("Vertical") <= 0f || rbfps.relativevelocity.magnitude < 1f) // same as above
        {
            WallrunningLeft = false;
            WallrunningRight = false;
        }

        if (WallrunningLeft || WallrunningRight) 
        {
            WallRunning = true;
            rbfps.Wallrunning = true; // this stops the playermovement (refer to fpscontroller script)
        }
        else
        {
            WallRunning = false;
            rbfps.Wallrunning = false;
        }

        if (WallrunningLeft)
        {     
            cameraAnimator.SetBool("WallLeft", true); //Wallrun camera tilt
        }
        else
        {
            cameraAnimator.SetBool("WallLeft", false);  
        }
        if (WallrunningRight)
        {           
            cameraAnimator.SetBool("WallRight", true);
        }
        else
        {
            cameraAnimator.SetBool("WallRight", false);
        }

        if (WallRunning)
        {
            
            rb.velocity = new Vector3(rb.velocity.x, upforce ,rb.velocity.z); //set the y velocity while wallrunning
            upforce -= WallRunUpForce_DecreaseRate * Time.deltaTime; //so the player will have a curve like wallrun, upforce from line 136

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = transform.forward * WallJumpForwardVelocity + transform.up * WallJumpUpVelocity; //walljump
                WallrunningLeft = false;
                WallrunningRight = false;
            }
            if(rbfps.Grounded)
            {
                WallrunningLeft = false;
                WallrunningRight = false;
            }
        }


    }

    void OnCollisionEnter(Collision collision)
    {


        


    }


    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Slope")
        {
            
            rb.useGravity = false;
            CanVault = false;
            
        }

       
       
    }

    void OnCollisionExit(Collision collision)
    {
        rb.useGravity = true;
        CanVault = false;
        
    }

   /* IEnumerator Vaulting()
    {
        IsVault = true;
        yield return new WaitForSeconds(0.1f);
        IsVault = false;
        yield return null;
        
    }
    */
    


}
