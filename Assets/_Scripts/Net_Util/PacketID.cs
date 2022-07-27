public enum ServerToClient : ushort
{
    spawnPlayer = 1,
    correctedState
}

public enum ClientToServer : ushort
{
    spawnPlayer = 1,
    playerInput,
}