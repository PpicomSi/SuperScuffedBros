using Sandbox;
using System.Diagnostics;

public sealed class Ledge : Component, Component.ITriggerListener
{
	[Property] GameObject Parent;
	[Property] float Offset;
	[Property] bool IsLeft;
	public void OnTriggerEnter(Collider other )
	{
		var player = other.Components.Get<PlayerMovement>();
		if ( player != null )
		{
			player.IsLedging = true;
			if( IsLeft )
			{
				player.IsLedgeLeft = true;
				player.Transform.LerpTo( new Transform( Parent.Transform.LocalPosition - new Vector3( 0, 0, Offset )),1f);
				player.Model.Transform.Rotation = Rotation.Lerp( player.Model.Transform.Rotation , new Angles( 0, -90, 0 ), 1f);
			}
			else
			{
				player.IsLedgeLeft = false;

				player.Transform.LerpTo( new Transform( Parent.Transform.LocalPosition - new Vector3( 0, 0, Offset )),1f);
				player.Model.Transform.Rotation = Rotation.Lerp( player.Model.Transform.Rotation, new Angles( 0, -270, 0 ), 1f );

			}
		}
	}
	public void OnTriggerExit(Collider other)
	{
		var player = other.Components.Get<PlayerMovement>();
		if ( player != null )
		{
			player.IsLedging = false;

		}
	}
}
