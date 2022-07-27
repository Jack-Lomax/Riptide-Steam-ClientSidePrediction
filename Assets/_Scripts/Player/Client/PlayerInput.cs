using UnityEngine;

public class PlayerInput : MonoBehaviour 
{
    public sbyte xInput {get; private set;}
    public sbyte zInput {get; private set;}

    void Update()
    {
        xInput = (sbyte)Input.GetAxisRaw("Horizontal");
        zInput = (sbyte)Input.GetAxisRaw("Vertical");
    }
}

public class InputPayload
{
    public uint tick;
    public sbyte xInput;
    public sbyte zInput;
}

