using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody))]
public abstract class PhysicsBody : MonoBehaviour
{
	public Rigidbody rb {get; private set;}

	protected NetworkedBodyState[] stateBuffer = new NetworkedBodyState[ServerSettings.BUFFER_SIZE];
	[SerializeField] protected Vector3[] statePositions = new Vector3[ServerSettings.BUFFER_SIZE];

	public static EventHandler<RollbackEventArgs> DoRollback;
	public static EventHandler<StepEventArgs> DoStep;
	public static Action DoOverrideState;

	protected uint rollbackTick;

	protected virtual void OnEnable()
	{
		rb = GetComponent<Rigidbody>();

		DoRollback += Rollback;
		DoOverrideState += OverrideState;
		
		for(int i = 0; i < stateBuffer.Length; i++)
		{
			stateBuffer[i] = new NetworkedBodyState();
			stateBuffer[i].position = rb.position;
			stateBuffer[i].velocity = rb.velocity;
			stateBuffer[i].rotation = Quaternion.identity;
			stateBuffer[i].angularVelocity = rb.angularVelocity;
		}

		if(Physics.autoSimulation)
			Debug.LogError("Auto Simulation must be disabled to use NetworkedBody");

		rollbackTick = NetworkManager.Singleton.tick;
	}

	void Update()
	{
		for(int i = 0; i < ServerSettings.BUFFER_SIZE; i++)
			statePositions[i] = stateBuffer[i].position;
	}

	public void Simulate(uint ticksToSimulate = 1)
	{
		DoRollback(this, new RollbackEventArgs(this, ticksToSimulate));
		for(int i = 0; i < ticksToSimulate; i++)
		{
			DoStep(this, new StepEventArgs(this));
			Physics.Simulate(ServerSettings.TICK_DT);	
			DoOverrideState();
		}
	}

	private void Rollback(object sender, RollbackEventArgs e)
	{
		if(e.bodyToIgnore.gameObject == this.gameObject)
			return;

		uint rewindTick = (rollbackTick - 2) % ServerSettings.BUFFER_SIZE;
		NetworkedBodyState state = stateBuffer[rewindTick];
		SetBodyState(state);
		rollbackTick = rewindTick+1;
	}

	private void OverrideState()
	{
		stateBuffer[rollbackTick % ServerSettings.BUFFER_SIZE] = GetBodyState();
		rollbackTick = (rollbackTick + 1) % ServerSettings.BUFFER_SIZE;
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
			tick = NetworkManager.Singleton.tick,
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
