using UnityEngine;

public class TickManager : MonoBehaviour 
{
    void Start()
    {
        NetworkManager.Singleton.OnTick += TickLoop;
    }

    void TickLoop()
    {
        
    }

    private void OnDestroy() 
	{
		NetworkManager.Singleton.OnTick -= TickLoop;
	}
}