using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class Identity : MonoBehaviour 
{
    public ushort Id {get; protected set;}
    public string username {get; protected set;}
    public CSteamID steamID {get; protected set;}
}