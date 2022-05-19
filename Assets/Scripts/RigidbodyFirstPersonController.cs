using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        public bool isParkour;
        public bool IsAir = false;


      
        PlayerController playerController;
        public GameObject player;
        public int counter = 1;
        public float speedboost; 
        public float speeddecay; //how much does speed decay by
        public float speeddecay2; //speed decays by this much if going high speed
        public float speedo = 1.0f; //speed decays to this 
        public float airboost = 1.0f;
        public float airdecay = 0.7f;
        public float airdecay2;
        public float airspeedo = 20.0f; //air speed decays to this 
        public int timerint; //timer

        public PostProcessVolume volume;

        public Vignette vignetteLayer;

        




        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float SpeedInAir = 8.0f;   // Speed when onair
            public float JumpForce = 30f;
            

            




            [HideInInspector] public float CurrentTargetSpeed = 8f;
            
#if !MOBILE_INPUT
            private bool m_Running;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = ForwardSpeed;
				}

            }

        }


        public bool canrotate;
        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public Vector3 relativevelocity;

        public DetectObs detectGround;


        public bool Wallrunning;



        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        public bool  m_IsGrounded;


        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        


        private void Awake()
        {

            volume.profile.TryGetSettings(out vignetteLayer);

            vignetteLayer.intensity.value = 0.0f;

            canrotate = true;
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init (transform, cam.transform);


            playerController = player.GetComponent<PlayerController>();
            InvokeRepeating("Timer", 1.0f, 1.0f);
        }


        void Update()
        {

            if (detectGround.Obstruction)
            {
                m_IsGrounded = true;
                IsAir = false;


            }
            else
            {
                m_IsGrounded = false;

                IsAir = true;

            }
        

        isParkour = playerController.IsParkour;
            if (isParkour == true)
            {
                counter++;
                if (counter == 2)
                {
                    //speedboost
                    movementSettings.ForwardSpeed += speedboost;
                    movementSettings.SpeedInAir += airboost;
                   
                }
            }
            if (Wallrunning == true)
            {
                counter++;
                if (counter == 2)
                {
                    //wallrunning speedbost
                    movementSettings.ForwardSpeed += speedboost;
                    movementSettings.SpeedInAir += airboost;
                }
            }

            if (isParkour == false && Wallrunning == false)
            {
                counter = 1;
                
                
            }

            relativevelocity = transform.InverseTransformDirection(m_RigidBody.velocity);
            if (m_IsGrounded)
            {

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    NormalJump();
                    
                }

            }

        }

        void Timer()
        {
            if ((movementSettings.ForwardSpeed > speedo) && movementSettings.ForwardSpeed > 150.0f) //if over high speed, speed decays more to stop people from storing up too much boost at once
            {
               // vignetteLayer.intensity.value = 0.8f;   

                movementSettings.ForwardSpeed -= speeddecay2;
                
             
                    movementSettings.SpeedInAir -= airdecay2;
                
            }

            else if ((movementSettings.ForwardSpeed > speedo) && movementSettings.ForwardSpeed > 115.0f ) //just to make sure it decays to the right amount 
            {
               // vignetteLayer.intensity.value = 0.5f;

                movementSettings.ForwardSpeed -= speeddecay;
                if (movementSettings.SpeedInAir >= airspeedo)
                {
                    movementSettings.SpeedInAir -= airdecay;
                }
            }
            else if ((movementSettings.ForwardSpeed > speedo) && movementSettings.ForwardSpeed < 115.1f) //the usual decay
            {
                speeddecay = 12.0f;
                
                movementSettings.ForwardSpeed = 100.0f;
            }

            else if ((movementSettings.SpeedInAir >= airspeedo) && ( movementSettings.SpeedInAir >= 30.0f)) 
                {
                    movementSettings.SpeedInAir -= airboost;
                }
            else if ((movementSettings.SpeedInAir >= airspeedo) && (movementSettings.SpeedInAir <= 29.0f))
            {
                movementSettings.SpeedInAir = airspeedo;
            }
        }


        private void LateUpdate()
        {
            if (canrotate)
            {
                RotateView();
            }
            else
            {
                mouseLook.LookOveride(transform, cam.transform);
            }
         

        }
        public void CamGoBack(float speed)
        {
            mouseLook.CamGoBack(transform, cam.transform, speed);

        }
        public void CamGoBackAll ()
        {
            mouseLook.CamGoBackAll(transform, cam.transform);

        }
        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();

            float h = input.x;
            float v = input.y;
            Vector3 inputVector = new Vector3(h, 0, v);
            inputVector = Vector3.ClampMagnitude(inputVector, 1);

            //grounded
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && m_IsGrounded && !Wallrunning)
            {
                

                if (Input.GetAxisRaw("Vertical") > 0.3f)
                {
                    m_RigidBody.AddRelativeForce(0, 0, Time.deltaTime * 1000f * movementSettings.ForwardSpeed * Mathf.Abs(inputVector.z));
                }
                if (Input.GetAxisRaw("Vertical") < -0.3f)
                {
                    m_RigidBody.AddRelativeForce(0, 0, Time.deltaTime * 1000f * -movementSettings.BackwardSpeed * Mathf.Abs(inputVector.z));
                }
                if (Input.GetAxisRaw("Horizontal") > 0.5f)
                {
                    m_RigidBody.AddRelativeForce(Time.deltaTime * 1000f * movementSettings.StrafeSpeed * Mathf.Abs(inputVector.x), 0, 0);
                }
                if (Input.GetAxisRaw("Horizontal") < -0.5f)
                {
                    m_RigidBody.AddRelativeForce(Time.deltaTime * 1000f * -movementSettings.StrafeSpeed * Mathf.Abs(inputVector.x), 0, 0);
                }

            }
            //inair
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && !m_IsGrounded  && !Wallrunning)
            {
                

                if (Input.GetAxisRaw("Vertical") > 0.3f)
                {
                    m_RigidBody.AddRelativeForce(0, 0, Time.deltaTime * 1000f * movementSettings.SpeedInAir * Mathf.Abs(inputVector.z));
                }
                if (Input.GetAxisRaw("Vertical") < -0.3f)
                {
                        
                    m_RigidBody.AddRelativeForce(0, 0, Time.deltaTime * 1000f * -movementSettings.SpeedInAir * Mathf.Abs(inputVector.z));
                }
                if (Input.GetAxisRaw("Horizontal") > 0.5f)
                {
                    m_RigidBody.AddRelativeForce(Time.deltaTime * 1000f * movementSettings.SpeedInAir * Mathf.Abs(inputVector.x), 0, 0);
                }
                if (Input.GetAxisRaw("Horizontal") < -0.5f)
                {
                    m_RigidBody.AddRelativeForce(Time.deltaTime * 1000f * -movementSettings.SpeedInAir * Mathf.Abs(inputVector.x), 0, 0);
                }

            }
            
     
        }

        public void NormalJump()
        {
            m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
            m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
        }
        public void SwitchDirectionJump()
        {
            m_RigidBody.velocity = transform.forward * m_RigidBody.velocity.magnitude;
            m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
        }
  

      


        private Vector2 GetInput()
        {
            
            Vector2 input = new Vector2
                {
                    x = Input.GetAxisRaw("Horizontal"),
                    y = Input.GetAxisRaw("Vertical")
                };
			movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation (transform, cam.transform);

       
        }


        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
          if(detectGround.Obstruction)
            {
                m_IsGrounded = true;
                IsAir = false;

                
            }
          else
            {
                m_IsGrounded = false;
                
                IsAir = true;

            }
        }
    }
}
