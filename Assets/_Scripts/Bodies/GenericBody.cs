using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericBody : PhysicsBody
{
	protected override void OnEnable()
    {
        base.OnEnable();
        DoStep += Step;
        NetworkManager.Singleton.OnTick += TickLoop;
    }

    void TickLoop()
    {
        Move();
        Simulate();
    }

	void Move()
	{
		rb.velocity = Movement.Gravity(rb.velocity, ServerSettings.TICK_DT);
	}

	protected override void Step(object sender, StepEventArgs e)
	{
        base.Step(sender, e);
        Move();
	}

	protected override void OnDisable()
    {
        base.OnDisable();
        DoStep -= Step;
        NetworkManager.Singleton.OnTick -= TickLoop;
    }
}
