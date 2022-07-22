using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using Steamworks;

public class ClientPlayer : Player 
{
    public static Dictionary<ushort, ClientPlayer> List {get; private set;} = new Dictionary<ushort, ClientPlayer>();

    public static void Spawn(ushort Id, string username, ulong steamID)
    {
        ClientPlayer player;
        Transform parent = NetworkManager.Singleton.ClientTransform;
        if(NetworkManager.Singleton.CheckLocalPlayer(Id))
            player = Instantiate(NetworkManager.Singleton.localPlayerPrefab, Vector3.zero, Quaternion.identity, parent);
        else
            player = Instantiate(NetworkManager.Singleton.nonLocalPlayerPrefab, Vector3.zero, Quaternion.identity, parent);

        player.Id = Id;
        player.username = username;
        player.steamID = new CSteamID(steamID);
        List.Add(Id, player);
    }

    void OnDestroy()
    {
        List.Remove(Id);
    }

    #region Sending

    #endregion

    #region Receiving

    [MessageHandler((ushort)ServerToClient.spawnPlayer)]
    private static void ReceiveSpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetULong());
    }

    #endregion
}