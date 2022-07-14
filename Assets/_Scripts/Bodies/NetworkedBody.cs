using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public abstract class NetworkedBody : MonoBehaviour
{
	public ushort ID {get; private set;}
	const uint maxID = 2147483647;
	private static ushort totalBodyCount;
	protected Rigidbody rb;

	protected Vector3 velocity;

	public virtual void Awake()
	{
		ID = totalBodyCount;
		totalBodyCount++;

		rb = GetComponent<Rigidbody>();

		if(Physics.autoSimulation)
			Physics.autoSimulation = false;
	}

	public void SimulatePhysics(float timeStep)
	{
		Activate();
		Physics.Simulate(timeStep);
		Deactivate();
	}

	private void Activate()
	{
		rb.velocity = velocity;
		rb.isKinematic = false;
	}

	private void Deactivate()
	{
		velocity = rb.velocity;
		rb.velocity = Vector3.zero;
		rb.isKinematic = true;
	}
}
