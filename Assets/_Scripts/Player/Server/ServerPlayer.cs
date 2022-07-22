using UnityEngine;
using RiptideNetworking;
using System.Collections.Generic;
using Steamworks;

public class ServerPlayer : Player 
{
    public static Dictionary<ushort, ServerPlayer> List {get; private set;} = new Dictionary<ushort, ServerPlayer>();

    private static void Spawn(ushort Id, string username, ulong steamID)
    {
        Transform parent = NetworkManager.Singleton.ServerTransform;
        ServerPlayer player = Instantiate(NetworkManager.Singleton.serverPlayerPrefab, Vector3.zero, Quaternion.identity, parent);
        player.Id = Id;
        player.username = $"Server Player {Id} ({(username == "" ? "UNKNOWN" : username)})";
        player.steamID = new CSteamID(steamID);

        player.SendSpawn();
        List.Add(Id, player);
    }

    void OnDestroy()
    {
        List.Remove(Id);
    }
    
    #region Sending

    public void SendSpawn()
    {
        NetworkManager.Singleton.Server.SendToAll(GetSpawnMessage());
    }

    public void SendSpawn(ushort desiredClientId)
    {
        NetworkManager.Singleton.Server.Send(GetSpawnMessage(), desiredClientId);
    }

    Message GetSpawnMessage()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.spawnPlayer);
        message.Add(Id);
        message.Add(username);
        message.Add((ulong)steamID);
        return message;
    }

    #endregion

    #region Receiving

    [MessageHandler((ushort)ClientToServer.spawnPlayer)]
    private static void ReceiveSpawnPlayer(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString(), message.GetULong());
    }

    #endregion
}