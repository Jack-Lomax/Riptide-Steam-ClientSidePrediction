using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*TODO: Create a SyncedBody that uses an authoritive version on the server.

public class GenericBody : PhysicsBody
{
    protected override void Awake()
    {
        base.Awake();
    }

	protected override void OnEnable()
    {
        base.OnEnable();
        DoStep += Step;
        NetworkManager.Singleton.OnTick += TickLoop;
    }

    protected virtual void TickLoop()
    {
        Move();
        Simulate();
    }

	void Move()
	{
		rb.velocity = Movement.Gravity(rb.velocity, ServerSettings.TICK_DT);
	}

	private void Step(object sender, StepEventArgs e)
	{
        if(e.bodyToIgnore != null)
			if(e.bodyToIgnore.gameObject == this.gameObject)
				return;
        Move();
	}

	protected override void OnDisable()
    {
        base.OnDisable();
        DoStep -= Step;
        NetworkManager.Singleton.OnTick -= TickLoop;
    }
}
