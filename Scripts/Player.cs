using Godot;
using System;
using System.ComponentModel;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 500.0f;
	[Export] public float JumpVelocity = -400.0f;
	[Export] public int MaxJumps = 2;
	[Export] public float DashForce = 30000f; 
	[Export] public NodePath SpritePath;
	[Export] public int MaxDashes = 2;
	[Export] public Timer DashDurationTimer; 
	//[Export] public NodePath AnimationPlayerPath;
	
	private Sprite2D _sprite;
	private AnimationPlayer _animationPlayer;
	private float _gravity;
	private int _jumpsRemaining;
	private int _dashesRemaining;
	public override void _Ready()
	{
		InitializeComponents();
		InitializeVariables();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		ApplyGravity(ref velocity, delta);
		HandleMovement(ref velocity, delta);
		HandleJump(ref velocity);
		HandleDash(ref  velocity , delta); 
		//UpdateAnimations(velocity);

		Velocity = velocity;
		MoveAndSlide();
	}

	private void InitializeComponents()
	{
		_gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

		if (!string.IsNullOrEmpty(SpritePath))
		{
			_sprite = GetNode<Sprite2D>(SpritePath);
		}
		else
		{
			GD.PushError("SpritePath is not set!");
		}

		//if (!string.IsNullOrEmpty(AnimationPlayerPath))
		//{
		//    _animationPlayer = GetNode<AnimationPlayer>(AnimationPlayerPath);
		//}
	}

	private void InitializeVariables()
	{ 
        DashDurationTimer = GetNode<Timer>("Timer");
        _jumpsRemaining = MaxJumps;
		_dashesRemaining = MaxDashes;
	  
	}

	private void ApplyGravity(ref Vector2 velocity, double delta)
	{
		if (!IsOnFloor())
		{
			velocity.Y += _gravity * (float)delta;
		}
	}

	private void HandleDash(ref Vector2 velocity, double delta)
	{
		if (Input.IsActionPressed("dash") && _dashesRemaining > 0)
		{
		   DashDurationTimer.Start();
            var dashDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();
			if (dashDirection.Length() > 0 && DashDurationTimer.TimeLeft != 0)
			{
				GD.Print("Clicked");
				velocity += dashDirection * DashForce;
				_dashesRemaining--;
			}
		}

		if (IsOnFloor())
		{
			_dashesRemaining = MaxDashes;
		}
	}



	private void HandleMovement(ref Vector2 velocity, double delta)
	{
		var inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		if (inputDirection.X != 0)
		{
			velocity.X = inputDirection.X * Speed;
			_sprite.FlipH = inputDirection.X < 0;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Speed * (float)delta * 1.5f);
		}
	}

	private void HandleJump(ref Vector2 velocity)
	{
		if (IsOnFloor())
		{
			_jumpsRemaining = MaxJumps;
		}

		if (Input.IsActionJustPressed("ui_accept") && _jumpsRemaining > 0)
		{
			velocity.Y = JumpVelocity;
			_jumpsRemaining--;
		}
	}


	// Add Crouching 
	// Refactor the code 
	// Learn Impulsing in Godot 
	// i hate my life 

	//private void UpdateAnimations(Vector2 velocity)
	//{
	//    if (_animationPlayer == null) return;

	//    if (!IsOnFloor())
	//    {
	//        _animationPlayer.Play("jump");
	//    }
	//    else if (Mathf.Abs(velocity.X) > 0)
	//    {
	//        _animationPlayer.Play("run");
	//    }
	//    else
	//    {
	//        _animationPlayer.Play("idle");
	//    }
	//}
}
