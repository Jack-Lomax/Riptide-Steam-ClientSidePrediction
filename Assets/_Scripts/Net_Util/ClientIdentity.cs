using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using Steamworks;

public class ClientIdentity : Identity 
{
    public static Dictionary<ushort, ClientIdentity> List {get; private set;} = new Dictionary<ushort, ClientIdentity>();

    public static void Spawn(ushort Id, string username, ulong steamID)
    {
        ClientIdentity identity;
        Transform parent = NetworkManager.Singleton.ClientTransform;
        if(NetworkManager.Singleton.CheckLocalPlayer(Id))
            identity = Instantiate(NetworkManager.Singleton.localIdentityPrefab, Vector3.zero, Quaternion.identity, parent);
        else
            identity = Instantiate(NetworkManager.Singleton.nonLocalIdentityPrefab, Vector3.zero, Quaternion.identity, parent);

        identity.Id = Id;
        identity.username = username;
        identity.steamID = new CSteamID(steamID);
        List.Add(Id, identity);
    }

    void OnDestroy()
    {
        List.Remove(Id);
    }

    #region Sending

    #endregion

    #region Receiving

    [MessageHandler((ushort)ServerToClient.spawnIdentity)]
    private static void ReceiveSpawnIdentity(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetULong());
    }

    #endregion
}