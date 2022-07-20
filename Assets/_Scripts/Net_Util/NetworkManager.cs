using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using RiptideNetworking.Transports.SteamTransport;


//*The NetworkManager is responsible for any connection changes. 
//*It handles the logic for things like players trying to connect and disconnect. 
//*And also handles the Tick system of both the Server and Clients.
public class NetworkManager : MonoBehaviour
{
	private static NetworkManager _singleton;
	internal static NetworkManager Singleton
	{
		get => _singleton;
		private set
		{
			if(!_singleton) 
				_singleton = value;
			else if(_singleton != value)
			{
				Debug.LogWarning($"{nameof(NetworkManager)} instance already exists, destroying objects");
				Destroy(value);
			}
		}
	}

	internal Server Server {get; private set;}
	internal Client Client {get; private set;}

	public uint tick; //TODO: MOVE THIS TO SERVERMANAGER

	public ClientIdentity localIdentityPrefab;
	public ClientIdentity nonLocalIdentityPrefab;
	public ServerIdentity serverIdentityPrefab;

	public Transform ServerTransform;
	public Transform ClientTransform;

	private void Awake()
	{
		Singleton = this;
	}

	private void Start()
	{
		if(!SteamManager.Initialized)
		{
			Debug.Log("Steam is not initialized");
			return;
		}

		#if UNITY_EDITOR
			RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
		#else
			RiptideLogger.Initialize(Debug.Log, true);
		#endif

		SteamServer steamServer = new SteamServer();
		Server = new Server(steamServer);
		Server.ClientConnected += ClientConnected;
		Server.ClientDisconnected += ClientDisconnected;

		Client = new Client(new SteamClient(steamServer));
		Client.Connected += LocalClientConnected;
		Client.ConnectionFailed += LocalClientFailedConnect;
		Client.Disconnected += LocalClientDisconnected;
		Client.ClientDisconnected += NonLocalClientDisconnected;
	}

	void FixedUpdate()
	{
		if(Server.IsRunning)
			Server.Tick();
		Client.Tick();
	}

	void OnApplicationQuit()
	{
		StopServer();
		Server.ClientConnected -= ClientConnected;
		Server.ClientDisconnected -= ClientDisconnected;
		DisconnectAllClients();
		Client.Connected -= LocalClientConnected;
		Client.ConnectionFailed -= LocalClientFailedConnect;
		Client.Disconnected -= LocalClientDisconnected;
		Client.ClientDisconnected -= NonLocalClientDisconnected;
	}

	public bool CheckLocalPlayer(ushort id) => id == Client.Id;

	public bool CheckServerHost(ushort id) => id == 1;
	

	#region Server

	void ClientConnected(object sender, ServerClientConnectedEventArgs e)
	{
		//Spawn Non-Local players on the just connected Client.
		foreach(ServerIdentity identity in ServerIdentity.List.Values)
			if(identity.Id != e.Client.Id)
				identity.SendSpawn(e.Client.Id);
	}

	void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
	{
		if(ServerIdentity.List.TryGetValue(e.Id, out ServerIdentity identity))
			Destroy(identity.gameObject);
	}

	public void StopServer()
	{
		Server.Stop();
		DestroyAllServerIds();
	}

	public void DisconnectAllClients()
	{
		Client.Disconnect();
		DestroyAllClientIds();
	}

	private void DestroyAllServerIds()
	{
		foreach(ServerIdentity identity in ServerIdentity.List.Values)
			Destroy(identity.gameObject);
	}

	#endregion

	#region Client

	void LocalClientConnected(object sender, EventArgs e)
	{
		//Inform the just connected client to send it's data to the server.
		Message message = Message.Create(MessageSendMode.reliable, ClientToServer.spawnIdentity);
		message.Add(Steamworks.SteamFriends.GetPersonaName());
		message.Add((ulong)Steamworks.SteamUser.GetSteamID());
		NetworkManager.Singleton.Client.Send(message);
	}	

	void LocalClientFailedConnect(object sender, EventArgs e)
	{
		
	}

	void LocalClientDisconnected(object sender, EventArgs e)
	{
		DestroyAllClientIds();
	}

	void NonLocalClientDisconnected(object sender, ClientDisconnectedEventArgs e)
	{
		if(ClientIdentity.List.TryGetValue(e.Id, out ClientIdentity identity))
			Destroy(identity.gameObject);
	}

	private void DestroyAllClientIds()
	{
		foreach(ClientIdentity identity in ClientIdentity.List.Values)
			Destroy(identity.gameObject);
		ClientIdentity.List.Clear(); //*THIS COULD CAUSE ISSUES, IDK.
	}

	#endregion
}
