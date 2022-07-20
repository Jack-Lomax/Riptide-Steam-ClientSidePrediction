using UnityEngine;
using System.Collections.Generic;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class NetworkedBody : MonoBehaviour
{
	private Rigidbody rb;
	public static event Action OnBodySimulate;

	private const uint bufferSize = 1024;
	private NetworkedBodyState[] stateBuffer = new NetworkedBodyState[bufferSize];

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		OnBodySimulate += Rewind;

		if(Physics.autoSimulation)
			Debug.LogError("[ERROR] Auto Simulation must be disabled to use NetworkedBody");
	}

	public void Simulate()
	{
		OnBodySimulate?.Invoke();
		Physics.Simulate(Time.fixedDeltaTime);
		stateBuffer[NetworkManager.Singleton.tick % bufferSize] = GetBodyState();
	}

	void Rewind()
	{
		//TODO: IMPLEMENT
	}

	public NetworkedBodyState GetBodyState()
	{
		return new NetworkedBodyState
		{
			position = rb.position,
			rotation = rb.rotation,
			velocity = rb.velocity,
			angularVelocity = rb.angularVelocity,
		};
		
	}

	public struct NetworkedBodyState
	{
		public Vector3 position;
		public Vector3 velocity;
		public Vector3 angularVelocity;
		public Quaternion rotation;
	}
}
