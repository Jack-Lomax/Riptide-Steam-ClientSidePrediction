using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class PlayerController : BaseController
{	
	//*INPUT
	[SerializeField] PlayerInput input;

	//*MODIFIERS
	public bool canMove = true;

	//TODO: Figure out how to reconcile n ticks

	protected override void OnEnable()
	{
		base.OnEnable();
		NetworkManager.Singleton.OnTick += TickLoop;
	}

	private void TickLoop()
	{
		InputPayload inputPayload = GenerateInputs();
		Move(inputPayload);
		Simulate();
	}

	InputPayload GenerateInputs()
	{
		InputPayload inputPayload = new InputPayload
		{
			xInput = input.xInput,
			zInput = input.zInput,
		};

		uint bufferSlot = NetworkManager.Singleton.tick % ServerSettings.BUFFER_SIZE;
		InputPayload inputBuffer = canMove ? inputPayload : new InputPayload();
		inputPayloadBuffer[bufferSlot] = inputBuffer;	
	
		return inputPayload;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		NetworkManager.Singleton.OnTick -= TickLoop;
	}

	void SendPlayerInput(InputPayload inputPayload)
	{
		Message message = Message.Create(MessageSendMode.unreliable, ClientToServer.playerInput);
		message.AddByte((byte)inputPayload.xInput);
		message.AddByte((byte)inputPayload.zInput);
		NetworkManager.Singleton.Client.Send(message);
	}

}
