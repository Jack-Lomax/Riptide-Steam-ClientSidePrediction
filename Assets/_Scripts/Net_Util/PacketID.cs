public enum ServerToClient : ushort
{
    spawnPlayer = 1
}

public enum ClientToServer : ushort
{
    spawnPlayer = 1,
    playerInput,
}