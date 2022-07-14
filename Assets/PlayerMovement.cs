using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkedBody
{
	[SerializeField] private float speed;

	private void Update() 
	{
		float xInput = Input.GetAxisRaw("Horizontal");	
		float zInput = Input.GetAxisRaw("Vertical");	

		Vector3 wishDir = new Vector3(xInput, 0, zInput) * speed;

		velocity = wishDir;
	}

	private void FixedUpdate() 
	{
		SimulatePhysics(Time.fixedDeltaTime);	
	}
}
