using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	NetworkedBody body;

	void Awake()
	{
		body = GetComponent<NetworkedBody>();
	}

	void FixedUpdate()
	{
		body.Simulate();
	}

}
