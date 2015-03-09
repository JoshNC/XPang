using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class CharacterControls : MonoBehaviour {

	/// Variables
	// Public
	public float speed = 10.0f;
	public float gravity = 9.81f;
	public float maxVelocityChange = 10.0f;
	public float jumpHeight = 2.0f;
	public float mouseSensitivity = 7f;
	public float maxUpDownRotation = 60f;
	public float jumpStrength = 10f;

	public bool canJump = true;
	
	public CharacterController charCont;

	// Private
	private bool grounded = false;
	
	private float playerPitch = 0f;
	private float verticalSpeed = 0f;
	
	
	
	void Awake () {
		GetComponent<Rigidbody>().freezeRotation = true;
		GetComponent<Rigidbody>().useGravity = false;
	}
	
	// Use this for initialization
	void Start () {
		charCont = GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void FixedUpdate () {
		//Rotation
		float playerJaw = Input.GetAxis ("Mouse X") * mouseSensitivity;
		transform.Rotate(0,playerJaw,0);
		
		playerPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		playerPitch = Mathf.Clamp (playerPitch, -maxUpDownRotation, maxUpDownRotation);
		Camera.main.transform.localRotation = Quaternion.Euler(playerPitch,0,0);

		if (grounded) {
			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;
			
			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = GetComponent<Rigidbody>().velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
			
			// Jump
			if (Input.GetButtonDown("Jump") && grounded && canJump){
				Jump ();
			}
		}
		
		// We apply gravity manually for more tuning control
		GetComponent<Rigidbody>().AddForce(new Vector3 (0, -gravity * GetComponent<Rigidbody>().mass, 0));
		
		grounded = false;
	}
	
	void OnCollisionStay () {
		grounded = true;
	}
	
	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	void Jump () {
		// Calculate how fast we should be moving
		Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		targetVelocity = transform.TransformDirection(targetVelocity);
		targetVelocity *= speed;
		
		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = GetComponent<Rigidbody>().velocity;
		Vector3 velocityChange = (targetVelocity - velocity);
		velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;
		GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);

		GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
	}
}
