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
    [SerializeField] private CanvasGroup lobbyGraphics;
    [SerializeField] private CanvasGroup gameGraphics;

    void Awake()
    {
        Singleton = this;
        SetLobbyGraphicState(true);
    }

    public void CreateLobbyClicked()
    {
        LobbyManager.Singleton.CreateLobby();
    }

    public void LeaveLobbyClicked()
    {
        LobbyManager.Singleton.LeaveLobby();
        SetLobbyGraphicState(true);
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

    void SetLobbyGraphicState(bool state)
    {
        lobbyGraphics.alpha = state ? 1 : 0;
        lobbyGraphics.interactable = state;
        lobbyGraphics.blocksRaycasts = state;

        state = !state;
        gameGraphics.alpha = state ? 1 : 0;
        gameGraphics.interactable = state;
        gameGraphics.blocksRaycasts = state;
    }

    #region Callbacks
    public void LobbyCreationFailed()
    {
        
    }

    public void LobbyCreationSucceeded(ulong lobbyId)
    {
        SetLobbyGraphicState(false);
    }

    public void LobbyEntered()
    {

    }
    #endregion


}