using Godot;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }

    // On initialization
    public override void _Ready()
    {
        GetNode<Control>("UserInterface/Retry").Hide();
    }

    private void OnMobTimerTimeout()
	{
		// Create a new instance of the Mob scene
		Mob mob = MobScene.Instantiate<Mob>();

		// Choose a random location on the SpawnPath
		// Store the reference to the SpawnLocation node
		var mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
		// Give it a random offset
		mobSpawnLocation.ProgressRatio = GD.Randf();

		Vector3 playerPosition = GetNode<Player>("Player").Position;
		mob.Initialize(mobSpawnLocation.Position, playerPosition);

		// Spawn the mob by adding it to the Main scene
		AddChild(mob);
		// Connect the mob to the score label
		mob.Squashed += GetNode<ScoreLabel>("UserInterface/ScoreLabel").OnMobSquashed;
	}

	// When player hit
	private void OnPlayerHit()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Control>("UserInterface/Retry").Show();
	}

    // On player input
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept") && GetNode<Control>("UserInterface/Retry").Visible)
		{
			// Restarts current scene
			GetTree().ReloadCurrentScene();
		}
    }
}
