using Godot;
using System;

public partial class Player : CharacterBody2D
{
    private PlayerMovement playerMovement;

    public override void _Ready()
    {
        playerMovement = GetNode<PlayerMovement>("PlayerMovement");
    }

    public override void _PhysicsProcess(double delta)
    {
        playerMovement.Update(delta);
    }

    private void OnDeathAreaEntered(CharacterBody2D player)
    {
        GD.Print("You Died");
        QueueFree();
    }
}
