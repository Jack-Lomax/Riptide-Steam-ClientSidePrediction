using UnityEngine;

public static class Movement
{
    private const float acceleration = 20;

    public static Vector3 Accelerate(Vector3 velocity, Vector3 wishDir, float dt)
    {
        float accel = acceleration * dt;

        return velocity + (wishDir * accel);
    }

    public static Vector3 Gravity(Vector3 velocity, float dt)
    {
        return velocity + (Vector3.up * ServerSettings.GRAVITY) * dt;
    }
}