using Godot;

public partial class Player : CharacterBody3D
{
	// Speed of player
	[Export]
	public int Speed { get; set; } = 14;

	// Downward acceleration while in the air
	[Export]
	public int FallAcceleration { get; set; } = 75;
	
	// Vertical impulse applied to teh character upon jumping in meters per second
	[Export]
	public int JumpImpulse { get; set; } = 20;

	// Vertical impulse applied to the character upon bouncing over a mob in meters per second
	[Export]
	public int BounceImpuse { get; set; } = 16;

	// Emitted when player is hit by a mob
	[Signal]
	public delegate void HitEventHandler();

	private Vector3 _targetVelocity = Vector3.Zero;

	public override void _PhysicsProcess(double delta)
	{
		// Local variable to store input direction
		var direction = Vector3.Zero;

		// Check for each move input and update direction
		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1.0f;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1.0f;
		}
		// Worth noting in 3D XZ are the ground plane
		if (Input.IsActionPressed("move_back"))
		{
			direction.Z += 1.0f;
		}
		if (Input.IsActionPressed("move_forward"))
		{
			direction.Z -= 1.0f;
		}

		// Normalize vector
		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			// Basis sets the rotation
			GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
			// Change animation speed
			GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 4;
		}
		else
		{
			GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 1;
		}

		// Ground velocity
		_targetVelocity.X = direction.X * Speed;
		_targetVelocity.Z = direction.Z * Speed;

		// Vertical velocity if in air
		if (!IsOnFloor())
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		// Jumping
		if (IsOnFloor() && Input.IsActionJustPressed("jump"))
		{
			_targetVelocity.Y = JumpImpulse;
		}

		// Iterate through all collisions that occured this frame
		for (int index = 0; index < GetSlideCollisionCount(); index++)
		{
			// Get one of the collisions with the player
			KinematicCollision3D collision = GetSlideCollision(index);

			// If the collision is with a mob
			if (collision.GetCollider() is Mob mob)
			{
				// We check that we are hitting it from above
				if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
				{
					// If so we squash it and bounce
					mob.Squash();
					_targetVelocity.Y = BounceImpuse;
					// Prevent further duplicate calls
					break;
				}
			}
		}

		// Moving the character
		Velocity = _targetVelocity;
		MoveAndSlide();

		// Pivoting character
		var pivot = GetNode<Node3D>("Pivot");
		pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, pivot.Rotation.Y, pivot.Rotation.Z);
	}

	public void Die()
	{
		EmitSignal(SignalName.Hit);
		QueueFree();
	}

	private void OnMobDetectorBodyEntered(Node3D body)
	{
		Die();
	}
}
