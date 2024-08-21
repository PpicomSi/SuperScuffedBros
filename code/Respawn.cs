using EZCameraShake;
using Sandbox;
using Sandbox.Network;

public sealed class Respawn : Component, Component.ITriggerListener
{
	[Property] [Sync] public List<PlayerHealth> AllPlayer {  get; set; } = new List<PlayerHealth>();
	[Property] public NetworkHelper NetworkHelper { get; set; }

	[Property] [Sync] public bool IsStarted { get; set; } = false;

	[Property] public CameraShaker fuckingcamera {  get; set; }

	[Broadcast]
	public void StartMatch()
	{
		Log.Warning( "Match Started" );
		NetworkHelper.PlayerPrefab = null;

		foreach ( var player in Scene.Components.GetAll<PlayerHealth>().Where( x => x.Tags.Has( "player" ) ) )
		{
			player.Life = 3;
			AllPlayer.Add( player );
		}
	}

	protected override void OnFixedUpdate()
	{
		if(!IsStarted)
		{
			if ( Input.Pressed( "Score" ) ) 
			{
				StartMatch();
				IsStarted = true;
			}
		}
	}

	public void OnTriggerEnter( Collider other )
	{
		if ( other.Tags.Has( "player" ) )
		{

			FuckingShake();

			Log.Info( other + "died" );
			FuckingDie( other );
		}
	}

	void LoseGame(PlayerHealth loserlol)
	{
		AllPlayer.Remove(loserlol );
	}


	public void FuckingDie( Collider other )
	{
		var health = other.Components.Get<PlayerHealth>();
		health.Percentage = 0;
		health.Life--;

		if ( health.Life < 0 )
		{
			LoseGame( health );
			health.GameObject.Destroy();

			if ( AllPlayer.Count == 1 )
			{
				GetWinner();
			}
		}

		other.Transform.Position = new Vector3(0,0,110);
	}

	[Broadcast]
	public void FuckingShake()
	{
		fuckingcamera.ShakeOnce(1000,100,0.1f,1);
	}

	[Broadcast]
	public void GetWinner()
	{
		Log.Warning( AllPlayer[0].ToString() + " Win!" );
	}
}
