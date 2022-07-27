using UnityEngine;
using RiptideNetworking;
using System.Collections.Generic;

public class ServerPlayerController : BaseController 
{
    private ServerPlayer localServerPlayer;

    private List<InputPayload> inputsToValidate = new List<InputPayload>();

    protected override void Awake()
    {
        base.Awake();
        localServerPlayer = GetComponent<ServerPlayer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        NetworkManager.Singleton.OnTick += TickLoop;
    }

    void TickLoop()
    {
        if(inputsToValidate.Count > 0)
        {
            InputPayload inputToValidate = inputsToValidate[0];
            inputPayloadBuffer[inputToValidate.tick % ServerSettings.BUFFER_SIZE] = inputToValidate;
            inputsToValidate.RemoveAt(0);
            Move(inputToValidate);
            Simulate();
            SendCorrectedState();
        }
    }

    protected override void OnDisable()
    {
        NetworkManager.Singleton.OnTick -= TickLoop;
        base.OnDisable();
    }

    #region Sending

    private void SendCorrectedState()
    {
        NetworkedBodyState state = GetBodyState();
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClient.correctedState);
        message.Add(localServerPlayer.Id);
        message.Add(state.tick);
        message.Add(state.position);
        message.Add(state.velocity);
        message.Add(state.rotation);
        message.Add(state.angularVelocity);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    #endregion

    #region Receiving

    [MessageHandler((ushort)ClientToServer.playerInput)]
    private static void ReceivePlayerInput(ushort fromClientId, Message message)
    {
        if(ServerPlayer.List.TryGetValue(fromClientId, out ServerPlayer player))
        {
            InputPayload inputPayload = new InputPayload
            {
                tick = message.GetUInt(),
                xInput = (sbyte)message.GetByte(),
                zInput = (sbyte)message.GetByte(),
            };
            player.GetComponent<ServerPlayerController>().inputsToValidate.Add(inputPayload);
        }
            
    }

    #endregion
}