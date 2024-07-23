using Godot;

public partial class Mob : CharacterBody3D
{
	// Minimum speed of mob in meters per second
	[Export]
	public int MinSpeed { get; set; } = 10;
	// Max speed
	[Export]
	public int MaxSpeed { get; set; } = 18;
	// Emitted when the palyer jumps on a mob
	[Signal]
	public delegate void SquashedEventHandler();

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	// Will be called from main scene
	public void Initialize(Vector3 startPosition, Vector3 playerPosition)
	{
		// We position the mob by placing it at startPosition
		// and rotate it towards playerPosition so it looks at the player
		LookAtFromPosition(startPosition, playerPosition, Vector3.Up);
		// Rotate this mob randomly within +- 90 degrees so it doesn't move directly towards the player
		RotateY((float)GD.RandRange(-Mathf.Pi / 4.0, Mathf.Pi / 4.0));

		// Calculate a random speed
		int randomSpeed = GD.RandRange(MinSpeed, MaxSpeed);
		// Calculate forward velocity
		Velocity = Vector3.Forward * randomSpeed;
		// Rotate the velocity vector based on the mob's Y rotation to move in the direction it is looking
		Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);

		// Adjust animation based on speed
		GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = randomSpeed / MinSpeed;
	}

	public void Squash()
	{
		EmitSignal(SignalName.Squashed);
		QueueFree();
	}
	
	// On leaving the screen delete the mob
	private void OnVisibleOnScreenNotifier3dScreenExited()
	{
		QueueFree();
	}
}
