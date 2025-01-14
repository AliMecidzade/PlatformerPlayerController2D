using Godot;
using System;

public partial class PlayerMovement : Node
{
    [Export] public float Speed = 500.0f;
    [Export] public float JumpVelocity = -400.0f;
    [Export] public int MaxJumps = 2;
    [Export] public float DashSpeed = 60000f;
    [Export] public NodePath SpritePath;
    [Export] public Timer DashDurationTimer;
    [Export] public Timer DashAgainTimer;
    [Export] private float GravityMultiplier = 1.8f;

    private CharacterBody2D player;
    private Sprite2D _sprite;
    private float _gravity;
    private int _jumpsRemaining;
    private bool _canDash = true;
    private bool _isDashing = false;

    private const string DashAction = "dash";
    private const string JumpAction = "ui_accept";

    public override void _Ready()
    {
        player = GetParent<CharacterBody2D>();
        Initialize();

        DashDurationTimer.Timeout += OnDashTimerTimeOut;
        DashAgainTimer.Timeout += OnDashAgainTimerTimeout;
    }

    public void Update(double delta)
    {
        var velocity = player.Velocity;

        ApplyGravity(ref velocity, delta);
        HandleMovement(ref velocity, delta);
        HandleJump(ref velocity);
        HandleDash(ref velocity, delta);

        player.Velocity = velocity;
        player.MoveAndSlide();
    }

    private void Initialize()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

        if (SpritePath != null && HasNode(SpritePath))
        {
            _sprite = GetNode<Sprite2D>(SpritePath);
        }
        else
        {
            GD.PushError("SpritePath is not set or invalid!");
        }

        _jumpsRemaining = MaxJumps;
    }

    private void ApplyGravity(ref Vector2 velocity, double delta)
    {
        if (!player.IsOnFloor())
        {
            velocity.Y += GravityMultiplier * _gravity * (float)delta;
        }
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
        var inputDirection = Input.GetAxis("ui_left", "ui_right");

        if (inputDirection != 0)
        {
            velocity.X = inputDirection * Speed;
            _sprite.FlipH = inputDirection < 0;
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed * (float)delta * 1.5f);
        }
    }

    private void HandleJump(ref Vector2 velocity)
    {
        if (player.IsOnFloor())
        {
            _jumpsRemaining = MaxJumps;
        }

        if (Input.IsActionJustPressed(JumpAction) && _jumpsRemaining > 0)
        {
            velocity.Y = JumpVelocity;
            _jumpsRemaining--;
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
}
