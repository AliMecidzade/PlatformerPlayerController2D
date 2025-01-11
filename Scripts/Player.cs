using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] public float Speed = 500.0f;
    [Export] public float JumpVelocity = -400.0f;
    [Export] public int MaxJumps = 2;
    [Export] public float DashSpeed = 60000f;
    [Export] public NodePath SpritePath;
    [Export] public Timer DashDurationTimer;
    [Export] public Timer DashAgainTimer;    
    [Export] private float GravityMultiplier = 1.8f;


    private bool _canDash = true;
    private bool _isDashing = false;
    private Sprite2D _sprite;
    private float _gravity;
    private int _jumpsRemaining;

    
    private const string DashAction = "dash";
    private const string JumpAction = "ui_accept";

    public override void _Ready()
    {
        Initialize();
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        ApplyGravity(ref velocity, delta);
        HandleMovement(ref velocity, delta);
        HandleJump(ref velocity);
        HandleDash(ref velocity, delta);

        Velocity = velocity;
        MoveAndSlide();
    }

    private void Initialize()
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

        _jumpsRemaining = MaxJumps;
    }

    private void ApplyGravity(ref Vector2 velocity, double delta)
    {
        if (!IsOnFloor())
        {
            velocity.Y += GravityMultiplier * _gravity * (float)delta;
        }
    }

    private void OnDashTimerTimeOut()
    {
        _isDashing = false;
    }

    private void OnDashAgainTimerTimeout()
    {
        _canDash = true;
    }

    private void HandleDash(ref Vector2 velocity, double delta)
    {
        var dashDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down").Normalized();
        if (Input.IsActionJustPressed(DashAction) && _canDash)
        {
            _isDashing = true;
            _canDash = false;
            DashDurationTimer.Start();
            DashAgainTimer.Start();
        }

        if (_isDashing)
        {
            velocity.X += dashDirection.X * DashSpeed * (float)delta;
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
        else
        {
            GravityMultiplier = 2.3f; 
        }

        if (Input.IsActionJustPressed(JumpAction) && _jumpsRemaining > 0)
        {
            velocity.Y = JumpVelocity;
            _jumpsRemaining--;
        }
    }

    private void OnDeathAreaEntered(CharacterBody2D player)
    {
        GD.Print("You Died");
        QueueFree();
    }
}
