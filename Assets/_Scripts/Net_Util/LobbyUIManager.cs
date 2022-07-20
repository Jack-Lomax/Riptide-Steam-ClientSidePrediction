using UnityEngine;
using TMPro;

public class LobbyUIManager : MonoBehaviour 
{
    private static LobbyUIManager _singleton;
    internal static LobbyUIManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(LobbyUIManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }

    [SerializeField] private TMP_InputField lobbyIdField;

    void Awake()
    {
        Singleton = this;
    }

    public void CreateLobbyClicked()
    {
        LobbyManager.Singleton.CreateLobby();
    }

    public void LeaveLobbyClicked()
    {
        LobbyManager.Singleton.LeaveLobby();
    }

    public void JoinLobbyClicked()
    {
        if(!ulong.TryParse(lobbyIdField.text, out ulong Id))
        {
            Debug.LogWarning("[WARNING] Invalid LobbyId format.");
            return;
        }

        LobbyManager.Singleton.JoinLobby(Id);
    }

    #region Callbacks
    public void LobbyCreationFailed()
    {

    }

    public void LobbyCreationSucceeded(ulong lobbyId)
    {
        
    }

    public void LobbyEntered()
    {

    }
    #endregion


}