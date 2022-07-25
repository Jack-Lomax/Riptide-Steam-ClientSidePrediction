using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody))]
public abstract class PhysicsBody : MonoBehaviour
{
	public Rigidbody rb {get; private set;}

	protected NetworkedBodyState[] stateBuffer = new NetworkedBodyState[ServerSettings.BUFFER_SIZE];

	public static EventHandler<RollbackEventArgs> DoRollback;
	public static EventHandler<StepEventArgs> DoStep;
	public static Action DoOverrideState;

	protected uint localTick;

	protected virtual void OnEnable()
	{
		rb = GetComponent<Rigidbody>();

		DoRollback += Rollback;
		DoOverrideState += OverrideState;

		localTick = 0;
		
		for(int i = 0; i < stateBuffer.Length; i++)
		{
			stateBuffer[i] = new NetworkedBodyState();
			stateBuffer[i].position = rb.position;
			stateBuffer[i].velocity = rb.velocity;
			stateBuffer[i].rotation = Quaternion.identity;
			stateBuffer[i].angularVelocity = rb.angularVelocity;
			stateBuffer[i].tick = (uint)i;
		}

		if(Physics.autoSimulation)
			Debug.LogError("Auto Simulation must be disabled to use NetworkedBody");
	}

	public void Reconcile(uint ticksToReconcile)
	{
		DoRollback(this, new RollbackEventArgs(null, ticksToReconcile));
		for(int i = 0; i < ticksToReconcile; i++)
		{
			DoStep(this, new StepEventArgs(null));
			Physics.Simulate(ServerSettings.TICK_DT);
			DoOverrideState();
		}
	}

	public void Simulate()
	{
		DoRollback(this, new RollbackEventArgs(this, 1));
		DoStep(this, new StepEventArgs(this));
		Physics.Simulate(ServerSettings.TICK_DT);	
		DoOverrideState();
	}

	protected void Rollback(object sender, RollbackEventArgs e)
	{
		if(e.bodyToIgnore != null)
			if(e.bodyToIgnore.gameObject == this.gameObject)
				return;

		localTick = (localTick - e.ticksToRollback) % ServerSettings.BUFFER_SIZE;
		NetworkedBodyState state = stateBuffer[localTick];
		SetBodyState(state);
	}

	private void OverrideState()
	{
		localTick++;
		stateBuffer[localTick % ServerSettings.BUFFER_SIZE] = GetBodyState();
	}

	protected virtual void OnDisable()
	{
		DoRollback -= Rollback;
		DoOverrideState -= OverrideState;
	}

	protected void SetBodyState(NetworkedBodyState state)
	{
		rb.position = state.position;
		rb.velocity = state.velocity;
		rb.rotation = state.rotation;
		rb.angularVelocity = state.angularVelocity;
	}

	public NetworkedBodyState GetBodyState()
	{
		return new NetworkedBodyState
		{
			position = rb.position,
			velocity = rb.velocity,
			rotation = rb.rotation,
			angularVelocity = rb.angularVelocity,
			tick = localTick,
		};
	}
}

public class NetworkedBodyState
{
	public uint tick;
	public Vector3 position;
	public Vector3 velocity;
	public Quaternion rotation;
	public Vector3 angularVelocity;
}

public class RollbackEventArgs : EventArgs
{
	public uint ticksToRollback;
	public PhysicsBody bodyToIgnore;

	public RollbackEventArgs(PhysicsBody bodyToIgnore, uint ticksToRollback)
	{
		this.bodyToIgnore = bodyToIgnore;
		this.ticksToRollback = ticksToRollback;
	}
}

public class StepEventArgs : EventArgs
{
	public PhysicsBody bodyToIgnore;

	public StepEventArgs(PhysicsBody bodyToIgnore)
	{
		this.bodyToIgnore = bodyToIgnore;
	}
}
