using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class PlayerController : BaseController
{	
	[SerializeField] PlayerInput input;
	private List<NetworkedBodyState> correctedStates = new List<NetworkedBodyState>();

	public bool canMove = true;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		NetworkManager.Singleton.OnTick += TickLoop;
	}

	private void TickLoop()
	{
		while(correctedStates.Count > 0)
			CheckReconcile();

		InputPayload inputPayload = GenerateInputs();
		SendPlayerInput(inputPayload);
		Move(inputPayload);
		Simulate();
	}

	InputPayload GenerateInputs()
	{
		InputPayload inputPayload = new InputPayload
		{
			tick = localTick % ServerSettings.BUFFER_SIZE,
			xInput = input.xInput,
			zInput = input.zInput,
		};

		InputPayload inputBuffer = canMove ? inputPayload : new InputPayload();
		inputPayloadBuffer[inputPayload.tick] = inputBuffer;	
	
		return inputPayload;
	}

	private void CheckReconcile()
	{
		const float maxPositionDelta = 0.0000001f;

		NetworkedBodyState state = correctedStates[0];
		correctedStates.RemoveAt(0);

		Vector3 positionDelta = (state.position - stateBuffer[state.tick % ServerSettings.BUFFER_SIZE].position);
		if(positionDelta.sqrMagnitude >= .02f)
		{
			stateBuffer[state.tick % ServerSettings.BUFFER_SIZE] = state;
		}
		if(positionDelta.sqrMagnitude > maxPositionDelta)
		{
			Reconcile((localTick - state.tick) % ServerSettings.BUFFER_SIZE);
		}
	}

	public void AddCorrectedState(NetworkedBodyState state)
	{
		correctedStates.Add(state);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		NetworkManager.Singleton.OnTick -= TickLoop;
	}

	#region Sending

	void SendPlayerInput(InputPayload inputPayload)
	{
		Message message = Message.Create(MessageSendMode.unreliable, ClientToServer.playerInput);
		message.Add(inputPayload.tick);
		message.Add((byte)inputPayload.xInput);
		message.Add((byte)inputPayload.zInput);
		NetworkManager.Singleton.Client.Send(message);
	}

	#endregion

	#region Receiving

	[MessageHandler((ushort)ServerToClient.correctedState)]
	private static void ReceiveCorrectedState(Message message)
	{
		ushort id = message.GetUShort();
		NetworkedBodyState state = new NetworkedBodyState()
		{
			tick = message.GetUInt(),
			position = message.GetVector3(),
			velocity = message.GetVector3(),
			rotation = message.GetQuaternion(),
			angularVelocity = message.GetVector3(),
		};

		if(ClientPlayer.List.TryGetValue(id, out ClientPlayer player))
		{
			if(ClientPlayer.localPlayer.Id == id)
				player.GetComponent<PlayerController>().AddCorrectedState(state);
		}
	}

	#endregion

}
