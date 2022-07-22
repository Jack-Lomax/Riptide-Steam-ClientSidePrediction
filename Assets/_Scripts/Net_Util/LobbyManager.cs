using Steamworks;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager _singleton;
    internal static LobbyManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(LobbyManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEnter;

    private const string HostAddressKey = "HostAddress";
    private CSteamID lobbyId;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized!");
            return;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    internal void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 5);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            LobbyUIManager.Singleton.LobbyCreationFailed();
            return;
        }

        lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(lobbyId, HostAddressKey, SteamUser.GetSteamID().ToString());
        LobbyUIManager.Singleton.LobbyCreationSucceeded(callback.m_ulSteamIDLobby);

        NetworkManager.Singleton.Server.Start(0, 5);
        NetworkManager.Singleton.Client.Connect("127.0.0.1");
    }

    internal void JoinLobby(ulong lobbyId)
    {
        if((ulong)LobbyManager.Singleton.lobbyId == lobbyId)
        {
            Debug.LogWarning("[WARNING] Can't join lobby you're already in.");
            return;
        }

        SteamMatchmaking.JoinLobby(new CSteamID(lobbyId));
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if (NetworkManager.Singleton.Server.IsRunning)
            return;

        lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        string hostAddress = SteamMatchmaking.GetLobbyData(lobbyId, HostAddressKey);

        NetworkManager.Singleton.Client.Connect(hostAddress);
        LobbyUIManager.Singleton.LobbyEntered();
    }

    internal void LeaveLobby()
    {
        //*FIXME: THIS COULD BREAK
        if(NetworkManager.Singleton.Server.IsRunning)
            NetworkManager.Singleton.StopServer();
        NetworkManager.Singleton.DisconnectAllClients();
        SteamMatchmaking.LeaveLobby(lobbyId);
        lobbyId = new CSteamID();
    }
}