using UnityEngine;

public static class ServerSettings 
{
    public const uint TICK_RATE = 50;
    public const float TICK_DT = 1f / TICK_RATE;
    public const uint BUFFER_SIZE = 1024;
    public const float GRAVITY = -9.81f;
}