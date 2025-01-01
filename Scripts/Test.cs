using Godot;
using System;

public partial class Test : RigidBody2D
{
	

	public override void _Ready()
	{
		
	}

	public void PushImpulse()
	{
		if (Input.IsActionJustPressed("crouch"))
		{
            ApplyImpulse(Vector2.Up, Vector2.Up * 500);
        }
		else if(Input.IsActionJustPressed("left"))
		{
            ApplyImpulse(Vector2.Left, Vector2.Left * 500);
        }
		
    }
	
	public override void _Process(double delta)
	{
		PushImpulse();
	}
}
