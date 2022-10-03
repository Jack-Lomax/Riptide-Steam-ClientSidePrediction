public enum ServerToClient : ushort
{
    spawnPlayer = 1,
    syncBody,
    correctedState
}

public enum ClientToServer : ushort
{
    spawnPlayer = 1,
    playerInput,
}