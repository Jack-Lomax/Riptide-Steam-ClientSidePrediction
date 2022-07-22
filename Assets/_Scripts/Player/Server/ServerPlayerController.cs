using UnityEngine;
using RiptideNetworking;

public class ServerPlayerController : BaseController 
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    void ValidateInput(InputPayload inputPayload)
    {
        Move(inputPayload);
    }

    #region Sending

    #endregion

    #region Receiving

    [MessageHandler((ushort)ClientToServer.playerInput)]
    private static void ReceivePlayerInput(ushort fromClientId, Message message)
    {
        if(ServerPlayer.List.TryGetValue(fromClientId, out ServerPlayer player))
        {
            InputPayload inputPayload = new InputPayload
            {
                xInput = (sbyte)message.GetByte(),
                zInput = (sbyte)message.GetByte()
            };
            player.GetComponent<ServerPlayerController>().ValidateInput(inputPayload);
        }
            
    }

    #endregion
}