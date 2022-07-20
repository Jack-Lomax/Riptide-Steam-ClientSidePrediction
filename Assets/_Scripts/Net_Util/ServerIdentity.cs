using UnityEngine;
using RiptideNetworking;
using System.Collections.Generic;
using Steamworks;

public class ServerIdentity : Identity 
{
    public static Dictionary<ushort, ServerIdentity> List {get; private set;} = new Dictionary<ushort, ServerIdentity>();

    private static void Spawn(ushort Id, string username, ulong steamID)
    {
        Transform parent = NetworkManager.Singleton.ServerTransform;
        ServerIdentity identity = Instantiate(NetworkManager.Singleton.serverIdentityPrefab, Vector3.zero, Quaternion.identity, parent);
        identity.Id = Id;
        identity.username = $"Server Player {Id} ({(username == "" ? "UNKNOWN" : username)})";
        identity.steamID = new CSteamID(steamID);

        identity.SendSpawn();
        List.Add(Id, identity);
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
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClient.spawnIdentity);
        message.Add(Id);
        message.Add(username);
        message.Add((ulong)steamID);
        return message;
    }

    #endregion

    #region Receiving

    [MessageHandler((ushort)ClientToServer.spawnIdentity)]
    public static void ReceiveSpawnIdentity(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString(), message.GetULong());
    }

    #endregion
}