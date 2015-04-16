using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ModifiedThirdPerson : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.


        //for other character!
		private ThirdPersonCharacter syncCharacter; // A reference to the ThirdPersonCharacter on the object
		//Transform syncCam;                  // A reference to the main camera in the scenes transform
		//Vector3 syncCamForward;             // The current forward direction of the camera
		private Vector3 syncMove;
		//private bool syncJump;
		//private bool syncCrouch;


        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
			if (GetComponent<NetworkView>().isMine){
				//InputMovement();

				if (!m_Jump)
				{
					m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
				}

			}
			else{
				syncCharacter = GetComponent<ThirdPersonCharacter> ();
				//doing nothing currently...
			}
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {

			if (GetComponent<NetworkView>().isMine)
			{
				// read inputs
				float h = CrossPlatformInputManager.GetAxis("Horizontal");
				float v = CrossPlatformInputManager.GetAxis("Vertical");
				bool crouch = Input.GetKey(KeyCode.C);
				
				// calculate move direction to pass to character
				if (m_Cam != null)
				{
					// calculate camera relative direction to move:
					m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
					m_Move = v*m_CamForward + h*m_Cam.right;
				}
				else
				{
					// we use world-relative directions in the case of no main camera
					m_Move = v*Vector3.forward + h*Vector3.right;
				}
				#if !MOBILE_INPUT
				// walk speed multiplier
				if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
				#endif
				
				// pass all parameters to the character control script
				m_Character.Move(m_Move, crouch, m_Jump);
				m_Jump = false;
			}
			else
			{

				syncCharacter = GetComponent<ThirdPersonCharacter> ();
				syncCharacter.Move(syncMove, false, false);
				//add checking for SHIFT key for running/walking
				//just setting crounch to false for now...
				//syncCharacter.Move(syncMove, false, syncJump);
				//syncJump = false;
			}
            
        }


		
		//NETWORKING TIME!!!

		//
		//
		//

		/*
		private float lastSynchronizationTime = 0f;
		private float syncDelay = 0f;
		private float syncTime = 0f;
		private Vector3 syncStartPosition = Vector3.zero;
		private Vector3 syncEndPosition = Vector3.zero;
*/

		void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
		{
			//Vector3 syncPosition = Vector3.zero;
			//Vector3 syncVelocity = Vector3.zero;

			if (stream.isWriting)
			{
				//Current player should serialize all of its data (so other client/player can see it)

				//myCharacter = m_Character;
				//can't serialize the ThirdPersonCharacter, try getting it instead?
				//stream.Serialize(ref m_Character);

				//myMove = m_Move;
				stream.Serialize(ref m_Move);

				//myJump = m_Jump;
				//stream.Serialize(ref m_Jump);

				//myCrouch = 
				//stream.Serialize(ref syncCrouch);
/*
				syncPosition = GetComponent<ThirdPersonCharacter>().GetComponent<Rigidbody>().position;
				//syncPosition = GetComponent<Rigidbody>().position;
				stream.Serialize(ref syncPosition);

				//syncPosition
				syncVelocity = m_Move;
				stream.Serialize(ref syncVelocity);
*/
			}
			else
			{
				//receiving other player's data MUST be in same order as sent out!
				//stream.Serialize(ref syncCharacter);
				//syncCharacter = GetComponent<ThirdPersonCharacter>();
				//stream.Serialize(ref syncMove);
				//stream.Serialize(ref syncJump);
				//
				//
				stream.Serialize(ref syncMove);
				/*
				stream.Serialize(ref syncPosition);
				stream.Serialize(ref syncVelocity);
				
				syncTime = 0f;
				syncDelay = Time.time - lastSynchronizationTime;
				lastSynchronizationTime = Time.time;
				
				syncEndPosition = syncPosition + syncVelocity * syncDelay;
				syncStartPosition = GetComponent<ThirdPersonCharacter>().GetComponent<Rigidbody>().position;
				*/
			}
		}

		/*
		private void SyncedMovement()
		{
			//syncTime += Time.deltaTime;
			//GetComponent<Rigidbody
			//
			//

			syncTime += Time.deltaTime;
			
			GetComponent<ThirdPersonCharacter>().GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);

		}
*/


    }
}
